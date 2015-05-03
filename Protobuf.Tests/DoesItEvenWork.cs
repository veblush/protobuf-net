using System;
using ProtoBuf;
using ProtoBuf.Meta;
using Xunit;

sealed class TestFixtureAttribute : Attribute { }
#if DNXCORE50
namespace System
{
    public sealed class SerializableAttribute : Attribute { }
}
namespace System.Runtime.Serialization
{
    public class DataContractAttribute : Attribute { }

    public class DataMemberAttribute : Attribute
    {
        public bool EmitDefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
public static class Helpers
{
    public static TypeModel Compile(this RuntimeTypeModel model, string x, string y)
    {
        return Compile(model);
    }
    public static TypeModel Compile(this RuntimeTypeModel model)
    {
        model.CompileInPlace();
        return model;
    }
}
#endif
namespace Protobuf.Tests
{

    public class DoesItEvenWork
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
