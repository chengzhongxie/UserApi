﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.API.Dtos
{
    public class UserIdentity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Company { get; set; }
        public string Tiatle { get; set; }
        public string Avatar { get; set; }
    }
}
