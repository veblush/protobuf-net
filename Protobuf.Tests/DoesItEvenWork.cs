using System;
using ProtoBuf;
using System.Linq;
using ProtoBuf.Meta;
using Xunit;
using System.Reflection;
using System.IO;

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
    public static bool IsGenericTypeDefinition(this Type type)
    {
#if DNXCORE50
        return type.GetTypeInfo().IsGenericTypeDefinition;
#else
        return type.IsGenericTypeDefinition;
#endif
    }
    public static bool IsGenericType(this Type type)
    {
#if DNXCORE50
        return type.GetTypeInfo().IsGenericType;
#else
        return type.IsGenericType;
#endif
    }
#if DNXCORE50
    public static Type[] GetGenericArguments(this Type type)
    {
        return type.GetTypeInfo().GenericTypeArguments;
    }
#endif
    public static Assembly Assembly(this Type type)
    {
#if DNXCORE50
        return type.GetTypeInfo().Assembly;
#else
        return type.Assembly;
#endif
    }
    public static Type BaseType(this Type type)
    {

#if DNXCORE50
        return type.GetTypeInfo().BaseType;
#else
        return type.BaseType;
#endif
    }

#if DNXCORE50
    public static bool IsDefined(this Type type, Type attributeType, bool inherit)
    {
        return type.GetTypeInfo().IsDefined(attributeType);
    }
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
    public static Type[] GetInterfaces(this Type type)
    {
        return type.GetTypeInfo().ImplementedInterfaces.ToArray();
    }
#endif
}
public static class Helpers
{
    public static TypeModel CompileIntoTestFolder(this RuntimeTypeModel model, string name, string path)
    {
#if DNXCORE50
        return Compile(model);
#else
        var final = System.IO.Path.Combine("TestFiles", path);
        var result = model.Compile(name, path);
        if (File.Exists(final)) File.Delete(final);
        File.Move(path, final);
        return result;
#endif
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
