using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageCreateResponse {
        public string Sender { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
    }
}
