using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageCreateRequest {
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
