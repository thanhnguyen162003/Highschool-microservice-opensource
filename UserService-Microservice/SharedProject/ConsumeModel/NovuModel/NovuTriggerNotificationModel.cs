using SharedProjects.ConsumeModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProjects.ConsumeModel.NovuModel
{
    public class NovuTriggerNotificationModel
    {
        public string WorkflowId { get; set; }
        public string TargetId { get; set; }
        public object Payload { get; set; }
        public NotificationTriggerType NotificationTriggerType { get; set; }
    }
}
