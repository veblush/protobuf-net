﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoBuf;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

namespace Examples.Issues
{
    [TestFixture]
    public class Issue174cs
    {
        [Test]
        public void TestDynamic()
        {
            Program.ExpectFailure<Exception>(() =>
            {
                var myVal = new TestProto { Value = true };
                byte[] serialized;
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, myVal);
                    serialized = ms.ToArray();
                }
                Assert.IsNotNull(serialized);
            }, "Dynamic type is not a contract-type: Boolean");
        }

        [ProtoContract]
        public class TestProto
        {
            [ProtoMember(1, DynamicType = true)]
            public object Value { get; internal set; }
        }
    }
}
