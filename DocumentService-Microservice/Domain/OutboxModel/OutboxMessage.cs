using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OutboxModel
{
	public class OutboxMessage
	{
		public int Event_Id { get; set; }
		public required string EventPayload { get; set; }
		public DateTime Event_Date { get; set; }
		public bool IsMessageDispatched { get; set; }
	}
}
