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
using System.Buffers.Binary;
using System.Text;

namespace CSCDR
{
    /// <summary>A class decoding basic DDS types encoded as CDR from a bytes array.</summary>
    public class CDRReader
    {
        public readonly RepresentationId ReprId;
        public readonly ushort ReprOpt;
        public readonly bool IsLittleEndian;

        private readonly byte[] _buf;
        private int _position;

        /// <summary>Initializes a new instance decoding types from the specified byte array.
        /// The 4-bytes DDS SerializedPayloadHeader (that includes the RepresentationIdentifier
        /// and the RepresentationOptions) is automatically read from the byte array by this constructor.
        /// </summary>
        public CDRReader(byte[] buf)
        {
            ushort rid = BinaryPrimitives.ReadUInt16BigEndian(buf);
            if (!Enum.IsDefined(typeof(RepresentationId), (int)rid))
            {
                throw new UnsupportedCDRRepresentation(rid);
            }
            this.ReprId = (RepresentationId)rid;

            // NOTE: we currently only support CDR_BE and CDR_LE
            if (this.ReprId != RepresentationId.CDR_BE && this.ReprId != RepresentationId.CDR_LE)
            {
                throw new UnsupportedCDRRepresentation(rid);
            }
            this.ReprOpt = BinaryPrimitives.ReadUInt16BigEndian(new ReadOnlySpan<Byte>(buf, 2, 2));
            this.IsLittleEndian = (rid & 0x0001) == 0x0001;

            this._buf = buf;
            this._position = 4;
        }

        public byte ReadByte()
        {
            return _buf[_position++];
        }

        public ReadOnlySpan<Byte> ReadBytes(int count)
        {
            var result = new ReadOnlySpan<Byte>(_buf, _position, count);
            _position += count;
            return result;
        }

        private void Align(int alignment)
        {
            // Note: The 4 starting bytes (for Representation Id and Options) are not considered for alignment
            var modulo = (_position + 4) % alignment;
            if (modulo > 0)
            {
                ReadBytes(alignment - modulo);
            }
        }

        public bool ReadBool() => ReadByte() != 0x00;

        public short ReadInt16()
        {
            Align(2);
            return IsLittleEndian
                ? BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(2))
                : BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
        }

        public ushort ReadUInt16()
        {
            Align(2);
            return IsLittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(2))
                : BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2));
        }

        public int ReadInt32()
        {
            Align(4);
            return IsLittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(ReadBytes(4))
                : BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
        }

        public uint ReadUInt32()
        {
            Align(4);
            return IsLittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(ReadBytes(4))
                : BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(4));
        }

        public long ReadInt64()
        {
            Align(8);
            return IsLittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(8))
                : BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));
        }

        public ulong ReadUInt64()
        {
            Align(8);
            return IsLittleEndian
                ? BinaryPrimitives.ReadUInt64LittleEndian(ReadBytes(8))
                : BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(8));
        }

        public float ReadSingle()
        {
            Align(4);
            return IsLittleEndian
                ? BinaryPrimitives.ReadSingleLittleEndian(ReadBytes(4))
                : BinaryPrimitives.ReadSingleBigEndian(ReadBytes(4));
        }

        public double ReadDouble()
        {
            Align(8);
            return IsLittleEndian
                ? BinaryPrimitives.ReadDoubleLittleEndian(ReadBytes(8))
                : BinaryPrimitives.ReadDoubleBigEndian(ReadBytes(8));
        }

        public char ReadChar() => Convert.ToChar(ReadByte());

        public String ReadString()
        {
            var len = ReadUInt32();
            // Note: skip null-termination char to construct C# String
            var strBuf = ReadBytes((int)len - 1);
            ReadByte();
            return Encoding.UTF8.GetString(strBuf);
        }

        public uint ReadEnum() => ReadUInt32();

        public uint ReadSequenceLength() => ReadUInt32();
    }


}