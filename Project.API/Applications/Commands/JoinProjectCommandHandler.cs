using MediatR;
using Project.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.API.Applications.Commands
{
    public class JoinProjectCommandHandler : IRequestHandler<JoinProjectCommand>
    {
        private IProjectRepository _projectRepository;

        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        async Task<Unit> IRequestHandler<JoinProjectCommand, Unit>.Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.Contributor.ProjectId.ToString());
            if (project == null)
            {
                throw new Domain.Exceptions.OrderingDomainException($"project not found:{request.Contributor.ProjectId.ToString()}");
            }
            project.AddContributor(request.Contributor);
            await _projectRepository.UnitOfWork.SaveChangesAsync();
            return new Unit();
        }
    }
}
