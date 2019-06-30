using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Dtos
{
    public class ServiceDisvoveryOptions
    {
        public bool Enable { get; set; }
        public string ServiceName { get; set; }

        public ConsulOptions Consul { get; set; }
    }
}
