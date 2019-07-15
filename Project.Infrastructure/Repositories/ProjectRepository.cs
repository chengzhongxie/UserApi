using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.Domain.AggregatesModel;
using Project.Domain.Seedwork;
using ProjectEntity = Project.Domain.AggregatesModel.Project;

namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Task<ProjectEntity> AddAsync(ProjectEntity project)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectEntity> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectEntity> UpdateAsync(ProjectEntity project)
        {
            throw new NotImplementedException();
        }
    }
}
