using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using Project.Tests.Helpers;
using Xunit;

namespace Project.Tests.Unit.Repositories {
    public class BaseRepositoryTests {
        [Fact]
        public async Task GetAll_ShouldReturnAllEntities() {
            // Arrange
            using var context = TestMockHelper.CreateInMemoryDbContext("GetAllTest");
            var repo = new BaseRepository<Course>(context);

            // Add test data
            var courses = TestDataFixture.Courses;
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.GetAll();

            // Assert
            result.Should().HaveCount(courses.Count);
        }

        [Fact]
        public async Task Create_ShouldAddEntity() {
            // Arrange
            using var context = TestMockHelper.CreateInMemoryDbContext("CreateTest");
            var repo = new BaseRepository<Course>(context);
            var course = TestDataFixture.SingleCourse;

            // Act
            var result = await repo.Create(course);
            await context.SaveChangesAsync();

            // Assert
            var savedCourse = await context.Courses.FindAsync(course.Id);
            savedCourse.Should().NotBeNull();
            savedCourse!.Title.Should().Be(course.Title);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity() {
            // Arrange
            using var context = TestMockHelper.CreateInMemoryDbContext("DeleteTest");
            var repo = new BaseRepository<Course>(context);
            var course = TestDataFixture.SingleCourse;

            // Add course first
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            // Act
            await repo.Delete(course);
            await context.SaveChangesAsync();

            // Assert
            var deletedCourse = await context.Courses.FindAsync(course.Id);
            deletedCourse.Should().BeNull();
        }
    }
}
