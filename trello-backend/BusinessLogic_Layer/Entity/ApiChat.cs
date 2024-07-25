using DataAccess_Layer.DTOs;
using DataAccess_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic_Layer.Entity
{
    public class ApiChat
    {
        public string? NameGroup { get; set; }
        public string? AvatarGroup { get; set; }
        public bool IsGroup { get; set; }
        public List<string> Members { get; set; }
        public List<Message> Messages { get; set; }
    }
}
