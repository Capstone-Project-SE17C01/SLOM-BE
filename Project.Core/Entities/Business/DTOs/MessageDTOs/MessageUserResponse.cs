using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageUserResponse {
        public string UserName { get; set; }
        public string LastMessage { get; set; }
        public bool IsSender { get; set; }
        public bool IsSeen { get; set; }
        public string Avatar { get; set; }
        public string LastSent { get; set; }
    }
}
