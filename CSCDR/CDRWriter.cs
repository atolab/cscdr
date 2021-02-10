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
using System.IO;
using System.Text;

namespace CSCDR
{
    /// <summary>A class encoding basic DDS types as CDR into a in-memory buffer.</summary>
    public class CDRWriter
    {
        private MemoryStream _stream;
        private BinaryWriter _writer;

        /// <summary>Initializes a new instance with an expandable capacity initialized to zero.
        /// The 4-bytes DDS SerializedPayloadHeader (that includes the RepresentationIdentifier
        /// and the RepresentationOptions) is automatically written into the buffer by this constructor.
        /// </summary>
        public CDRWriter() : this(new MemoryStream()) { }

        /// <summary>Initializes a new non-resizable instance based on the specified byte array.
        /// The 4-bytes DDS SerializedPayloadHeader (that includes the RepresentationIdentifier
        /// and the RepresentationOptions) is automatically written into the buffer by this constructor.
        /// </summary>
        public CDRWriter(byte[] buffer) : this(new MemoryStream(buffer)) { }

        private CDRWriter(MemoryStream stream)
        {
            _stream = stream;
            _writer = new BinaryWriter(_stream);

            // Note: currently only CDR_LE and CDR_BE without representation options are supported
            if (BitConverter.IsLittleEndian)
            {
                _writer.Write((byte)0x00);
                _writer.Write((byte)0x01);
                _writer.Write((byte)0x00);
                _writer.Write((byte)0x00);
            }
            else
            {
                _writer.Write((byte)0x00);
                _writer.Write((byte)0x00);
                _writer.Write((byte)0x00);
                _writer.Write((byte)0x00);
            }
        }

        public ReadOnlySpan<Byte> GetBuffer()
        {
            return new ReadOnlySpan<Byte>(_stream.GetBuffer(), 0, (int)_stream.Length);
        }

        public void WriteByte(byte b) => _writer.Write(b);

        public void WriteBytes(ReadOnlySpan<byte> buf) => _writer.Write(buf);

        private void Align(int alignment)
        {
            // Note: The 4 starting bytes (for Representation Id and Options) are not considered for alignment
            var modulo = (_writer.BaseStream.Position + 4) % alignment;
            if (modulo > 0)
            {
                Console.WriteLine("***** align to {0}  pos={1} => modulo={2} => add {3}", alignment, _writer.BaseStream.Position, modulo, alignment - modulo);
                for (int i = 0; i < alignment - modulo; i++)
                {
                    WriteByte(0x00);
                }
            }
        }

        public void WriteBool(bool b) => WriteByte(b ? (byte)0x01 : (byte)0x00);

        public void WriteInt16(short s)
        {
            Align(2);
            _writer.Write(s);
        }

        public void WriteUInt16(ushort s)
        {
            Align(2);
            _writer.Write(s);
        }

        public void WriteInt32(int i)
        {
            Align(4);
            _writer.Write(i);
        }

        public void WriteUInt32(uint i)
        {
            Align(4);
            _writer.Write(i);
        }

        public void WriteInt64(long l)
        {
            Align(8);
            _writer.Write(l);
        }

        public void WriteUInt64(ulong l)
        {
            Align(8);
            _writer.Write(l);
        }

        public void WriteSingle(float f)
        {
            Align(4);
            _writer.Write(f);
        }

        public void WriteDouble(double d)
        {
            Align(8);
            _writer.Write(d);
        }

        public void WriteChar(char c) => _writer.Write(Convert.ToByte(c));

        public void WriteString(String s)
        {
            // Note: Add null-termination char as not present in C# String
            WriteUInt32((uint)s.Length + 1);
            WriteBytes(Encoding.UTF8.GetBytes(s));
            WriteByte(0x00);
        }

        public void WriteEnum(uint enumVal) => WriteUInt32(enumVal);

        public void WriteSequenceLength(uint length) => WriteUInt32(length);
    }
}