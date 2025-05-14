using Microsoft.EntityFrameworkCore;
using server1.Models;




public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
       

        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Announcement> Announcements { get; set; } //Added Announcements




    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Announcement>() //Configure Announcement Entity
               .Property(a => a.StartDate)
               .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Announcement>()
                .Property(a => a.EndDate)
                .HasColumnType("timestamp with time zone");


    }


}

