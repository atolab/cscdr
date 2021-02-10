//
// Copyright (c) 2021 ADLINK Technology Inc.
//
// This program and the accompanying materials are made available under the
// terms of the Eclipse Public License 2.0 which is available at
// http://www.eclipse.org/legal/epl-2.0, or the Apache License, Version 2.0
// which is available at https://www.apache.org/licenses/LICENSE-2.0.
//
// SPDX-License-Identifier: EPL-2.0 OR Apache-2.0
//
using System;
using Xunit;
using CSCDR;

namespace CSCDR.Tests
{
    public class CDRWriterTests
    {
        [Fact]
        public void CDRHeader()
        {
            var val = BitConverter.IsLittleEndian
                ? new byte[] { 0x00, 0x01, 0x00, 0x00 }
                : new byte[] { 0x00, 0x00, 0x00, 0x00 };

            CDRWriter writer = new CDRWriter();
            Assert.Equal(val, writer.GetBuffer().ToArray());
        }
    }

        public class BasicTypesWriteTests
        {
            [Theory]
            [InlineData(0x00)]
            [InlineData(0x0A)]
            [InlineData(0xFF)]
            public void WriteByte(byte val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteByte(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadByte());
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void WriteBool(bool val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteBool(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadBool());
            }

            [Theory]
            [InlineData(0x0A0B)]
            [InlineData(Int16.MinValue)]
            [InlineData(Int16.MaxValue)]
            public void WriteInt16(short val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteInt16(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadInt16());
            }

            [Theory]
            [InlineData(0x0A0B)]
            [InlineData(UInt16.MinValue)]
            [InlineData(UInt16.MaxValue)]
            public void WriteUInt16(ushort val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteUInt16(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadUInt16());
            }

            [Theory]
            [InlineData(0x0A0B0C0D)]
            [InlineData(Int32.MinValue)]
            [InlineData(Int32.MaxValue)]
            public void WriteInt32(int val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteInt32(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadInt32());
            }

            [Theory]
            [InlineData(0x0A0B0C0D)]
            [InlineData(UInt32.MinValue)]
            [InlineData(UInt32.MaxValue)]
            public void WriteUInt32(uint val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteUInt32(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadUInt32());
            }

            [Theory]
            [InlineData(0x0A0B0C0D0E0F0102)]
            [InlineData(Int64.MinValue)]
            [InlineData(Int64.MaxValue)]
            public void WriteInt64(long val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteInt64(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadInt64());
            }

            [Theory]
            [InlineData(0x0A0B0C0D0E0F0102)]
            [InlineData(UInt64.MinValue)]
            [InlineData(UInt64.MaxValue)]
            public void WriteUInt64(ulong val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteUInt64(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadUInt64());
            }

            [Theory]
            [InlineData(1.0f)]
            [InlineData(0.01f)]
            [InlineData(Single.MinValue)]
            [InlineData(Single.MaxValue)]
            public void WriteSingle(float val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteSingle(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadSingle());
            }

            [Theory]
            [InlineData(1.0)]
            [InlineData(0.01)]
            [InlineData(Double.MinValue)]
            [InlineData(Double.MaxValue)]
            public void WriteDouble(double val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteDouble(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadDouble());
            }

            [Theory]
            [InlineData('A')]
            [InlineData('a')]
            [InlineData(' ')]
            [InlineData(';')]
            public void WriteChar(char val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteChar(val);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadChar());
            }

            [Theory]
            [InlineData("Hello World!")]
            public void WriteString(string val)
            {
                CDRWriter writer = new CDRWriter();
                writer.WriteString(val);
                Assert.Equal(4+ 4+ val.Length +1, writer.GetBuffer().Length);
                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(val, reader.ReadString());
            }

        }
/*
        public class WriteAlignmentTests
        {
            [Fact]
            public void BasicTypesAlignment()
            {
                Console.WriteLine("******* BasicTypesAlignment");

                var buf = new byte[] {
                    0x00, 0x00, 0x00, 0x00,                          // Header
                    0xFF,                                            // 1 byte to force padding
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,        // padding for 8-bytes alignment
                    0x0A, 0x0B, 0x0C, 0X0D, 0x0E, 0x0F, 0x01, 0X02,  // uint64
                    0xFF,                                            // 1 byte to force padding
                    0x00, 0x00, 0x00,                                // padding for 4-bytes alignment
                    0x0A, 0x0B, 0x0C, 0X0D,                          // uint32
                    0xFF,                                            // 1 byte to force padding
                    0x00,                                            // padding for 2-bytes alignment
                    0x0A, 0x0B,                                      // uint16
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF,                    // 5 bytes to force padding
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,        // padding for 8-bytes alignment
                    0x3F, 0x84, 0x7A, 0xE1, 0x47, 0xAE, 0x14, 0x7B,  // double
                    0xFF,                                            // 1 byte to force padding
                    0x00, 0x00, 0x00,                                // padding for 4-bytes alignment
                    0x3C, 0x23, 0xD7, 0x0A,                          // float
                    0xFF,                                            // 1 byte to force padding
                    0x00, 0x00, 0x00,                                // padding for 4-bytes alignment
                    0x00, 0x00, 0x00, 0x0D,                          // "Hello World!"
                    0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20,
                    0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0x00
                };

                CDRReader reader = new CDRReader(writer.GetBuffer().ToArray());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0x0A0B0C0D0E0F0102UL, reader.ReadUInt64());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0x0A0B0C0DU, reader.ReadUInt32());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0x0A0B, reader.ReadUInt16());

                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0.01, reader.ReadDouble());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal(0.01f, reader.ReadSingle());
                Assert.Equal(0xFF, reader.ReadByte());
                Assert.Equal("Hello World!", reader.ReadString());
            }

        }
        */
}
