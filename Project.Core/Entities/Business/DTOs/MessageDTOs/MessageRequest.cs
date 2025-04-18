using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageRequest {
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; }
        public int PageNumber { get; set; }
    }
}
