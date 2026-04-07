using MeetingRoom.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Data;

// EF Core model configuration
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // users
        modelBuilder.Entity<User>(entity => {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").HasMaxLength(64);
            entity.Property(x => x.DisplayName).HasColumnName("display_name").IsRequired().HasMaxLength(120);
            entity.Property(x => x.IsDefault).HasColumnName("is_default").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        });

        // rooms
        modelBuilder.Entity<Room>(entity => {
            entity.ToTable("rooms", table => {
                table.HasCheckConstraint("ck_rooms_capacity", "capacity > 0");
            });

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").HasMaxLength(64);
            entity.Property(x => x.RoomName).HasColumnName("room_name").IsRequired().HasMaxLength(120);
            entity.Property(x => x.RoomNumber).HasColumnName("room_number").IsRequired().HasMaxLength(40);
            entity.Property(x => x.Capacity).HasColumnName("capacity").IsRequired();
            entity.Property(x => x.ImageUrl).HasColumnName("image_url");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            entity.HasIndex(x => x.RoomNumber).IsUnique();
        });

        // reservations
        modelBuilder.Entity<Reservation>(entity => {
            entity.ToTable("reservations", table => {
                // Ensures valid slot ranges
                table.HasCheckConstraint(
                    "ck_reservations_slot_range",
                    "start_slot_index >= 0 AND start_slot_index <= 19 AND end_slot_index >= 1 AND end_slot_index <= 20 "
                    + "AND start_slot_index < end_slot_index AND (end_slot_index - start_slot_index) <= 4");
            });

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").HasMaxLength(80);
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired().HasMaxLength(64);
            entity.Property(x => x.RoomId).HasColumnName("room_id").IsRequired().HasMaxLength(64);
            entity.Property(x => x.ReservationDate).HasColumnName("reservation_date").IsRequired();
            entity.Property(x => x.StartSlotIndex).HasColumnName("start_slot_index").IsRequired();
            entity.Property(x => x.EndSlotIndex).HasColumnName("end_slot_index").IsRequired();
            entity.Property(x => x.EventName).HasColumnName("event_name").IsRequired().HasMaxLength(200);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            // Speeds up conflict checks and room availability lookup by room & date.
            entity.HasIndex(x => new { x.RoomId, x.ReservationDate });
            // Keeps "My Reservations" listing fast with current sort pattern.
            entity.HasIndex(x => new { x.UserId, x.ReservationDate, x.StartSlotIndex });

            // Do not allow accidental user deletion while reservations still exist.
            entity.HasOne(x => x.User)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Removing a room also removes dependent reservations.
            entity.HasOne(x => x.Room)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
