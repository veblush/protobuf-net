using System;
using ProtoBuf;
using System.Linq;
using ProtoBuf.Meta;
using Xunit;
using System.Reflection;
sealed class TestFixtureAttribute : Attribute { }

[Obsolete]
sealed class ExpectedException : Attribute
{
    public ExpectedException(Type type) { }
    public ExpectedException(string type) { }

    public string ExpectedMessage { get; set; }
}

#if DNXCORE50
namespace System
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class NonSerializedAttribute : Attribute
    {

    }
    public sealed class SerializableAttribute : Attribute { }
}
namespace System.ComponentModel
{
    public sealed class BrowsableAttribute : Attribute { public BrowsableAttribute(bool value) { } }
}
#endif
static class FrameworkHelpers
{
    public static bool IsValueType(this Type type)
    {
#if DNXCORE50
        return type.GetTypeInfo().IsValueType;
#else
        return type.IsValueType;
#endif
    }
#if DNXCORE50
    public static PropertyInfo GetProperty(this Type type, string name)
    {
        return type.GetRuntimeProperty(name);
    }
    public static MethodInfo GetMethod(this Type type, string name)
    {
        return type.GetRuntimeMethods().SingleOrDefault(x => x.Name == name);
    }
    public static PropertyInfo[] GetProperties(this Type type)
    {
        return type.GetRuntimeProperties().ToArray();
    }
#endif
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
