using MeetingRoom.Api.Models.Entities;

namespace MeetingRoom.Api.Data;

public static class DbSeeder {
    public static void Seed(AppDbContext dbContext) {
        // Get the current UTC time.
        var utcNow = DateTime.UtcNow;

        if (!dbContext.Users.Any()) {
            dbContext.Users.Add(new User {
                Id = "default-user",
                DisplayName = "Default Test User",
                IsDefault = true,
                CreatedAt = utcNow
            });
        }

        if (!dbContext.Rooms.Any()) {
            dbContext.Rooms.AddRange(
                new Room {
                    Id = "room-a",
                    RoomName = "Discussion Room",
                    RoomNumber = "A-301",
                    Capacity = 4,
                    ImageUrl = "/assets/demo-rooms/small-meeting-room.avif",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                },
                new Room {
                    Id = "room-b",
                    RoomName = "Meeting Room A",
                    RoomNumber = "A-305",
                    Capacity = 10,
                    ImageUrl = "/assets/demo-rooms/meeting-room-a.jpg",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                },
                new Room {
                    Id = "room-c",
                    RoomName = "Meeting Room B",
                    RoomNumber = "A-306",
                    Capacity = 10,
                    ImageUrl = "/assets/demo-rooms/meeting-room-b.jpg",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                },
                new Room {
                    Id = "room-d",
                    RoomName = "Conference Room",
                    RoomNumber = "B-201",
                    Capacity = 35,
                    ImageUrl = "/assets/demo-rooms/conference-room.jpg",
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                });
        }

        dbContext.SaveChanges();
    }
}
