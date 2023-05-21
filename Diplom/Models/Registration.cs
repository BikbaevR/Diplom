namespace Diplom.Models
{
	public class Registration
	{
		public int Id { get; set; }
		public int event_id { get; set; }
		public string user_id { get; set; }
		public DateTime registration_date { get; set; } = DateTime.Now;
		public bool appreciated { get; set; } = false;
		public string status_of_registrate { get; set; }
	}
}
