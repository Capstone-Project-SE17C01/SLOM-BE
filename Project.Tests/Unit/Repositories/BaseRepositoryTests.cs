using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.General;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using Project.Tests.Helpers;

namespace Project.Tests.Unit.Repositories {
    public class BaseRepositoryTests {
        [Test]
        public async Task GetAll_ShouldReturnAllEntities() {
            var data = TestDataFixture.Courses.AsQueryable();
            var mockSet = TestMockHelper.CreateMockDbSet(data);
            Mock<ApplicationDbContext> mockContext = TestMockHelper.CreateMockDbContext(mockSet);

            var repo = new BaseRepository<Course>(mockContext.Object);

            var result = await repo.GetAll();

            Assert.That(result.Count(), Is.EqualTo(data.Count()));
        }

        [Test]
        public async Task Create_ShouldAddEntity() {
            var mockSet = new Mock<DbSet<Course>>();
            Mock<ApplicationDbContext> mockContext = TestMockHelper.CreateMockDbContext(mockSet);

            var repo = new BaseRepository<Course>(mockContext.Object);
            var course = TestDataFixture.SingleCourse;

            var result = await repo.Create(course);

            mockSet.Verify(m => m.AddAsync(course, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldRemoveEntity() {
            var mockSet = new Mock<DbSet<Course>>();
            Mock<ApplicationDbContext> mockContext = TestMockHelper.CreateMockDbContext(mockSet);

            var repo = new BaseRepository<Course>(mockContext.Object);
            var course = TestDataFixture.SingleCourse;

            await repo.Delete(course);

            mockSet.Verify(m => m.Remove(course), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
