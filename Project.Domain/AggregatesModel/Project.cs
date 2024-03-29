﻿using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Project.Domain.Events;

namespace Project.Domain.AggregatesModel
{
    public class Project : Entity, IAggregateRoot
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 项目logo
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 原BP文件地址
        /// </summary>
        public string OriginBPFile { get; set; }

        /// <summary>
        /// 转换后的BP文件地址
        /// </summary>
        public string FormatBPFile { get; set; }

        /// <summary>
        /// 是否显示敏感信息
        /// </summary>
        public bool ShowSecurityInfo { get; set; }

        /// <summary>
        /// 公司所在省Id
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 公司所在省名称
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 公司所在城市Id
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 公司所在城市名称
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 公司成立时间
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 项目基本信息
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 出让股份比例
        /// </summary>
        public string FinPercentage { get; set; }

        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }

        /// <summary>
        /// 融资金额 单位(万)
        /// </summary>
        public int FinMoney { get; set; }

        /// <summary>
        /// 收入 单位(万)
        /// </summary>
        public int Income { get; set; }

        /// <summary>
        /// 利润 单位(万)
        /// </summary>
        public int Revenue { get; set; }

        /// <summary>
        /// 估值 单位(万)
        /// </summary>
        public int Valuation { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int BrokerageOptions { get; set; }

        /// <summary>
        /// 是否委托给finbook
        /// </summary>
        public bool OnPlatform { get; set; }

        /// <summary>
        /// 可以范围设置
        /// </summary>
        public ProjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 根引用项目Id
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// 上级引用项目Id
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 项目属性：行业领域、融资币种
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }

        /// <summary>
        /// 贡献者列表
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }

        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        private Project CloneProject(Project source = null)
        {
            if (source == null)
            {
                source = this;
            }
            var newProject = new Project
            {
                AreaId = source.AreaId,
                BrokerageOptions = source.BrokerageOptions,
                Avatar = source.Avatar,
                City = source.City,
                CityId = source.CityId,
                Company = source.Company,
                Contributors = new List<ProjectContributor> { },
                Viewers = new List<ProjectViewer> { },
                CreatedTime = DateTime.Now,
                FinMoney = source.FinMoney,
                FinPercentage = source.FinPercentage,
                FinStage = source.FinStage,
                FormatBPFile = source.FormatBPFile,
                Income = source.Income,
                Introduction = source.Introduction,
                OnPlatform = source.OnPlatform,
                OriginBPFile = source.OriginBPFile,
                Properties = source.Properties,
                ProvinceId = source.ProvinceId,
                VisibleRule = source.VisibleRule == null ? null : new ProjectVisibleRule
                {
                    Visible = source.VisibleRule.Visible,
                    Tags = source.VisibleRule.Tags
                },
                Tags = source.Tags,
                Valuation = source.Valuation,
                ShowSecurityInfo = source.ShowSecurityInfo,
                RegisterTime = source.RegisterTime,
                Revenue = source.Revenue
            };
            newProject.Properties = new List<ProjectProperty> { };
            foreach (var item in source.Properties)
            {
                newProject.Properties.Add(new ProjectProperty
                (
                    item.Key,
                    item.Text,
                    item.Value
                 ));
            }
            return newProject;
        }

        /// <summary>
        /// 参与者得到项目拷贝
        /// </summary>
        /// <param name="contributorId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Project ContributorFork(string contributorId, Project source = null)
        {
            if (source == null)
            {
                source = this;
            }
            var newProject = CloneProject(source);
            newProject.UserId = contributorId;
            newProject.SourceId = source.SourceId == 0 ? source.Id : source.SourceId;
            newProject.ReferenceId = source.ReferenceId == 0 ? source.Id : source.ReferenceId;
            newProject.UpdateTime = DateTime.Now;
            return newProject;
        }

        public Project()
        {
            Viewers = new List<ProjectViewer>();
            Contributors = new List<ProjectContributor>();
            AddDomainEvent(new ProjectCreateEvent { Project = this });
        }

        public void AddViewer(string userId, string userName, string avatar)
        {
            var viewer = new ProjectViewer
            {
                UserId = userId,
                UserName = userName,
                Avatar = avatar,
                CreatedTime = DateTime.Now
            };

            if (!Viewers.Any(v => v.UserId == UserId))
            {
                Viewers.Add(viewer);
                AddDomainEvent(new ProjectViewedEvent { Viewer = viewer });
            }

           
        }

        public void AddContributor(ProjectContributor contributor)
        {
            if (!Contributors.Any(v => v.UserId == UserId))
            {
                Contributors.Add(contributor);
                AddDomainEvent(new ProjectJoinedEvent { Contributor = contributor });
            }
        }
    }
}
