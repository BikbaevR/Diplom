namespace Diplom.Models
{
    public class Paid
    {
        public int Id {  get; set; }
        public int EventId { get; set; }
        public DateTime RequestDate { get; set; }
        public bool done { get; set; } = false;
        public DateTime? DateOfCompletion { get; set; }
    }
}
