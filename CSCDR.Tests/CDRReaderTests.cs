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
    public class CDRReaderTests
    {
        [Fact]
        public void CDRLEHeader()
        {
            var buf = new byte[] {
                    0x00, 0x01, 0x0A, 0x0B
                };
            CDRReader reader = new CDRReader(buf);

            Assert.Equal(RepresentationId.CDR_LE, reader.ReprId);
            Assert.Equal(0x0A0B, reader.ReprOpt);
            Assert.True(reader.IsLittleEndian);
        }

        [Fact]
        public void CDRBEHeader()
        {
            var buf = new byte[] {
                    0x00, 0x00, 0x0C, 0x0D
                };
            CDRReader reader = new CDRReader(buf);

            Assert.Equal(RepresentationId.CDR_BE, reader.ReprId);
            Assert.Equal(0x0C0D, reader.ReprOpt);
            Assert.False(reader.IsLittleEndian);
        }

        [Fact]
        public void UnsupportedHeader()
        {
            var buf = new byte[] {
                    0x00, 0x02, 0x0C, 0x0D
                };
            try
            {
                CDRReader reader = new CDRReader(buf);
                Assert.True(false, "UnsupportedCDRRepresentation was expected");
            }
            catch (UnsupportedCDRRepresentation)
            {
                Assert.True(true);
            }
        }

    }

    public class BasicTypesReadTests
    {
        [Theory]
        [InlineData(0x00, new byte[] { 0x00, 0x00, 0x00, 0x00, 0X00 })]
        [InlineData(0x0A, new byte[] { 0x00, 0x00, 0x00, 0x00, 0X0A })]
        [InlineData(0xFF, new byte[] { 0x00, 0x00, 0x00, 0x00, 0XFF })]
        [InlineData(0x00, new byte[] { 0x00, 0x01, 0x00, 0x00, 0X00 })]
        [InlineData(0x0A, new byte[] { 0x00, 0x01, 0x00, 0x00, 0X0A })]
        [InlineData(0xFF, new byte[] { 0x00, 0x01, 0x00, 0x00, 0XFF })]
        public void Byte(byte expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadByte());
        }

        [Theory]
        [InlineData(false, new byte[] { 0x00, 0x00, 0x00, 0x00, 0X00 })]
        [InlineData(true, new byte[] { 0x00, 0x00, 0x00, 0x00, 0X01 })]
        [InlineData(true, new byte[] { 0x00, 0x00, 0x00, 0x00, 0XFF })]
        [InlineData(false, new byte[] { 0x00, 0x01, 0x00, 0x00, 0X00 })]
        [InlineData(true, new byte[] { 0x00, 0x01, 0x00, 0x00, 0X01 })]
        [InlineData(true, new byte[] { 0x00, 0x01, 0x00, 0x00, 0XFF })]
        public void Bool(bool expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadBool());
        }

        [Theory]
        [InlineData(0x0A0B, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B })]
        [InlineData(Int16.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x80, 0X00 })]
        [InlineData(Int16.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7F, 0XFF })]
        [InlineData(0x0A0B, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0B, 0x0A })]
        [InlineData(Int16.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x80 })]
        [InlineData(Int16.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0x7F })]
        public void ReadInt16(short expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadInt16());
        }

        [Theory]
        [InlineData(0x0A0B, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B })]
        [InlineData(UInt16.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0X00 })]
        [InlineData(UInt16.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0XFF })]
        [InlineData(0x0A0B, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0B, 0x0A })]
        [InlineData(UInt16.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(UInt16.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF })]
        public void ReadUInt16(ushort expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadUInt16());
        }

        [Theory]
        [InlineData(0x0A0B0C0D, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B, 0x0C, 0x0D })]
        [InlineData(Int32.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x80, 0X00, 0X00, 0X00 })]
        [InlineData(Int32.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7F, 0XFF, 0XFF, 0XFF })]
        [InlineData(0x0A0B0C0D, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0D, 0x0C, 0x0B, 0x0A })]
        [InlineData(Int32.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 })]
        [InlineData(Int32.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x7F })]
        public void ReadInt32(int expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadInt32());
        }

        [Theory]
        [InlineData(0x0A0B0C0D, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B, 0x0C, 0x0D })]
        [InlineData(UInt32.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0X00, 0x00, 0x00 })]
        [InlineData(UInt32.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0XFF, 0XFF, 0XFF })]
        [InlineData(0x0A0B0C0D, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0D, 0x0C, 0x0B, 0x0A })]
        [InlineData(UInt32.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(UInt32.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0XFF, 0XFF })]
        public void ReadUInt32(uint expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadUInt32());
        }

        [Theory]
        [InlineData(0x0A0B0C0D0E0F0102, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B, 0x0C, 0x0D, 0x0E, 0X0F, 0x01, 0x02 })]
        [InlineData(Int64.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x80, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00 })]
        [InlineData(Int64.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7F, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF })]
        [InlineData(0x0A0B0C0D0E0F0102, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x02, 0X01, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A })]
        [InlineData(Int64.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0X00, 0X00, 0X00, 0X00, 0x00, 0x00, 0x80 })]
        [InlineData(Int64.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0XFF, 0XFF, 0XFF, 0XFF, 0xFF, 0x7F })]
        public void ReadInt64(long expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadInt64());
        }

        [Theory]
        [InlineData(0x0A0B0C0D0E0F0102, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0A, 0X0B, 0x0C, 0x0D, 0x0E, 0X0F, 0x01, 0x02 })]
        [InlineData(UInt64.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(UInt64.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF })]
        [InlineData(0x0A0B0C0D0E0F0102, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x02, 0X01, 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A })]
        [InlineData(UInt64.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(UInt64.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF })]
        public void ReadUInt64(ulong expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadUInt64());
        }

        [Theory]
        [InlineData(1.0f, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 })]
        [InlineData(0.01f, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3C, 0x23, 0xD7, 0x0A })]
        [InlineData(Single.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7F, 0xFF, 0xFF })]
        [InlineData(Single.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0xFF, 0xFF })]
        [InlineData(1.0f, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F })]
        [InlineData(0.01f, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0A, 0xD7, 0x23, 0x3C })]
        [InlineData(Single.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xFF })]
        [InlineData(Single.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0x7F })]
        public void ReadSingle(float expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadSingle());
        }

        [Theory]
        [InlineData(1.0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3F, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(0.01, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x3F, 0x84, 0x7A, 0xE1, 0x47, 0xAE, 0x14, 0x7B })]
        [InlineData(Double.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        [InlineData(Double.MaxValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        [InlineData(1.0, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F })]
        [InlineData(0.01, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x7B, 0x14, 0xAE, 0x47, 0xE1, 0x7A, 0x84, 0x3F })]
        [InlineData(Double.MinValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF, })]
        [InlineData(Double.MaxValue, new byte[] { 0x00, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F, })]
        public void ReadDouble(double expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadDouble());
        }

        [Theory]
        [InlineData('A', new byte[] { 0x00, 0x00, 0x00, 0x00, 0X41 })]
        [InlineData('a', new byte[] { 0x00, 0x00, 0x00, 0x00, 0X61 })]
        [InlineData(' ', new byte[] { 0x00, 0x00, 0x00, 0x00, 0X20 })]
        [InlineData(';', new byte[] { 0x00, 0x00, 0x00, 0x00, 0X3B })]
        public void ReadChar(char expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadChar());
        }

        [Theory]
        [InlineData("Hello World!", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D,
                0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0x00 })]
        [InlineData("Hello World!", new byte[] { 0x00, 0x01, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00,
                0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x21, 0x00 })]
        public void ReadString(string expected, byte[] buf)
        {
            CDRReader reader = new CDRReader(buf);
            Assert.Equal(expected, reader.ReadString());
        }

    }

    public class ReadAlignmentTests
    {
        [Fact]
        public void BasicTypesAlignment()
        {
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

            CDRReader reader = new CDRReader(buf);
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
}
