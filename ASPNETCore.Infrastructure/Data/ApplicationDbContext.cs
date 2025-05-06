//using ASPNETCore.Domain.Entities;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace ASPNETCore.Infrastructure.Data
//{
//    public class ApplicationDbContext : IdentityDbContext<User>
//    {
//        public virtual DbSet<DealType> DealTypes { get; set; }
//        public virtual DbSet<REObject> Objects { get; set; }
//        public virtual DbSet<ObjectType> ObjectTypes { get; set; }
//        public virtual DbSet<Status> Statuses { get; set; }

//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);
//            modelBuilder.Entity<DealType>()
//                .Property(e => e.DealName)
//                .IsUnicode(false);

//            modelBuilder.Entity<DealType>()
//                .HasMany(e => e.Object)
//                .WithOne(e => e.DealType)
//                .OnDelete(DeleteBehavior.Restrict);

//            modelBuilder.Entity<REObject>()
//                .Property(e => e.Street)
//                .IsUnicode(false);

//            modelBuilder.Entity<ObjectType>()
//                .Property(e => e.TypeName)
//                .IsUnicode(false);

//            modelBuilder.Entity<ObjectType>()
//                .HasMany(e => e.Object)
//                .WithOne(e => e.ObjectType)
//                .HasForeignKey(e => e.TypeId)
//                .OnDelete(DeleteBehavior.Restrict);

//            modelBuilder.Entity<Status>()
//                .Property(e => e.StatusName)
//                .IsUnicode(false);

//            modelBuilder.Entity<Status>()
//                .HasMany(e => e.Object)
//                .WithOne(e => e.Status)
//                .OnDelete(DeleteBehavior.Restrict);

//            modelBuilder.Entity<User>()
//                .Property(e => e.FullName)
//                .IsUnicode(false);

//            modelBuilder.Entity<User>()
//                .Property(e => e.Passport)
//                .IsUnicode(false);

//            //modelBuilder.Entity<User>()
//            //    .Property(e => e.Phone)
//            //    .IsUnicode(false);

//            modelBuilder.Entity<User>()
//                .Property(e => e.UserName)
//                .IsUnicode(false);


//            modelBuilder.Entity<UserRole>()
//                .Property(e => e.RoleName)
//                .IsUnicode(false);

//        }
//    }
//}
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public virtual DbSet<DealType> DealTypes { get; set; }
        public virtual DbSet<REObject> Objects { get; set; }
        public virtual DbSet<ObjectType> ObjectTypes { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ResStatus> ResStatuses { get; set; }
        public virtual DbSet<ObjectImages> ObjectImages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DealType>()
                        .Property(e => e.DealName)
                    .IsUnicode(false);

            modelBuilder.Entity<DealType>()
                .HasMany(e => e.Object)
                .WithOne(e => e.DealType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<REObject>()
                .Property(e => e.Street)
                .IsUnicode(false);

            modelBuilder.Entity<ObjectType>()
                .Property(e => e.TypeName)
                .IsUnicode(false);

            modelBuilder.Entity<ObjectType>()
                .HasMany(e => e.Object)
                .WithOne(e => e.ObjectType)
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Status>()
                .Property(e => e.StatusName)
                .IsUnicode(false);

            modelBuilder.Entity<Status>()
                .HasMany(e => e.Object)
                .WithOne(e => e.Status)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Passport)
                .IsUnicode(false);

            // Конфигурация ResStatus
            modelBuilder.Entity<ResStatus>()
                .Property(rs => rs.StatusType)
                .IsUnicode(false)
                .HasMaxLength(50);

            // Конфигурация Contract
            modelBuilder.Entity<Contract>()
                .Property(c => c.SignDate)
                .HasColumnType("date");

            modelBuilder.Entity<Contract>()
                .Property(c => c.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Reservation)
                .WithMany()
                .HasForeignKey(c => c.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectImages>(entity =>
            {
                entity.Property(e => e.ImagePath)
                    .IsUnicode(false)
                    .IsRequired();
                entity.HasOne(oi => oi.Object)
                    .WithMany(o => o.ObjectImages)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}