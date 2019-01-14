using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Values;

namespace MyAbpDemo.Core
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User: FullAuditedEntity<long>, IPassivable
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Emial { get; set; }

        /// <summary>
        /// (私有)地址信息
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 行版本
        /// </summary>
        public byte[] RowVersion { get; set; }
    }

    /// <summary>
    /// https://personball.com/abp/2017/09/04/abp-why-value-object-should-be-immutable
    /// 值对象必须被设计成不可变的，当你（或者其他人）想修改它时，必须new一个新实例！
    /// </summary>
    public class Address : ValueObject<Address>
    {
        public int CityId { get; private set; } //私有 ,A reference to a City entity.

        public string Street { get; private set; } //私有

        public int Number { get; private set; } //私有

        public Address(int cityId, string street, int number)
        {
            CityId = cityId;
            Street = street;
            Number = number;
        }
    }

}
