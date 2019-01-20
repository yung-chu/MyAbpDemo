namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    public interface IAbpEasyNetQConfiguration
    {
        string RabbitMqConnectionString { get; set; }
    }
}
