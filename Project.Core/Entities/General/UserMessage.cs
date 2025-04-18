using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.General
{
    public class UserMessage {
        public int MessageId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public Profile Sender {  get; set; }
        public Profile Receiver {  get; set; }
    }
}
