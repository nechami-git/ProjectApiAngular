using Microsoft.EntityFrameworkCore;
using server.Models;


namespace server.DAL
{ 
        public class ChineseSaleContext : DbContext

        {
            public DbSet<UserModel> Users { get; set; }

            public DbSet<GiftModel> Gifts { get; set; }

            public DbSet<DonorModel> Donors { get; set; }

            public DbSet<TicketModel> Tickets { get; set; }

            public DbSet<CategoryModel> Categories { get; set; }


        public ChineseSaleContext(DbContextOptions<ChineseSaleContext> options) : base(options)

            {

            }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                    .Property(u => u.Role)
                    .HasConversion<string>();

            // הגדרת הקשר בין מתנה לקטגוריה
            modelBuilder.Entity<GiftModel>()
            .HasOne(g => g.Category)
            .WithMany(c => c.Gifts)
            .HasForeignKey(g => g.CategoryId);

            // הגדרת הקשר בין מתנה לתורם
            modelBuilder.Entity<GiftModel>()
            .HasOne(g => g.Donor)
            .WithMany(d => d.Donations)
            .HasForeignKey(g => g.DonorId)
            .OnDelete(DeleteBehavior.Cascade);

            // הגדרת הקשר בין כרטיס למשתמש
            modelBuilder.Entity<TicketModel>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.BuyerId);

            // הגדרת הקשר בין כרטיס למתנה
            modelBuilder.Entity<TicketModel>()
                .HasOne(t => t.Gift)
                .WithMany(g => g.Tickets)
                .HasForeignKey(t => t.GiftId);

            // הגדרת הקשר בין מתנה למשתמש כמנצח
            modelBuilder.Entity<GiftModel>()
                .HasOne(g => g.Winner)
                .WithMany()
                .HasForeignKey(g => g.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);



            base.OnModelCreating(modelBuilder);
        }

    }
    }

