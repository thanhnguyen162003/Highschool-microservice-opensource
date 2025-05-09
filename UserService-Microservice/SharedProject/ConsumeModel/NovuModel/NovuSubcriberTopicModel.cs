using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProjects.ConsumeModel.NovuModel
{
    public class NovuSubcriberTopicModel
    {
        public List<string> SubcriberIds { get; set; }= new List<string>();
        public string TopicKey { get; set; }
    }
}
