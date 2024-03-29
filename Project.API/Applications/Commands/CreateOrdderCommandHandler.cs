﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.AggregatesModel;

namespace Project.API.Applications.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Domain.AggregatesModel.Project>
    {
        private IProjectRepository _projectRepository;

        public CreateOrderCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Domain.AggregatesModel.Project> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _projectRepository.AddAsync(request.Project);
            await _projectRepository.UnitOfWork.SaveChangesAsync();
            return request.Project;
        }
    }
}
