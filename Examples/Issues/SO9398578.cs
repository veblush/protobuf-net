using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using ProtoBuf;

namespace Examples.Issues
{
    
    public class SO9398578
    {
        [Fact]
        public void TestRandomDataWithString()
        {
            Assert.Throws<ProtoException>(() =>
            {
                var input = File.ReadAllBytes("protobuf-net.dll");
                var stream = new MemoryStream(input);
                stream.Seek(0, SeekOrigin.Begin);
                Assert.True(stream.Length > 0);
                Serializer.Deserialize<string>(stream);
            });
        }
        [Fact]
        public void TestRandomDataWithContractType()
        {
            Assert.Throws<ProtoException>(() =>
            {
                var input = File.ReadAllBytes("protobuf-net.dll");
                var stream = new MemoryStream(input);
                stream.Seek(0, SeekOrigin.Begin);
                Assert.True(stream.Length > 0);
                Serializer.Deserialize<Foo>(stream);
            });
        }
        [Fact]
        public void TestRandomDataWithReader()
        {
            Assert.Throws<ProtoException>(() =>
            {
                var input = File.ReadAllBytes("protobuf-net.dll");
                var stream = new MemoryStream(input);
                stream.Seek(0, SeekOrigin.Begin);
                Assert.True(stream.Length > 0);

                using (var reader = new ProtoReader(stream, null, null))
                {
                    while (reader.ReadFieldHeader() > 0)
                    {
                        reader.SkipField();
                    }
                }
            });
        }

        [ProtoContract]
        public class Foo
        {
        }
    }
}
