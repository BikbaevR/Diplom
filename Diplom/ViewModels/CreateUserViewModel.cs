using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.ViewModels
{
    public class CreateUserViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [Column(TypeName = "date")]
        public DateTime birthDay { get; set; }

    }
}
