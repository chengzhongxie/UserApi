﻿using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Domain.AggregatesModel
{
    public class ProjectContributor : Entity
    {
        public Guid ProjectId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 关闭者
        /// </summary>
        public bool IsCloser { get; set; }

        /// <summary>
        /// 1:财务顾问 2：投资机构
        /// </summary>
        public int ContributorType { get; set; }
    }
}
