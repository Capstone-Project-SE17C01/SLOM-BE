using FluentAssertions;
using Moq;
using Project.Core.Mapper;
using Project.Tests.Helpers;
using Xunit;

namespace Project.Tests.Unit.Mappers {
    public class BaseMapperTests {
        public class Source { public int Id { get; set; } }
        public class Destination { public int Id { get; set; } }

        [Fact]
        public void Map_ShouldReturnMappedObject() {
            // Arrange
            var source = new Source { Id = 10 };
            var dest = new Destination { Id = 10 };
            var mockMapper = TestMockHelper.CreateMockMapper<Source, Destination>(dest);
            var baseMapper = new BaseMapper<Source, Destination>(mockMapper.Object);

            // Act
            var result = baseMapper.MapModel(source);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(10);
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenSourceIsNull() {
            // Arrange
            var mockMapper = TestMockHelper.CreateMockMapper<Source, Destination>(null);
            var baseMapper = new BaseMapper<Source, Destination>(mockMapper.Object);

            // Act
            var result = baseMapper.MapModel(null);

            // Assert
            result.Should().BeNull();
        }
    }
}
