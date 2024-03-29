﻿using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.AggregatesModel
{
    /// <summary>
    /// 领域模型
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetAsync(string id);

        Task<Project> AddAsync(Project project);

        Task<Project> UpdateAsync(Project project);
    }
}
