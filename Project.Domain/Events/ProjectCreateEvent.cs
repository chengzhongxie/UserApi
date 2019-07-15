using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Project.Domain.Events
{
    public class ProjectCreateEvent : INotification
    {
        public AggregatesModel.Project Project { get; set; }
    }
}
