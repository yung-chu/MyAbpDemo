using Castle.Core.Logging;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.Logging;
using EasyNetQ.SystemMessages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQComponent = EasyNetQ;

namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    public class ConsumerErrorStategy : IConsumerErrorStrategy
    {
        private readonly ILogger _logger;
        private readonly EasyNetQComponent.IConnectionFactory connectionFactory;
        private readonly ISerializer serializer;
        private readonly IConventions conventions;
        private readonly ITypeNameSerializer typeNameSerializer;
        private readonly IErrorMessageSerializer errorMessageSerializer;
        private readonly object syncLock = new object();

        private IConnection connection;

        public ConsumerErrorStategy(
            EasyNetQComponent.IConnectionFactory connectionFactory,
            ILogger logger,
            ISerializer serializer,
            IConventions conventions,
            ITypeNameSerializer typeNameSerializer,
            IErrorMessageSerializer errorMessageSerializer)
        {
            this._logger = logger;
            this.connectionFactory = connectionFactory;
            this.serializer = serializer;
            this.conventions = conventions;
            this.typeNameSerializer = typeNameSerializer;
            this.errorMessageSerializer = errorMessageSerializer;
        }

        /// <summary>
        /// 连接RabbitMQ客户端
        /// </summary>
        protected void Connect()
        {
            if (connection == null || !connection.IsOpen)
            {
                lock (syncLock)
                {
                    if ((connection == null || !connection.IsOpen) && !(disposing || disposed))
                    {
                        if (connection != null)
                        {
                            try
                            {
                                connection.Dispose();
                            }
                            catch
                            {
                                if (connection.CloseReason != null)
                                {
                                    _logger.InfoFormat("Connection {connection} has shutdown with reason={reason}", connection.ToString(), connection.CloseReason.Cause);
                                }
                                else
                                {
                                    throw;
                                }
                            }

                        }

                        connection = connectionFactory.CreateConnection();
                        if (disposing || disposed)
                        {
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 声明失败队列
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string DeclareFailedExchangeWithQueue(IModel model, ConsumerExecutionContext context)
        {
            var failedExchangeName = $"Failed_{context.Info.Exchange}";
            var failedQueueName = $"Failed_{context.Info.Queue}";
            var routingKey = string.IsNullOrEmpty(context.Info.RoutingKey) ? context.Info.Queue.Substring(0, context.Info.Exchange.Length + 1) : context.Info.RoutingKey;
            DeclareAndBindExchangeWithQueue(model, failedExchangeName, failedQueueName, routingKey);

            return failedExchangeName;
        }

        /// <summary>
        /// 声明重试队列
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string DeclareReTryExchangeWithQueue(IModel model, ConsumerExecutionContext context)
        {
            var reTryExchangeName = $"ReTry_{context.Info.Exchange}";
            var reTryQueueName = $"ReTry_{context.Info.Queue}";
            var routingKey = string.IsNullOrEmpty(context.Info.RoutingKey) ? context.Info.Queue.Substring(context.Info.Exchange.Length + 1) : context.Info.RoutingKey;
            DeclareAndBindExchangeWithQueue(model, reTryExchangeName, reTryQueueName, routingKey);
            return reTryExchangeName;
        }

        /// <summary>
        /// 声明队列并绑定
        /// </summary>
        /// <param name="model"></param>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        private void DeclareAndBindExchangeWithQueue(IModel model, string exchangeName, string queueName, string routingKey)
        {
            model.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            model.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true);
            model.QueueBind(queueName, exchangeName, routingKey);
        }

        /// <summary>
        /// 消费异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public virtual AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            if (disposed || disposing)
            {
                _logger.ErrorFormat(
                    "ErrorStrategy was already disposed, when attempting to handle consumer error. Error message will not be published and message with receivedInfo={receivedInfo} will be requeued",
                    context.Info
                );

                return AckStrategies.NackWithRequeue;
            }

            try
            {
                Connect();
                _logger.Error($"消费类型为{context.Properties.Type ?? "UnKown"}的消息时出现异常 {exception.ToString()}");
                using (var model = connection.CreateModel())
                {
                    try
                    {
                        var temp = context.Properties.Headers["ReTryCount"];
                        if ((int)temp >= 5)
                        {
                            //进入失败队列，不重试
                            var failedExchange = DeclareFailedExchangeWithQueue(model, context);
                            var failedProperties = model.CreateBasicProperties();
                            failedProperties.Persistent = true;
                            failedProperties.Type = context.Properties.Type == null ? typeNameSerializer.Serialize(typeof(Object)) : context.Properties.Type;
                            var failedRoutingKey = string.IsNullOrEmpty(context.Info.RoutingKey) ? context.Info.Queue.Substring(context.Info.Exchange.Length + 1) : context.Info.RoutingKey;
                            model.BasicPublish(failedExchange, failedRoutingKey, failedProperties, context.Body);
                            return AckStrategies.Ack;

                        }
                        else
                        {
                            context.Properties.Headers["ReTryCount"] = (int)context.Properties.Headers["ReTryCount"] + 1;
                        }
                    }
                    catch (Exception e)
                    {
                        context.Properties.Headers.Add("ReTryCount", 1);
                    }
                    //进入死信队列，进行重试
                    var errorExchange = DeclareReTryExchangeWithQueue(model, context);
                    var messageBody = context.Body; //CreateErrorMessage(context, exception);
                    var properties = model.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Type = context.Properties.Type == null ? typeNameSerializer.Serialize(typeof(Object)) : context.Properties.Type;
                    properties.Headers = context.Properties.Headers;
                    var errorRoutingKey = string.IsNullOrEmpty(context.Info.RoutingKey) ? context.Info.Queue.Substring(context.Info.Exchange.Length + 1) : context.Info.RoutingKey;
                    model.BasicPublish(errorExchange, errorRoutingKey, properties, messageBody);
                    return AckStrategies.Ack;
                }
            }
            catch (BrokerUnreachableException unreachableException)
            {
                _logger.Error($"Cannot connect to broker while attempting to publish error message {unreachableException.ToString()}");
            }
            catch (OperationInterruptedException interruptedException)
            {
                _logger.Error($"Broker connection was closed while attempting to publish error message {interruptedException.ToString()}");
            }
            catch (Exception unexpectedException)
            {
                _logger.Error($"Failed to publish error message {unexpectedException.ToString()}");
            }
            return AckStrategies.NackWithRequeue;
        }

        public AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
        {
            return AckStrategies.NackWithRequeue;
        }

        private byte[] CreateErrorMessage(ConsumerExecutionContext context, Exception exception)
        {
            var messageAsString = errorMessageSerializer.Serialize(context.Body);
            var error = new Error
            {
                RoutingKey = context.Info.RoutingKey,
                Exchange = context.Info.Exchange,
                Queue = context.Info.Queue,
                Exception = exception.ToString(),
                Message = messageAsString,
                DateTime = DateTime.UtcNow
            };

            if (context.Properties.Headers == null)
            {
                error.BasicProperties = context.Properties;
            }
            else
            {
                error.BasicProperties = (MessageProperties)context.Properties.Clone();
                error.BasicProperties.Headers = context.Properties.Headers.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value is byte[] ? Encoding.UTF8.GetString((byte[])kvp.Value) : kvp.Value);
            }

            return serializer.MessageToBytes(error);
        }

        private bool disposed;
        private bool disposing;

        /// <summary>
        /// 回收释放
        /// </summary>
        public virtual void Dispose()
        {
            if (disposed) return;
            disposing = true;

            if (connection != null) { connection.Dispose(); }

            disposed = true;
        }
    }
}
