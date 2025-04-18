using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageContent {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsSender { get; set; }
    }

    public class MessageResponse {
        public bool IsLoadFullPage { get; set; }
        public List<MessageContent> Data { get; set; }
    }
}
