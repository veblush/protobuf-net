using Xunit;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples
{
    
    public class NonPublic_Compile
    {
        private static void Compile<T>()
        {
            var model = TypeModel.Create();
            model.Add(typeof(T), true);
            string name = typeof(T).Name + "Serializer", path = name + ".dll";
            model.CompileIntoTestFolder(name, path);
            PEVerify.AssertValid(path);
            Assert.Equal("##fail##", "Should have failed");
        }
        [ProtoContract]
        private class PrivateType
        {
        }
        [Fact]
        public void PrivateTypeShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateType>();
            }).Message;
            Assert.Equal("Non-public type cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateType", msg);
        }
        private class NonPublicWrapper
        {
            [ProtoContract]
            internal class IndirectlyPrivateType
            {
            }
        }
        [Fact]
        public void IndirectlyPrivateTypeShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<NonPublicWrapper.IndirectlyPrivateType>();
            }).Message;
            Assert.Equal("Non-public type cannot be used with full dll compilation: Examples.NonPublic_Compile+NonPublicWrapper+IndirectlyPrivateType", msg);
        }
        [ProtoContract]
        public class PrivateCallback
        {
            [ProtoBeforeSerialization]
            private void OnDeserialize() { }
        }
        [Fact]
        public void PrivateCallbackShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateCallback>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateCallback.OnDeserialize", msg);
        }

        [ProtoContract]
        public class PrivateField
        {
#pragma warning disable 0169
            [ProtoMember(1)]
            private int Foo;
#pragma warning restore 0169
        }
        [Fact]
        public void PrivateFieldShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateField>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateField.Foo", msg);
        }
        [ProtoContract]
        public class PrivateProperty
        {
            [ProtoMember(1)]
            private int Foo { get; set; }
        }
        [Fact]
        public void PrivatePropertyShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateProperty>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateProperty.get_Foo", msg);
        }
        [ProtoContract]
        public class PrivatePropertyGet
        {
            [ProtoMember(1)]
            public int Foo { private get; set; }
        }
        [Fact]
        public void PrivatePropertyGetShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivatePropertyGet>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivatePropertyGet.get_Foo", msg);
        }
        [ProtoContract]
        public class PrivatePropertySet
        {
            [ProtoMember(1)]
            public int Foo { get; private set; }
        }
        [Fact]
        public void PrivatePropertySetShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivatePropertySet>();
            }).Message;
            Assert.Equal("Cannot apply changes to property Examples.NonPublic_Compile+PrivatePropertySet.Foo", msg);
        }
        [ProtoContract]
        public class PrivateConditional
        {
            [ProtoMember(1)]
            public int Foo { get; set; }

            private bool ShouldSerializeFoo() { return true; }
        }
        [Fact]
        public void PrivateConditionalSetShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateConditional>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateConditional.ShouldSerializeFoo", msg);
        }
        [ProtoContract]
        public class PrivateConstructor
        {
            private PrivateConstructor() { }
            [ProtoMember(1)]
            public int Foo { get; set; }
        }
        [Fact]
        public void PrivateConstructorShouldFail()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                Compile<PrivateConstructor>();
            }).Message;
            Assert.Equal("Non-public member cannot be used with full dll compilation: Examples.NonPublic_Compile+PrivateConstructor..ctor", msg);
        }
    }
}
