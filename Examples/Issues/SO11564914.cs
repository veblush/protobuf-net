using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using ProtoBuf;

namespace Examples.Issues
{
    
    public class SO11564914
    {
        [Fact]
        public void SerializeFromProtobufCSharpPortShouldGiveUsefulMessage()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                var obj = new BlockHeader();
                Serializer.Serialize(Stream.Null, obj);
            }).Message;
            Assert.Equal("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: Examples.Issues.SO11564914+BlockHeader", msg);
        }
        [Fact]
        public void DeserializeFromProtobufCSharpPortShouldGiveUsefulMessage()
        {
            var msg = Assert.Throws<InvalidOperationException>(() =>
            {
                var obj = new BlockHeader();
                Serializer.Deserialize<BlockHeader>(Stream.Null);
            }).Message;
            Assert.Equal("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: Examples.Issues.SO11564914+BlockHeader", msg);
        }

        public sealed partial class BlockHeader : GeneratedMessage<BlockHeader, BlockHeader.Builder>
        {
            // yada yada yada
            public class Builder
            {
                
            }
        }

        public class GeneratedMessage<TFoo, TBar>
        {
        }
        
    }
}
