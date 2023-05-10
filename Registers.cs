using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ASM_Interpreter
{   
    // Podtipovi - dijelovi registra
    public enum RegisterSubType
    {
        None,
        Full,
        LowDoubleWord,
        LowWord,
        HighByte,
        LowByte
    }

    /// <summary>
    /// Klasa 64-bitnih registara opšte namjene
    /// </summary>
    public class Register
    {
        /// <summary>
        /// Vrijednost registra
        /// </summary>
        public ulong Value;

        /// <summary>
        /// Inicijalizacija vrijednosti registra na 0
        /// </summary>
        public Register() { }

        /// <summary>
        /// Inicijalizacija registra specificiranom vrijednošću
        /// </summary>
        /// <param name="value">Vrijednost na koju se inicijalizuje registar</param>
        public Register(ulong value)
        {
            Value = value;
        }

        /// <summary>
        /// Nižih 32 bita registra
        /// </summary>
        public uint LowDoubleWord
        {
            get
            {
                return (uint)(Value & 0xFFFFFFFF);
            }
            set
            {
                Value = (uint)((Value & 0xFFFFFFFF00000000) | value);
            }
        }

        /// <summary>
        /// Nižih 16 bita registra LowDoubleWord-a registra
        /// </summary>
        public ushort LowWord
        {
            get
            {
                return (ushort)(Value & 0xFFFF);
            }
            set
            {
                Value = (uint)((Value & 0xFFFFFFFFFFFF0000) | value);
            }
        }

        /// <summary>
        /// Viših 8 bita LowWorda-a registra
        /// </summary>
        public byte HighByte
        {
            get
            {
                return (byte)((Value & 0xFF00) >> 8);
            }
            set
            {
                Value = (uint)((Value & 0xFFFFFFFFFFFF00FF) | (uint)((ushort)value << 8));
            }
        }

        /// <summary>
        /// Nižih 8 bita registra
        /// </summary>
        public byte LowByte
        {
            get
            {
                return (byte)(Value & 0xFF);
            }
            set
            {
                Value = (uint)((Value & 0xFFFFFFFFFFFFFF00) | value);
            }
        }
   
        /// <summary>
        /// Postavlja specificirani dio registra na specificiranu vrijednost
        /// </summary>
        /// <param name="type">Specificirani dio registra koji postavljamo.</param>
        /// <param name="value">Vrijednost na koju postavljamo.</param>
        public void SetValue(RegisterSubType type, ulong value)
        {
            switch (type)
            {
                case RegisterSubType.Full: Value = value; break;
                case RegisterSubType.LowDoubleWord: LowDoubleWord = (uint)value; break;
                case RegisterSubType.LowWord: LowWord = (ushort)value; break;
                case RegisterSubType.LowByte: LowByte = (byte)value; break;
                case RegisterSubType.HighByte: HighByte = (byte)value; break;
                default: throw new ArgumentException();
            }
        }

        /// <summary>
        /// Vraća vrijednost specificiranog dijela registra
        /// </summary>
        /// <param name="type">Specificirani dio registra koji uzimamo</param>
        /// <returns></returns>
        public ulong GetValue(RegisterSubType type)
        {
            switch (type)
            {
                case RegisterSubType.Full: return Value;
                case RegisterSubType.LowDoubleWord: return LowDoubleWord;
                case RegisterSubType.LowWord: return LowWord;
                case RegisterSubType.LowByte: return LowByte;
                case RegisterSubType.HighByte: return HighByte;
                default: throw new ArgumentException();
            }
        }
       
        /// <summary>
        /// Pruža automatsko kastovanje objekta tipa registar u unsigned long
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static implicit operator ulong(Register reg)
        {
            return reg.Value;
        }

        /// <summary>
        /// Pruža automatsko kastovanje unsigned long u objekat tipa registar.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static implicit operator Register(ulong reg)
        {
            return new Register(reg);
        }
    }

    /// <summary>
    /// Accumulator RAX - naslijeđuje klasu Register (za operande i rezultate operacija).
    /// </summary>
    public class RAX : Register
    {
        public readonly string Name = "RAX";
        public uint EAX { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort AX { get { return LowWord; } set { LowWord = value; } }
        public byte AL { get { return LowByte; } set { LowByte = value; } }
        public byte AH { get { return HighByte; } set { HighByte = value; } }
    }

    /// <summary>
    /// Base Index - RBX (Data pointer)
    /// </summary>
    public class RBX : Register
    {
        public readonly string Name = "RBX";
        public uint EBX { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort BX { get { return LowWord; } set { LowWord = value; } }
        public byte BL { get { return LowByte; } set { LowByte = value; } }
        public byte BH { get { return HighByte; } set { HighByte = value; } }
    }

    /// <summary>
    /// Counter - RCX (za stringove i petlje)
    /// </summary>
    public class RCX : Register
    {
        public readonly string Name = "RCX";
        public uint ECX { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort CX { get { return LowWord; } set { LowWord = value; } }
        public byte CL { get { return LowByte; } set { LowByte = value; } }
        public byte CH { get { return HighByte; } set { HighByte = value; } }
    }

    /// <summary>
    /// Data - RDX 
    /// </summary>
    public class RDX : Register
    {
        public readonly string Name = "RDX";
        public uint EDX { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort DX { get { return LowWord; } set { LowWord = value; } }
        public byte DL { get { return LowByte; } set { LowByte = value; } }
        public byte DH { get { return HighByte; } set { HighByte = value; } }
    }

    /// <summary>
    /// Stack pointer - RSP
    /// </summary>
    public class RSP : Register
    {
        public readonly string Name = "RSP";
        public uint ESP { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort SP { get { return LowWord; } set { LowWord = value; } }
    }

    /// <summary>
    /// Base index - RBP
    /// </summary>
    public class RBP : Register
    {
        public readonly string Name = "RBP";
        public uint EBP { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort BP { get { return LowWord; } set { LowWord = value; } }
    }

    /// <summary>
    /// Source index - RSI (string operations)
    /// </summary>
    public class RSI : Register
    {
        public readonly string Name = "RSI";
        public uint ESI { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort SI { get { return LowWord; } set { LowWord = value; } }
    }

    /// <summary>
    /// Destination index - RDI (string operations)
    /// </summary>
    public class RDI : Register
    {
        public readonly string Name = "RDI";
        public uint EDI { get { return LowDoubleWord; } set { LowDoubleWord = value; } }
        public ushort DI { get { return LowWord; } set { LowWord = value; } }
    }

    /// <summary>
    /// Mapiranje svih flag-ova potrebnih pri izvršavanju osnovnih instrukcija
    /// System i reserved flag-ovi nisu navedeni
    /// </summary>
    public enum RFLAGS
    {
        /// <summary>
        /// CF - Postavlja se na 1 ako aritmetička operacija generiše prenos (+) ili pozajmicu (-)
        /// najznačajnijeg bita rezultata; U suprotnom je 0; 
        /// </summary>
        CarryFlag = 1 << 0,

        /// <summary>
        /// PF - Postavlja se na 1 ako najniži bajt rezultata sadrži paran broj jedinica (1)
        /// U suprotnom je 0;
        /// </summary>
        ParityFlag = 1 << 2,

        /// <summary>
        /// AF - Postavlja se na 1 ako aritmetička operacija generiše prenos ili pozajmicu
        /// trećeg bita rezultata; Inače je 0; Koristi se za BCD aritmetiku;
        /// </summary>
        AdjustFlag = 1 << 4,

        /// <summary>
        /// ZF - Postavlja se na 1, ako je rezultat operacije 0; Inače je 0;
        /// </summary>
        ZeroFlag = 1 << 6,

        /// <summary>
        /// SF - Bit znaka rezultata (kopija najvišeg bita rezultata)
        /// Tj. postavlja se na 1, ako je rezultat negativan, a na 0 ako je pozitivan;
        /// </summary>
        SignFlag = 1 << 7,

        /// <summary>
        /// DF - Ako je postavljen na 1 sadržaj registara RSI i RDI se dekrementuje pri izvršavanju
        /// funkcije za kopiranje sadržaja nizova ili stringova; U suprotnom sadržaj registara RSI i RDI
        /// se inkrementuje pri izvršavanju kopiranja;
        /// </summary>
        DirectionFlag = 1 << 10,

        /// <summary>
        /// OF - Bit prekoračenja, koristi se kod operacija sa označenim brojevima
        /// dok se kod neoznačenih ignoriše; Postavlja se na 1 ako je došlo do prekoračenja;
        /// U suprotnom je jednak 0;
        /// </summary>
        OverflowFlag = 1 << 11,
    }
}
