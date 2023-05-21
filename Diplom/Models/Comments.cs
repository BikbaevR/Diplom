using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models
{
	public class Comments
	{
		public int Id { get; set; }
		public int event_id { get; set; }
		public string user_id { get; set; }
		public string comment { get; set; }
		public DateTime comment_date { get; set; } = DateTime.Now;
		public bool edit { get; set; } = false;

	}
}
