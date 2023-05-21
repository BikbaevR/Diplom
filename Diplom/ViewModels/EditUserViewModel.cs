using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? image { get; set; }
        public string? linkToVk { get; set; }
        [Column(TypeName = "date")]
        public DateTime birthDay { get; set; }
    }
}
