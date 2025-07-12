using Project.Core.Entities.General;

namespace Project.Tests.TestData.SeedData
{
    public static class MeetingSeed
    {
        public static List<Meeting> GetMeetings()
        {
            return new List<Meeting>
            {
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    HostId = Guid.NewGuid(),
                    Title = "Cuộc họp 1",
                    Description = "Cuộc họp kiểm thử 1",
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Status = "Scheduled",
                    MaxParticipants = 50,
                    IsPrivate = false,
                    GuestCode = null,
                    CreatedAt = DateTime.UtcNow
                },
                new Meeting
                {
                    Id = Guid.NewGuid(),
                    HostId = Guid.NewGuid(),
                    Title = "Cuộc họp 2",
                    Description = "Cuộc họp kiểm thử 2",
                    StartTime = DateTime.UtcNow.AddDays(2),
                    EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
                    Status = "Scheduled",
                    MaxParticipants = 50,
                    IsPrivate = false,
                    GuestCode = null,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static Meeting GetSingleMeeting()
        {
            return new Meeting
            {
                Id = Guid.NewGuid(),
                HostId = Guid.NewGuid(),
                Title = "Cuộc họp kiểm thử",
                Description = "Dành cho kiểm thử",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1),
                Status = "Scheduled",
                MaxParticipants = 50,
                IsPrivate = false,
                GuestCode = null,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
