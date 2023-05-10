using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Interpreter
{   
    // Tipovi mogućih argumenata instrukcija
    public enum ArgumentType
    {
        Immediate,
        Register,
        memory
    }

    // Klasa koja implementira metode za pravilno određivanje tipa i veličine
    // argumenata instrukcija
    public class InstructionArgument
    {
        private Interpreter interpreter;

        // Informacije potrebne za tipove argumenata
        public readonly ArgumentType Type;
        public readonly RegisterSubType RegisterType;
        public readonly string RegisterName;
        public uint Size;

        // Vrijednosti argumenata
        public readonly Register Register;
        public readonly ulong Address;
        public readonly ulong Immediate;

        /// <summary>
        /// Konstruktor klase - prima objekat klase Interpreter
        /// i proslijeđeni argument tipa string 
        /// </summary>
        /// <param name="arg"></param>
        public InstructionArgument(Interpreter interpreter, string arg)
        {
            this.interpreter = interpreter;
            arg = arg.Trim();

            // Slučaj kada argumen sadrži neku od direktiva za rad sa memorijom
            if (arg.Contains("DIR"))
            {
                // Obrada asemblerskih direktiva
                // Zbog jednostavnnije implementacije umjesto navođenja samo imena
                // direktive, koristi se i ključna riječ DIR
                if (arg.StartsWith("BYTE DIR [")) Size = 1;
                else if (arg.StartsWith("WORD DIR [")) Size = 2;
                else if (arg.StartsWith("DWORD DIR [")) Size = 4;
                else if (arg.StartsWith("QWORD DIR [")) Size = 8;
                else throw new ArgumentException();
                
                int lbi = arg.IndexOf('[');
                int rbi = arg.IndexOf(']');
                if (lbi > -1 && rbi > -1 && rbi > lbi)
                {
                    // Dohvatanje stringa unutar zagrada []
                    string dir = arg.Substring(lbi + 1, rbi - lbi - 1);

                    // Zamijena registara sa vrijednostima sadržanim u njima
                    dir = dir.Replace("RAX", interpreter.RAX.Value.ToString());
                    dir = dir.Replace("RBX", interpreter.RBX.Value.ToString());
                    dir = dir.Replace("RCX", interpreter.RCX.Value.ToString());
                    dir = dir.Replace("RDX", interpreter.RDX.Value.ToString());
                    dir = dir.Replace("RSI", interpreter.RSI.Value.ToString());
                    dir = dir.Replace("RDI", interpreter.RDI.Value.ToString());
                    dir = dir.Replace("RSP", interpreter.RSP.Value.ToString());
                    dir = dir.Replace("RBP", interpreter.RBP.Value.ToString());
                    dir = dir.Replace("EAX", interpreter.RAX.EAX.ToString());
                    dir = dir.Replace("EBX", interpreter.RBX.EBX.ToString());
                    dir = dir.Replace("ECX", interpreter.RCX.ECX.ToString());
                    dir = dir.Replace("EDX", interpreter.RDX.EDX.ToString());
                    dir = dir.Replace("ESI", interpreter.RSI.ESI.ToString());
                    dir = dir.Replace("EDI", interpreter.RDI.EDI.ToString());
                    dir = dir.Replace("ESP", interpreter.RSP.ESP.ToString());
                    dir = dir.Replace("EBP", interpreter.RBP.EBP.ToString());
                    dir = dir.Replace("AX", interpreter.RAX.AX.ToString());
                    dir = dir.Replace("BX", interpreter.RBX.BX.ToString());
                    dir = dir.Replace("CX", interpreter.RCX.CX.ToString());
                    dir = dir.Replace("DX", interpreter.RDX.DX.ToString());
                    dir = dir.Replace("SI", interpreter.RSI.SI.ToString());
                    dir = dir.Replace("DI", interpreter.RDI.DI.ToString());
                    dir = dir.Replace("SP", interpreter.RSP.SP.ToString());
                    dir = dir.Replace("BP", interpreter.RBP.BP.ToString());
                    dir = dir.Replace("AL", interpreter.RAX.AL.ToString());
                    dir = dir.Replace("AH", interpreter.RAX.AH.ToString());
                    dir = dir.Replace("BL", interpreter.RBX.BL.ToString());
                    dir = dir.Replace("BH", interpreter.RBX.BH.ToString());
                    dir = dir.Replace("CL", interpreter.RCX.CL.ToString());
                    dir = dir.Replace("CH", interpreter.RCX.CH.ToString());
                    dir = dir.Replace("DL", interpreter.RDX.DL.ToString());
                    dir = dir.Replace("DH", interpreter.RDX.DH.ToString());

                    // Izračunavanje rezultujuće memorijske adrese korištenjem odgovarajućih Regexa
                    // i funkcija za izračunavanje izraza
                    Address = Expression.Evaluate(dir);
                }
                else throw new ArgumentException();

                Type = ArgumentType.memory;
            }

            // Slučajevi kada je argument neki od registara
            else if (arg == "RAX")
            {
                Register = interpreter.RAX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RAX";
                Size = 8;
            }
            else if (arg == "EAX")
            {
                Register = interpreter.RAX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "EAX";
                Size = 4;
            }
            else if (arg == "AX")
            {
                Register = interpreter.RAX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "AX";
                Size = 2;
            }
            else if (arg == "AH")
            {
                Register = interpreter.RAX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.HighByte;
                RegisterName = "AH";
                Size = 1;
            }
            else if (arg == "AL")
            {
                Register = interpreter.RAX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowByte;
                RegisterName = "AL";
                Size = 1;
            }
            else if (arg == "RBX")
            {
                Register = interpreter.RBX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RBX";
                Size = 8;
            }
            else if (arg == "EBX")
            {
                Register = interpreter.RBX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "EBX";
                Size = 4;
            }
            else if (arg == "BX")
            {
                Register = interpreter.RBX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "BX";
                Size = 2;
            }
            else if (arg == "BH")
            {
                Register = interpreter.RBX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.HighByte;
                RegisterName = "BH";
                Size = 1;
            }
            else if (arg == "BL")
            {
                Register = interpreter.RBX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowByte;
                RegisterName = "BL";
                Size = 1;
            }
            else if (arg == "RCX")
            {
                Register = interpreter.RCX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RCX";
                Size = 8;
            }
            else if (arg == "ECX")
            {
                Register = interpreter.RCX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "ECX";
                Size = 4;
            }
            else if (arg == "CX")
            {
                Register = interpreter.RCX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "CX";
                Size = 2;
            }
            else if (arg == "CH")
            {
                Register = interpreter.RCX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.HighByte;
                RegisterName = "CH";
                Size = 1;
            }
            else if (arg == "CL")
            {
                Register = interpreter.RCX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowByte;
                RegisterName = "CL";
                Size = 1;
            }
            else if (arg == "RDX")
            {
                Register = interpreter.RDX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RDX";
                Size = 8;
            }
            else if (arg == "EDX")
            {
                Register = interpreter.RDX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "EDX";
                Size = 4;
            }
            else if (arg == "DX")
            {
                Register = interpreter.RDX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "DX";
                Size = 2;
            }
            else if (arg == "DH")
            {
                Register = interpreter.RDX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.HighByte;
                RegisterName = "DH";
                Size = 1;
            }
            else if (arg == "DL")
            {
                Register = interpreter.RDX;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowByte;
                RegisterName = "DL";
                Size = 1;
            }
            else if (arg == "RSI")
            {
                Register = interpreter.RSI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RSI";
                Size = 8;
            }
            else if (arg == "ESI")
            {
                Register = interpreter.RSI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "ESI";
                Size = 4;
            }
            else if (arg == "SI")
            {
                Register = interpreter.RSI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "SI";
                Size = 2;
            }
            else if (arg == "RDI")
            {
                Register = interpreter.RDI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RDI";
                Size = 8;
            }
            else if (arg == "EDI")
            {
                Register = interpreter.RDI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "EDI";
                Size = 4;
            }
            else if (arg == "DI")
            {
                Register = interpreter.RDI;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "DI";
                Size = 2;
            }
            else if (arg == "RSP")
            {
                Register = interpreter.RSP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RSP";
                Size = 8;
            }
            else if (arg == "ESP")
            {
                Register = interpreter.RSP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "ESP";
                Size = 4;
            }
            else if (arg == "SP")
            {
                Register = interpreter.RSP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "SP";
                Size = 2;
            }
            else if (arg == "RBP")
            {
                Register = interpreter.RBP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.Full;
                RegisterName = "RBP";
                Size = 8;
            }
            else if (arg == "EBP")
            {
                Register = interpreter.RBP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowDoubleWord;
                RegisterName = "EBP";
                Size = 4;
            }
            else if (arg == "BP")
            {
                Register = interpreter.RBP;
                Type = ArgumentType.Register;
                RegisterType = ASM_Interpreter.RegisterSubType.LowWord;
                RegisterName = "BP";
                Size = 2;
            }
            // Slučaj kada je argument tipa Immediate - konstantantna vrijednost
            else
            {
                Type = ArgumentType.Immediate;

                // Dohvatanje vrijednosti konstante
                // Slučaj kada je konstanta zadata kao heksadecimalna vrijednost
                if (arg.EndsWith("H"))
                {
                    Immediate = (ulong)Convert.ToUInt64(arg.Substring(0, arg.Length - 1), 16);
                }
                // Slučaj kada je konstanta decimalna vrijednost
                else Immediate = (uint)Convert.ToUInt64(arg);

                // Dohvatanje veličine immediate vrijednosti
                if ((ulong)Immediate < byte.MaxValue) Size = 1;
                else if ((ulong)Immediate < ushort.MaxValue) Size = 2;
                else if ((ulong)Immediate < uint.MaxValue) Size = 4;
                else Size = 8;
            }
        }

        /// <summary>
        /// Dohvatanje unsigned long vrijednosti tekućeg argumenta
        /// </summary>
        /// <returns></returns>
        public ulong GetValue()
        {
            switch (Type)
            {
                case ArgumentType.Register: return Register.GetValue(RegisterType);
                case ArgumentType.memory: return interpreter.Memory.ReadFromMemory(Address, Size);
                case ArgumentType.Immediate: return Immediate;
                default: throw new Exception();
            }
        }

        /// <summary>
        /// Postavljanje vrijednosti argumenta i automatsko kastovanje prema njegovom tipu (registar, memorija ili konstanta -immediate)
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ulong value)
        {
            switch (Type)
            {
                case ArgumentType.Register:
                    Register.SetValue(RegisterType, value);
                    break;
                case ArgumentType.memory:
                    interpreter.Memory.WriteToMemory(Address, Size, value);
                    break;
                case ArgumentType.Immediate: throw new ArgumentException("Nedozvoljeno modifikovanje vrijednosti readonly immediate tipa.");
                default: throw new ArgumentException("Neispravan tip argumenta.");
            }
        }
    }
}
