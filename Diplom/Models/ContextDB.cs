using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace Diplom.Models
{
    public class ContextDB : DbContext
    {
        public DbSet<Events> Events { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Catigory> Category { get; set; }
        public DbSet<CountOfView> CountOfView { get; set; }
        public DbSet<Paid> Paid { get; set; }



		public ContextDB(DbContextOptions<ContextDB> options) : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

    }
}
