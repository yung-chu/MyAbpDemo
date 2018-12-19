namespace MyAbpDemo.Infrastructure.Api
{
    public class JwtSetting
    {   
        /// <summary>
        ///签名秘钥
        /// </summary>
        public string ServerSecret { get; set; }
        /// <summary>
        /// 颁发机构
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 颁发受众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期天数
        /// </summary>
        public int ExpireDays { get; set; }
    }
}
