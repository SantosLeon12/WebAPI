using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public int PKUser { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [ForeignKey("Roles")]
        public int? FKRol { get; set; }
        public Rol Roles { get; set; }
    }
}
