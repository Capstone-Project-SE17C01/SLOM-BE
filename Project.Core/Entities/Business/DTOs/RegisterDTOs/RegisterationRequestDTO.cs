using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.RegisterDTOs
{
    public class RegisterationRequestDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}
