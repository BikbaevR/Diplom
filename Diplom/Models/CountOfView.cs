using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models
{
	public class CountOfView
	{
		public int Id { get; set; }
		public int EventId { get; set; }
		public DateTime date { get; set; }
		public int count { get; set; }

	}
}
