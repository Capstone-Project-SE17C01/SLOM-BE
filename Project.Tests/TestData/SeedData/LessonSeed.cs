using Project.Core.Entities.General;

namespace Project.Tests.TestData.SeedData {
    public static class LessonSeed {
        public static List<Lesson> GetLessons() {
            return new List<Lesson>
            {
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    ModuleId = Guid.NewGuid(),
                    Title = "Bài học 1: Giới thiệu",
                    Content = "Nội dung bài học 1",
                    VideoUrl = null,
                    DurationMinutes = 10,
                    OrderNumber = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Lesson
                {
                    Id = Guid.NewGuid(),
                    ModuleId = Guid.NewGuid(),
                    Title = "Bài học 2: Bảng chữ cái",
                    Content = "Nội dung bài học 2",
                    VideoUrl = null,
                    DurationMinutes = 15,
                    OrderNumber = 2,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static Lesson GetSingleLesson() {
            return new Lesson {
                Id = Guid.NewGuid(),
                ModuleId = Guid.NewGuid(),
                Title = "Bài học kiểm thử",
                Content = "Nội dung kiểm thử",
                VideoUrl = null,
                DurationMinutes = 20,
                OrderNumber = 99,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
