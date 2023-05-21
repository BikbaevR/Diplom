using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom.Models
{
    public class Events
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string status { get; set; } = "news";
        public string short_description { get; set; }
        public string description { get; set; }
        public string img_one { get; set; }
        public string? img_two { get; set;}
        public string? img_three { get; set;}
        public int age_limit { get; set; }
        public DateTime start_date { get; set; }
        public string city { get; set; }
        public int number_of_registration { get; set; } = 0;
        public string? created_by { get; set; }
        public bool paid { get; set; } = false;
        public int registrations_required { get; set; }
        public int rating { get; set; } = 0;
        public DateTime created_date { get; set; } = DateTime.Now;
        public string catigory { get; set; }
    }
}
