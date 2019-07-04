using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.API
{
    public class AppSettings
    {
        /// <summary>
        /// MongoDb 链接字符串
        /// </summary>
        public string MongoContactConnectionString { get; set; }
        /// <summary>
        /// MongoDb 数据库
        /// </summary>
        public string MongoContectDatabase { get; set; }
    }
}
