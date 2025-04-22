using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.MessageDTOs {
    public class MessageUserResponse {
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string LastMessage { get; set; }
        public bool IsSender { get; set; }
        public bool IsSeen { get; set; }
        public required string Avatar { get; set; }
        public required string LastSent { get; set; }
    }
}
