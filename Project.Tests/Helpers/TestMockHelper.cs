using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace Project.Tests.Helpers {
    public static class TestMockHelper {
        public static ApplicationDbContext CreateInMemoryDbContext(string databaseName = "TestDatabase") {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Setup async operations
            mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync((T entity, CancellationToken token) => {
                       var entry = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>>();
                       entry.Setup(e => e.Entity).Returns(entity);
                       return entry.Object;
                   });

            // Setup ToListAsync
            mockSet.Setup(m => m.ToListAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(data.ToList());

            // Setup AsNoTracking
            mockSet.Setup(m => m.AsNoTracking()).Returns(mockSet.Object);

            return mockSet;
        }

        public static Mock<Project.Infrastructure.Data.ApplicationDbContext> CreateMockDbContext<T>(Mock<DbSet<T>> mockSet) where T : class {
            var mockContext = new Mock<Project.Infrastructure.Data.ApplicationDbContext>();
            mockContext.Setup(c => c.Set<T>()).Returns(mockSet.Object);
            return mockContext;
        }

        public static Mock<IMapper> CreateMockMapper<TSource, TDestination>(TDestination result) {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<TDestination>(It.IsAny<TSource>())).Returns(result);
            return mockMapper;
        }
    }
}
