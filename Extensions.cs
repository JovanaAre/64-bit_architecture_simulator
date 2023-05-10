using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Interpreter
{   
    // Klasa za rad sa flag-ovima
    public static class Extensions
    {
        private const RFLAGS AllEFlags = RFLAGS.AdjustFlag | RFLAGS.CarryFlag | RFLAGS.DirectionFlag | RFLAGS.OverflowFlag | RFLAGS.ParityFlag | RFLAGS.SignFlag | RFLAGS.ZeroFlag;

        public static RFLAGS Set(this RFLAGS rFlags, RFLAGS flags)
        {
            return rFlags | flags & AllEFlags;
        }

        public static RFLAGS Clear(this RFLAGS rFlags, RFLAGS flags)
        {
            return rFlags & (RFLAGS)(AllEFlags - flags);
        }

        public static RFLAGS Clear(this RFLAGS rFlags)
        {
            return RFLAGS.ZeroFlag;
        }
    }
}
