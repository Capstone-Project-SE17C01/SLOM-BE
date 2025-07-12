using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Project.Tests.Helpers
{
    public static class TestMockHelper
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        public static Mock<Project.Infrastructure.Data.ApplicationDbContext> CreateMockDbContext<T>(Mock<DbSet<T>> mockSet) where T : class
        {
            var mockContext = new Mock<Project.Infrastructure.Data.ApplicationDbContext>();
            mockContext.Setup(c => c.Set<T>()).Returns(mockSet.Object);
            return mockContext;
        }

        public static Mock<IMapper> CreateMockMapper<TSource, TDestination>(TDestination result)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<TDestination>(It.IsAny<TSource>())).Returns(result);
            return mockMapper;
        }
    }
}
