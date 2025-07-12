using Project.Core.Entities.General;

namespace Project.Tests.TestData.SeedData {
    public static class CourseSeed {
        public static List<Course> GetCourses() {
            return new List<Course>
            {
                new Course
                {
                    Id = Guid.NewGuid(),
                    Title = "Khóa học Nhập môn ASL",
                    Description = "Khóa học cơ bản về ngôn ngữ ký hiệu ASL.",
                    DifficultyLevel = "Beginner",
                    ThumbnailUrl = null,
                    LanguageId = null,
                    CategoryId = null,
                    CreatorId = null,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    Title = "Khóa học Nâng cao ASL",
                    Description = "Khóa học nâng cao về ngôn ngữ ký hiệu ASL.",
                    DifficultyLevel = "Advanced",
                    ThumbnailUrl = null,
                    LanguageId = null,
                    CategoryId = null,
                    CreatorId = null,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static Course GetSingleCourse() {
            return new Course {
                Id = Guid.NewGuid(),
                Title = "Khóa học Đặc biệt",
                Description = "Khóa học dành cho kiểm thử.",
                DifficultyLevel = "Special",
                ThumbnailUrl = null,
                LanguageId = null,
                CategoryId = null,
                CreatorId = null,
                IsPublished = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
