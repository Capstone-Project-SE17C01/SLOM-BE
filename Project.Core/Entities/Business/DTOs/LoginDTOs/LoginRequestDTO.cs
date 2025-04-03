using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Entities.Business.DTOs.LoginDTOs
{
    public class LoginRequestDTO
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
