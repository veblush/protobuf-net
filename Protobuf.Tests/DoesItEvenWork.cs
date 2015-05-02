using ProtoBuf;
using Xunit;

namespace Protobuf.Tests
{
    public class Class1
    {
        [Fact]
        public void Test()
        {
            var obj = new MyDto { Value = 42 };
            var clone = Serializer.DeepClone(obj);
            Assert.Equal(42, clone.Value);
        }
        [ProtoContract]
        public class MyDto
        {
            [ProtoMember(1)]
            public int Value { get; set; }
        }
    }
}
