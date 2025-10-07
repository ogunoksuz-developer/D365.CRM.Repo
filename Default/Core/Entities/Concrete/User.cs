using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LCW.Core.Entities;

namespace LCW.Core.Entities.Concrete
{
    public class User : IEntity
    {
        public Guid UserId { get; set; }

        public string IdentityNumber { get; set; }

        public string Email { get; set; }

        public byte[] Password { get; set; }

        public byte[] PasswordSalt { get; set; }

        public bool Confirmed { get; set; } = false;

        public bool Status { get; set; } = false;

        [NotMapped]
        public string FirstName { get; set; }

        [NotMapped]
        public string LastName { get; set; }

   
    }
}
