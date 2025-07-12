using Project.Core.Mapper;
using Project.Tests.Helpers;

namespace Project.Tests.Unit.Mappers {
    public class BaseMapperTests {
        public class Source { public int Id { get; set; } }
        public class Destination { public int Id { get; set; } }

        [Test]
        public void Map_ShouldReturnMappedObject() {
            var source = new Source { Id = 10 };
            var dest = new Destination { Id = 10 };
            var mockMapper = TestMockHelper.CreateMockMapper<Source, Destination>(dest);

            var baseMapper = new BaseMapper<Source, Destination>(mockMapper.Object);

            var result = baseMapper.MapModel(source);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(10));
        }

        [Test]
        public void Map_ShouldReturnNull_WhenSourceIsNull() {
            var mockMapper = TestMockHelper.CreateMockMapper<Source, Destination>(null);

            var baseMapper = new BaseMapper<Source, Destination>(mockMapper.Object);

            var result = baseMapper.MapModel(null);

            Assert.That(result, Is.Null);
        }
    }
}
