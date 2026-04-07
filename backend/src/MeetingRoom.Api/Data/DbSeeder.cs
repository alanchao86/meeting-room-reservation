using MeetingRoom.Api.Models.Entities;

namespace MeetingRoom.Api.Data;

public static class DbSeeder {
    public static void Seed(AppDbContext dbContext) {
        var utcNow = DateTime.UtcNow;

        if (!dbContext.Users.Any()) {
            dbContext.Users.Add(new User {
                Id = "default-user",
                DisplayName = "Default Test User",
                IsDefault = true,
                CreatedAt = utcNow
            });
        }

        var seedRooms = new[] {
            new Room {
                Id = "room-a",
                RoomName = "Discussion Room",
                RoomNumber = "A-301",
                Capacity = 4,
                ImageUrl = "/assets/demo-rooms/small-meeting-room.avif"
            },
            new Room {
                Id = "room-b",
                RoomName = "Meeting Room A",
                RoomNumber = "A-305",
                Capacity = 10,
                ImageUrl = "/assets/demo-rooms/meeting-room-a.jpg"
            },
            new Room {
                Id = "room-c",
                RoomName = "Meeting Room B",
                RoomNumber = "A-306",
                Capacity = 10,
                ImageUrl = "/assets/demo-rooms/meeting-room-b.jpg"
            },
            new Room {
                Id = "room-d",
                RoomName = "Conference Room",
                RoomNumber = "B-201",
                Capacity = 35,
                ImageUrl = "/assets/demo-rooms/conference-room.jpg"
            }
        };

        foreach (var seed in seedRooms) {
            var existing = dbContext.Rooms.FirstOrDefault(x => x.Id == seed.Id);
            if (existing is null) {
                seed.CreatedAt = utcNow;
                seed.UpdatedAt = utcNow;
                dbContext.Rooms.Add(seed);
                continue;
            }

            // Keep room seed stable across runs so demo data doesn't drift.
            existing.RoomName = seed.RoomName;
            existing.RoomNumber = seed.RoomNumber;
            existing.Capacity = seed.Capacity;
            existing.ImageUrl = seed.ImageUrl;
            existing.UpdatedAt = utcNow;
        }

        dbContext.SaveChanges();
    }
}
