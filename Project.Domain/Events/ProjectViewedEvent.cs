using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Project.Domain.AggregatesModel;

namespace Project.Domain.Events
{
    public class ProjectViewedEvent : INotification
    {
        public ProjectViewer Viewer { get; set; }
    }
}
