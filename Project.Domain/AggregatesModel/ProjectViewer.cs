using System;
using System.Collections.Generic;
using System.Text;
using Project.Domain.Seedwork;

namespace Project.Domain.AggregatesModel
{
    public class ProjectViewer : Entity
    {
        public Guid ProjectId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
