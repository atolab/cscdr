using System;

namespace CSCDR
{
    public enum RepresentationId
    {
        CDR_BE = 0x0000,
        CDR_LE = 0x0001,
        PL_CDR_LE = 0x0002,
        PL_CDR_BE = 0x0003,
        CDR2_BE = 0x0010,
        CDR2_LE = 0x0011,
        PL_CDR2_BE = 0x0012,
        PL_CDR2_LE = 0x0013,
        D_CDR_BE = 0x0014,
        D_CDR_LE = 0x0015,
        XML = 0x0004,
    }

    public class UnsupportedCDRRepresentation : Exception
    {
        private readonly ushort _rid;

        public UnsupportedCDRRepresentation(ushort rid)
        {
            _rid = rid;
        }

        public override string ToString()
        {
            string ridName = Enum.IsDefined(typeof(RepresentationId), (int)_rid) ?
                String.Format(" ({0})", ((RepresentationId)_rid).ToString()) : "";
            return String.Format("Unsupported CDR representation: 0x{0:X4}{1}", _rid, ridName);
        }
    }

}