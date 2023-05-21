using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models
{
    public class User : IdentityUser
    {
        public string? image { get; set; }
        [Column(TypeName = "date")]
        public DateTime birthDay { get; set; }
        public string? linkToVk { get; set; }
        public DateTime dayOfRegistration { get; set; } = DateTime.Now;
    }
}
