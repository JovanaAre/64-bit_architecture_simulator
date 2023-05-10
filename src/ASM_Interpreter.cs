using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Linq;
using System.Text;

// Napomena: .asm fajl će biti tretiran bez segmenata, tj. posmatraćemo kao da imamo samo .text segment

namespace ASM_Interpreter
{
    public partial class Interpreter
    {
        public Interpreter()
        {
            RBP.Value = RSP.Value = (UInt64)maxMemorySize - 1;
            // Za Sysread 
            //initRegForSysread();
            // Za Syswrite
            //initRegForSyswrite();
        }

        // Instanciranje registara opšte namjene
        public readonly RAX RAX = new RAX();
        public readonly RBX RBX = new RBX();
        public readonly RCX RCX = new RCX();
        public readonly RDX RDX = new RDX();
        public readonly RSI RSI = new RSI();
        public readonly RDI RDI = new RDI();
        public readonly RSP RSP = new RSP();
        public readonly RBP RBP = new RBP();
        public uint RIP = 1;   // Instruction pointer (za rad sa linijama .asm fajla)
        public RFLAGS RFLAGS = RFLAGS.ZeroFlag;

        // Pomoćna funkcija za inicijalizovanje registara
        // Za potrebe primjera sistemskog poziva Sysread
        public void initRegForSysread()
        {
            RSI.Value = 2;
            RDX.Value = 3;
        }

        // Pomoćna funkcija za inicijalizovanje registara i memorije
        // Za potrebe primjera sistemskog poziva Sysread
        public void initRegForSyswrite()
        {
            RSI.Value = 2;
            RDX.Value = 3;
            // upis abc u memoriju na lokaciju RSI.Value
            Memory [RSI.Value] = 97;
            Memory[RSI.Value + 1] = 98;
            Memory[RSI.Value + 2] = 99;
        }
        // Funkcija koja vraća registre na osnovu imena
        public Register GetRegisterByName(string reg)
        {
            switch (reg)
            {
                case "RAX":
                case "EAX":
                case "AX":
                case "AL":
                case "AH": return RAX;
                case "RBX":
                case "EBX":
                case "BX":
                case "BL":
                case "BH": return RBX;
                case "RCX":
                case "ECX":
                case "CX":
                case "CL":
                case "CH": return RCX;
                case "RDX":
                case "EDX":
                case "DX":
                case "DL":
                case "DH": return RDX;
                case "RSI":
                case "ESI":
                case "SI": return RSI;
                case "RDI":
                case "EDI":
                case "DI": return RDI;
                case "RSP":
                case "ESP":
                case "SP": return RSP;
                case "RBP":
                case "EBP":
                case "BP": return RBP;
                default: throw new ArgumentException();
            }
        }

        // Funkcija koja vraća vrijednost registara na osnovu imena
        public ulong GetRegisterValueByName(string reg)
        {
            switch (reg)
            {
                case "RAX": return RAX.Value;
                case "EAX": return RAX.EAX;
                case "AX": return RAX.AX;
                case "AL": return RAX.AL;
                case "AH": return RAX.AH;
                case "RBX": return RBX.Value;
                case "EBX": return RBX.EBX;
                case "BX": return RBX.BX;
                case "BL": return RBX.BL;
                case "BH": return RBX.BH;
                case "RCX": return RCX.Value;
                case "ECX": return RCX.ECX;
                case "CX": return RCX.CX;
                case "CL": return RCX.CL;
                case "CH": return RCX.CH;
                case "RDX": return RDX.Value;
                case "EDX": return RDX.EDX;
                case "DX": return RDX.DX;
                case "DL": return RDX.DL;
                case "DH": return RDX.DH;
                case "RSI": return RSI.Value;
                case "ESI": return RSI.ESI;
                case "SI": return RSI.SI;
                case "RDI": return RDI.Value;
                case "EDI": return RDI.EDI;
                case "DI": return RDI.DI;
                case "RSP": return RSP.Value;
                case "ESP": return RSP.ESP;
                case "SP": return RSP.SP;
                case "RBP": return RBP.Value;
                case "EBP": return RBP.EBP;
                case "BP": return RBP.BP;
                default: throw new ArgumentException();
            }
        }

        // Funkcija koja vrijednost registara na osnovu imena
        public void SetRegisterValueByName(string reg, ulong value)
        {
            switch (reg)
            {
                case "RAX": RAX.Value = value; break;
                case "EAX": RAX.EAX = (uint)value; break;
                case "AX": RAX.AX = (ushort)value; break;
                case "AL": RAX.AL = (byte)value; break;
                case "AH": RAX.AH = (byte)value; break;
                case "RBX": RBX.Value = value; break;
                case "EBX": RBX.EBX = (uint)value; break;
                case "BX": RBX.BX = (ushort)value; break;
                case "BL": RBX.BL = (byte)value; break;
                case "BH": RBX.BH = (byte)value; break;
                case "RCX": RCX.Value = value; break;
                case "ECX": RCX.ECX = (uint)value; break;
                case "CX": RCX.CX = (ushort)value; break;
                case "CL": RCX.CL = (byte)value; break;
                case "CH": RCX.CH = (byte)value; break;
                case "RDX": RDX.Value = value; break;
                case "EDX": RDX.EDX = (uint)value; break;
                case "DX": RDX.DX = (ushort)value; break;
                case "DL": RDX.DL = (byte)value; break;
                case "DH": RDX.DH = (byte)value; break;
                case "RSI": RSI.Value = value; break;
                case "ESI": RSI.ESI = (uint)value; break;
                case "SI": RSI.SI = (ushort)value; break;
                case "RDI": RDI.Value = value; break;
                case "EDI": RDI.EDI = (uint)value; break;
                case "DI": RDI.DI = (ushort)value; break;
                case "RSP": RSP.Value = value; break;
                case "ESP": RSP.ESP = (uint)value; break;
                case "SP": RSP.SP = (ushort)value; break;
                case "RBP": RBP.Value = value; break;
                case "EBP": RBP.EBP = (uint)value; break;
                case "BP": RBP.BP = (ushort)value; break;
                default: throw new ArgumentException();
            }
        }

        #region RFLAGS
        // Funkcija za postavljanje flag-ova
        public void SetRFlags(RFLAGS flags)
        {
            RFLAGS = RFLAGS.Set(flags);
        }
        // Funkcija za brisanje - resetovanje flag-ova
        public void ClearRFlags(RFLAGS flags)
        {
            RFLAGS = RFLAGS.Clear(flags);
        }
        public void ClearRFlags()
        {
            RFLAGS = RFLAGS.Clear();
        }
        /// <summary>
        /// Ažurira RFLAGS u zavisnosti od rezultata result
        /// </summary>
        /// <param name="result">Ulong rezultat</param>
        /// <param name="size">Odredišna veličina rezultata u bajtovima</param>
        /// <param name="flags">Flag-ovi koji trebaju biti ažurirani</param>
        public void UpdateRFlags(ulong result, uint size, RFLAGS flags)
        {
            int val = (int)result;

            if (flags.HasFlag(RFLAGS.ZeroFlag))
            {
                if (result == 0) SetRFlags(RFLAGS.ZeroFlag);
                else ClearRFlags(RFLAGS.ZeroFlag);
            }

            // Postavljanje PF ako najniži bajt sadrži paran broj bitova postavljenih na 1
            if (flags.HasFlag(RFLAGS.ParityFlag))
            {

                if (((((((ulong)result & 0xFF) * (ulong)0x0101010101010101) & 0x8040201008040201) % 0x1FF) & 1) == 0)
                {
                    SetRFlags(RFLAGS.ParityFlag);
                }
                else ClearRFlags(RFLAGS.ParityFlag);
            }

            if (flags.HasFlag(RFLAGS.SignFlag))
            {
                uint signFlag = (uint)(1 << (int)((size << 3) - 1));
                if (((result & signFlag) == signFlag))
                {
                    SetRFlags(RFLAGS.SignFlag);
                }
                else ClearRFlags(RFLAGS.SignFlag);
            }

            if (flags.HasFlag(RFLAGS.AdjustFlag))   // 4. bit
            {
                if ((result & 8) == 8) SetRFlags(RFLAGS.AdjustFlag);
                else ClearRFlags(RFLAGS.AdjustFlag);
            }

            if (flags.HasFlag(RFLAGS.OverflowFlag)) // signed overflow
            {
                long signedResult = (long)result;
                switch (size)
                {
                    case 1:
                        if (signedResult > sbyte.MaxValue || signedResult < sbyte.MinValue)
                            SetRFlags(RFLAGS.OverflowFlag);
                        else ClearRFlags(RFLAGS.OverflowFlag);
                        break;
                    case 2:
                        if (signedResult > short.MaxValue || signedResult < short.MinValue)
                            SetRFlags(RFLAGS.OverflowFlag);
                        else ClearRFlags(RFLAGS.OverflowFlag);
                        break;
                    case 4:
                        if (signedResult > int.MaxValue || signedResult < int.MinValue)
                            SetRFlags(RFLAGS.OverflowFlag);
                        else ClearRFlags(RFLAGS.OverflowFlag);
                        break;
                    case 8:
                        if (signedResult > long.MaxValue || signedResult < long.MinValue)
                            SetRFlags(RFLAGS.OverflowFlag);
                        else ClearRFlags(RFLAGS.OverflowFlag);
                        break;
                    default: throw new ArgumentException();
                }
            }

            if (flags.HasFlag(RFLAGS.CarryFlag))    // unsigned overflow
            {
                if (((result >> ((int)size << 3)) & 1) == 1)
                {
                    SetRFlags(RFLAGS.CarryFlag);
                }
                else ClearRFlags(RFLAGS.CarryFlag);
            }
        }
        #endregion

        /// <summary>
        /// Putanja do .asm fajla
        /// </summary>
        public static string ASMFilePath = Environment.CurrentDirectory;

        /// <summary>
        /// Asm kod smješten u .asm fajlu
        /// </summary>
        public List<string> ASMCode;

        #region Memorija
        /// <summary>
        /// Maksimalna dozvoljena veličina memorije
        /// </summary>
        private static readonly uint maxMemorySize = 300_097_152;

        /// <summary>
        /// 64-bit adresabilna memorija
        /// </summary>
        public byte[] Memory = new byte[maxMemorySize];
        
        // Pomoćna funkcija za ispis vrijednosti registara
        public void printRegisters()
        {
            Console.WriteLine("REGISTRI:");
            Console.WriteLine("HEKSADECIMALNO                    DECIMALNO:");
            Console.Write("RAX = 0x{0:X16}          ", RAX.Value);
            Console.Write(RAX.Value);
            Console.WriteLine();
            Console.Write("RBX = 0x{0:X16}          ", RBX.Value);
            Console.Write(RBX.Value);
            Console.WriteLine();
            Console.Write("RCX = 0x{0:X16}          ", RCX.Value);
            Console.Write(RCX.Value);
            Console.WriteLine();
            Console.Write("RDX = 0x{0:X16}          ", RDX.Value);
            Console.Write(RDX.Value);
            Console.WriteLine();
            Console.Write("RSI = 0x{0:X16}          ", RSI.Value);
            Console.Write(RSI.Value);
            Console.WriteLine();
            Console.Write("RDI = 0x{0:X16}          ", RDI.Value);
            Console.Write(RDI.Value);
            Console.WriteLine();
            Console.Write("RSP = 0x{0:X16}          ", RSP.Value);
            Console.Write(RSP.Value);
            Console.WriteLine();
            Console.Write("RBP = 0x{0:X16}          ", RBP.Value);
            Console.Write(RBP.Value);
            Console.WriteLine();
        }

        // Pomoćna funkcija za ispis određenog dijela memorije za potrebe primjera
        public void printMemory()
        {
            Console.WriteLine("MEMORIJA:");
            //printMemoryForSysread();
            printMemoryForMovPushPop();
        }

        // Pomoćna funkcija za ispis memorije
        // Za potrebe primjera sistemskog poziva Sysread
        public void printMemoryForSysread()
        {
            //ako je u memoriji string "abc"
            Console.WriteLine("Upisali ste sljedeće vrijednosti, sa standardnog ulaza, u memoriju pri pozivu sistemskog poziva SysRead:");
            Console.WriteLine(Memory[RSI.Value]);
            Console.WriteLine(Memory[RSI.Value + 1]);
            Console.WriteLine(Memory[RSI.Value + 2]);
        }

        // Pomoćna funkcija za ispis memorije
        // Za potrebe primjera instrukcija MOV, PUSH i POP
        public void printMemoryForMovPushPop()
        {
            Console.Write("Memory[RAX] = ");
            Console.Write(Memory[RAX.Value]);
            Console.WriteLine();
            Console.Write("Memory[RDX] = ");
            Console.Write(Memory[RDX.Value]);
            Console.WriteLine();
            Console.Write("Memory[RSP - 8] = ");
            Console.Write(Memory[RSP - 8]);
            Console.WriteLine();
            Console.Write("Memory[RSP - 2] = ");
            Console.Write(Memory[RSP - 2]);
            Console.WriteLine();
        }

        // Pomoćna funkcija za ispis vrijednosti flag-ova
        public void printFlags()
        { 
            Console.WriteLine("FLAGOVI:");
            Console.Write("CF = ");
            if (RFLAGS.HasFlag(RFLAGS.CarryFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("PF = ");
            if (RFLAGS.HasFlag(RFLAGS.ParityFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("AF = ");
            if (RFLAGS.HasFlag(RFLAGS.AdjustFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("ZF = ");
            if (RFLAGS.HasFlag(RFLAGS.ZeroFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("SF = ");
            if (RFLAGS.HasFlag(RFLAGS.SignFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("DF = ");
            if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
            Console.Write("OF = ");
            if (RFLAGS.HasFlag(RFLAGS.OverflowFlag))
                Console.Write("1");
            else
                Console.Write("0");
            Console.WriteLine();
        }
        #endregion

        #region Rad sa sistemskim pozivima
        // Pomoćna funkcija koja postavlja vrijednost registra RAX na -1
        private void SetRaxToNegativeValue()
        {
            RAX.Value = 0xffff_ffff_ffff_ffff;
        }

        // Funkcija koja obrađuje sistemski poziv read
        private bool SysRead()
        {
            if (RDI.Value == 0) //standardni ulaz
            {
                var index = RSI.Value; //početna adresa memorijske lokacije gdje će biti upisan tekst
                if (index > maxMemorySize)
                {
                    Console.WriteLine("Vrijednost RSI je izvan granica memorije.");
                    return false;
                }
                else
                {
                    Console.WriteLine("Unesite sadrzaj koji ce biti smjesten u memoriju pozivom SysRead:");
                       var userInput = Console.ReadLine();
                        var maxLen = RDX.Value; //dužina teksta koji će biti unesen
                        if (maxLen > maxMemorySize || maxLen + index > maxMemorySize)
                        {
                            Console.WriteLine("Problem sa alokacijom memorije.");
                            return false;
                        }
                        else
                        {
                            if (userInput.Length > (int)maxLen)
                            {
                                userInput = userInput.Substring(0, (int)maxLen);
                            }

                        // RAX se setuje na vrijednost broja elemenata stringa upisanog
                        // u memoriju, ako je string uspješno upisan; Inače na -1
                        RAX.Value = !Memory.WriteStringToMemory(index, userInput) ? 0xffff_ffff_ffff_ffff : (UInt64)userInput.Length;
                            return true;
                        }
                    
                }
            }
            else
            {
                SetRaxToNegativeValue();
            }

            return true;
        }

        // Funkcija koja obrađuje sistemski poziv write
        private bool SysWrite()
        {
            if (RDI.Value == 1) //standardni izlaz
            {
                var index = RSI.Value; //početna adresa teksta koji je potrebno ispisati
                if (index > maxMemorySize)
                {
                    Console.WriteLine("Vrijednost RSI je izvan granica memorije.");
                    return false;
                }
                else
                {
                    var maxLen = RDX.Value; //dužina teksta koji je potrebno ispisati
                    if (maxLen > maxMemorySize || maxLen + index > maxMemorySize)
                    {
                        Console.WriteLine("Problem sa alokacijom memorije.");
                        return false;
                    }
                    else
                    {
                        // RAX se setuje na vrijednost broja elemenata pročitanog stringa
                        //iz memorije, ako je string uspješno pročitan; Inače na -1
                        RAX.Value = !Memory.ReadStringFromMemory(index, maxLen, out var stringFromMem) ? 0xffff_ffff_ffff_ffff : (UInt64)stringFromMem.Length;                        
                        Console.WriteLine("String dohvaćen iz memorije je:");
                        Console.WriteLine(stringFromMem);
                        return true;
                    }
                }
            }
            else
            {
                SetRaxToNegativeValue();
            }

            return true;
        }
        #endregion

        #region Interpreter
        // Funkcija koja vrši interpretaciju ASM koda
        public void Interpret_asm()
        {
            if (File.Exists(ASMFilePath))
            {
                // Čitanje svih linija .asm fajla u memoriju
                ASMCode = new List<string>(File.ReadAllLines(ASMFilePath));
            }
            else
            {
                Console.WriteLine($"Asm fajl \"{Path.GetFileName(ASMFilePath)}\" ne postoji.");
                return;
            }
            //Formiranje niza od gore-dobijene liste zbog lakše manipulacije instrukcijama
            string[] ASM_lines = ASMCode.ToArray();

            Console.WriteLine("Početno stanje registara:");
            printRegisters();

            while (RIP <= ASM_lines.Length)
            {
                string mnemonic = null;
                string syscall = "SYSCALL"; //mnemonic sistemskog poziva
                bool notInstruction = false; //True ukoliko je .asm linija koda prazna ili sadrži labelu

                // Dohvatanje trenutne instrukcije
                string instruction = NormalizeInstruction(ASM_lines[RIP-1]);
                
                Console.Write("Trenutna vrijednost Instruction Pointer-a: RIP = ");
                Console.WriteLine(RIP);

                // Provjera whitespace-a i labela
                if (instruction.Length == 0 || instruction.EndsWith(":"))
                {
                    RIP++;
                    notInstruction = true;
                }

                if (notInstruction == false)
                {
                    // Dohvatanje mnemonic-a instrukcije (npr. MOV,ADD,..)
                    int nextSpace = instruction.IndexOf(' ');
                    mnemonic = (nextSpace > -1) ? instruction.Substring(0, nextSpace) : instruction;

                    // Instrukcije se dijele u 3 dijela i u skladu s tim se obrađuju:
                    // Skokovi, sistemski pozivi i preostale instrukcije

                    // Skok
                    if (mnemonic[0] == 'J')
                    {
                        Console.WriteLine("Tekuća instrukcija je:");
                        Console.WriteLine(instruction);
                        string label = instruction.Substring(instruction.IndexOf(' ') + 1);
                        ExecuteJump(mnemonic, ref ASM_lines, label);  
                    }
                    else if (mnemonic.Equals(syscall))
                    {
                        Console.WriteLine("Tekuća instrukcija je:");
                        Console.WriteLine(instruction);
                        switch (RAX.Value)
                        {
                            // SysRead
                            case 0:
                                SysRead();
                                printRegisters();
                                printMemory();
                                RIP++;
                                break;
                            // SysWrite
                            case 1:
                                SysWrite();
                                printRegisters();
                                RIP++;
                                break;
                            default:
                                SetRaxToNegativeValue();
                                break;
                        }

                    }
                    else // Standardna instrukcija (koja nije sistemski poziv ili skok)
                    {
                        // Parsiranje argumenata instrukcije
                        List<InstructionArgument> arguments = new List<InstructionArgument>();
                        
                        Console.WriteLine("Tekuća instrukcija je:");
                        Console.WriteLine(instruction);

                        // Sljedeća linija koda izbacuje mnemonic iz stringa tekuće instrukcije, briše prazan prostor ako ga ima
                        // A zatim argumente odvaja zarezom i smiješa u niz argums
                        string[] argums = instruction.Replace(mnemonic, string.Empty).Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            foreach (string argum in argums)
                            {
                                InstructionArgument item = new InstructionArgument(this, argum);
                                arguments.Add(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        // Verifikacija tipova argumenata

                        InstructionArgument dst = null;
                        InstructionArgument src = null;
                        InstructionArgument arg2 = null;

                        if (argums.Length == 0)
                        {

                        }
                        else if (argums.Length == 1)
                        {
                            dst = arguments[0];
                            src = arguments[0];
                        }
                        else if (argums.Length == 2)
                        {
                            dst = arguments[0];
                            src = arguments[1];
                        }
                        else if (argums.Length == 3)
                        {
                            arg2 = arguments[2];
                        }
                        else throw new ArgumentException();

                        // Destination lokacija ne može biti konstanta - immediate
                        if (argums.Length == 2 && dst.Type == ArgumentType.Immediate) throw new ArgumentException();

                        switch (mnemonic)
                        {
                            case "NOP":
                                RIP++;
                                break;
                            // Sintaksa: MOV DEST, SRC
                            // Instrukcija MOV ne utiče na flag-ove
                            // Premiješta sadržaj sa adrese SRC na adresu DEST
                            case "MOV":
                                if (argums.Length != 2)
                                {
                                    throw new ArgumentException("Instrukcija MOV zahtijeva 2 argumenta.");
                                }
                                else if (dst.Type == ArgumentType.Immediate)
                                {
                                    throw new ArgumentException("Immediate ne može biti destination adresa.");
                                }
                                else if (src.Type == ArgumentType.Immediate && src.Size > dst.Size)
                                {
                                  
                                    throw new ArgumentException("Immediate veličina je veća od destination veličine.");
                                }
                                else if (src.Type != ArgumentType.Immediate && src.Size != dst.Size)
                                {
                                   
                                    throw new ArgumentException("Source i destination veličine moraju biti iste.");
                                }
                                else
                                {
                                    dst.SetValue(src.GetValue());
                                    RIP++;
                                    printRegisters();
                                    printMemory();
                                }
                                break;
                            // Sintaksa: MOVSB
                            // Pomijera podatak veličine bajta sa adrese memorijske adrese RSI na adresu RDI
                            case "MOVSB":
                                Memory.WriteToMemory((uint)RDI.Value, 1, Memory.ReadFromMemory((uint)RSI.Value, 1));
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value--;
                                    RSI.Value--;
                                }
                                else
                                {
                                    RDI.Value++;
                                    RSI.Value++;
                                }
                                RIP++;
                                break;
                            // Sintaksa: MOVSW
                            // Pomijera podatak veličine word-a sa adrese memorijske adrese RSI na adresu RDI
                            case "MOVSW":
                                Memory.WriteToMemory((uint)RDI.Value, 2, Memory.ReadFromMemory((uint)RSI.Value, 2));
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value -= 2;
                                    RSI.Value -= 2;
                                }
                                else
                                {
                                    RDI.Value += 2;
                                    RSI.Value += 2;
                                }
                                RIP++;
                                break;
                            // Sintaksa: MOVSD
                            // Pomijera podatak veličine double word-a sa adrese memorijske adrese RSI na adresu RDI
                            case "MOVSD":
                                Memory.WriteToMemory((uint)RDI.Value, 4, Memory.ReadFromMemory((uint)RSI.Value, 4));
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value -= 4;
                                    RSI.Value -= 4;
                                }
                                else
                                {
                                    RDI.Value += 4;
                                    RSI.Value += 4;
                                }
                                RIP++;
                                break;
                            // Sintaksa: STOSB
                            // Skladišti vrijednost registra AL na memorijsku adresu od RDI
                            case "STOSB":
                                Memory.WriteToMemory(RDI.Value, 1, RAX.AL);
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value--;
                                    RSI.Value--;
                                }
                                else
                                {
                                    RDI.Value++;
                                    RSI.Value++;
                                }
                                RIP++;
                                break;
                            // Sintaksa: STOSW
                            // Skladišti vrijednost registra AX na memorijsku adresu od RDI
                            case "STOSW":
                                Memory.WriteToMemory(RDI.Value, 2, RAX.AX);
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value -= 2;
                                    RSI.Value -= 2;
                                }
                                else
                                {
                                    RDI.Value += 2;
                                    RSI.Value += 2;
                                }
                                RIP++;
                                break;
                            // Sintaksa: STOSD
                            // Skladišti vrijednost registra EAX na memorijsku adresu od RDI
                            case "STOSD":
                                Memory.WriteToMemory(RDI.Value, 4, RAX.EAX);
                                printRegisters();
                                RCX.Value--;
                                if (RFLAGS.HasFlag(RFLAGS.DirectionFlag))
                                {
                                    RDI.Value -= 4;
                                    RSI.Value -= 4;
                                }
                                else
                                {
                                    RDI.Value += 4;
                                    RSI.Value += 4;
                                }
                                RIP++;
                                break;
                            // Sintaksa: XCHG DEST, SRC
                            // Ne utiče na flag-ove
                            // Mijenja sadržaje sa source i destination adresa
                            case "XCHG":
                                if (argums.Length != 2)
                                {
                                    throw new ArgumentException("XCHG mnemonic prihvata isključivo 2 argumenta.");
                                }
                                else if (dst.Type != ArgumentType.Register || src.Type == ArgumentType.Immediate)
                                {
                                    throw new ArgumentException("Destination mora biti registar i source ne može biti immediate vrijednost.");
                                }
                                else if (src.Size != dst.Size)
                                {
                                    throw new ArgumentException("Source i destination veličine moraju biti iste.");
                                }
                                else
                                {
                                    ulong tmp = src.GetValue();
                                    src.SetValue(dst.GetValue());
                                    dst.SetValue(tmp);
                                    printRegisters();
                                    RIP++;
                                }
                                break;
                            // Sintaksa: NOT DEST
                            // Bitska NOT operacija (svaka 0 se postavlja na 1 i svaka 1 se postavlja na 0)
                            // nad destination operandom
                            // Rezultat se smiješta na destination lokaciju
                            // Ne utiče na flag-ove
                            case "NOT":
                                if (argums.Length == 1 && dst.Type != ArgumentType.Immediate)
                                {
                                    dst.SetValue(~dst.GetValue());
                                    printRegisters();
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: NEG DEST
                            // Unarni operator koji mijenja vrijednost sa destination lokacije sa njenim
                            // drugim komplementom i rezultat smiješta na destination 
                            // NEG je zapravo množenje sa -1 (pozitivan broj postaje negativan, a negativan pozitivan)
                            // 0 ostaje nepromijenjena
                            case "NEG":
                                if (argums.Length == 1 && dst.Type != ArgumentType.Immediate)
                                {
                                    ulong val = (ulong)(-(long)dst.GetValue());
                                    UpdateRFlags(val, dst.Size, RFLAGS.OverflowFlag | RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.AdjustFlag | RFLAGS.ParityFlag | RFLAGS.CarryFlag);
                                    dst.SetValue((ulong)val);
                                    printRegisters();
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: ADD DEST, SRC
                            // Sabira podatke sa DEST i SRC adrese i rezultat smiješta u DEST
                            // Promjena flag-ova kod ADD instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF=1 ako dodavanje uzrokuje prenos sa višeg bita; Inače je CF=0
                            // OF=1 ako dodavanje rezultuje arithmetic under/ overflow; Inače je OF=0
                            case "ADD":
                                if (argums.Length == 2)
                                {
                                    ulong val = (ulong)dst.GetValue() + src.GetValue();
                                    UpdateRFlags(val, dst.Size, RFLAGS.OverflowFlag | RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.AdjustFlag | RFLAGS.ParityFlag | RFLAGS.CarryFlag);
                                    dst.SetValue((uint)val);
                                    RIP++;
                                    printRegisters();
                                }
                                else throw new ArgumentException();
                                break;
                            // Sintaksa: SUB DEST, SRC
                            // Oduzima podatke sa DEST i SRC adrese i rezultat smiješta u DEST
                            // Promjena flag-ova kod SUB instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF=1 ako oduzimanje povlači posudjivanje sa niža 4 bita; Inače je CF=0
                            // OF=1 ako oduzimanje rezultuje arithmetic under/ overflow; Inače je OF=0
                            case "SUB":
                                if (argums.Length == 2)
                                {
                                    ulong val = (ulong)dst.GetValue() - (ulong)src.GetValue();
                                    UpdateRFlags(val, dst.Size, RFLAGS.OverflowFlag | RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.AdjustFlag | RFLAGS.ParityFlag | RFLAGS.CarryFlag);
                                    dst.SetValue((uint)val);
                                    RIP++;
                                    printRegisters();
                                }
                                else throw new ArgumentException();
                                break;
                            // Sintaksa: AND DEST, SRC
                            // Bitsko AND ( 1 & 1 = 1, 1 & 0 = 0, 0 & 1 = 0, 0 & 0 = 0)
                            // Rezultat smiješta na DEST lokaciju
                            // Promjena flag-ova kod AND instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF = OF =0
                            case "AND":
                                if (argums.Length == 2)
                                {
                                    ulong result = dst.GetValue() & src.GetValue();
                                    dst.SetValue(result);
                                    ClearRFlags(RFLAGS.OverflowFlag | RFLAGS.CarryFlag);
                                    UpdateRFlags(result, dst.Size, RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.ParityFlag);
                                    printRegisters();
                                    RIP++;
                                }
                                else throw new ArgumentException();
                                break;
                            // Sintaksa: OR DEST, SRC
                            // Bitsko OR ( 1 | 1 = 1, 1 | 0 = 1, 0 | 1 = 1, 0 | 0 = 0)
                            // Rezultat smiješta na DEST lokaciju
                            // Promjena flag-ova kod OR instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF = OF =0
                            case "OR":
                                if (argums.Length == 2)
                                {
                                    ulong result = dst.GetValue() | src.GetValue();
                                    dst.SetValue(result);
                                    ClearRFlags(RFLAGS.OverflowFlag | RFLAGS.CarryFlag);
                                    UpdateRFlags(result, dst.Size, RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.ParityFlag);
                                    printRegisters();
                                    RIP++;
                                }
                                else throw new ArgumentException();
                                break;
                            // Sintaksa: XOR DEST, SRC
                            // Bitsko XOR ( 1 ^ 1 = 0, 1 ^ 0 = 1, 0 ^ 1 = 1, 0 ^ 0 = 0)
                            // Rezultat smiješta na DEST lokaciju
                            // Promjena flag-ova kod XOR instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF = OF =0
                            // AF - nedefinisano
                            case "XOR":
                                if (argums.Length == 2)
                                {
                                    ulong result = dst.GetValue() ^ src.GetValue();
                                    dst.SetValue(result);
                                    ClearRFlags(RFLAGS.OverflowFlag | RFLAGS.CarryFlag);
                                    UpdateRFlags(result, dst.Size, RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.ParityFlag);
                                    printRegisters();
                                    RIP++;
                                }
                                else throw new ArgumentException();
                                break;
                                // Sintaksa: PUSH DEST
                                // Postavljanje vrijednosti veličine s u bajtovima na vrh steka
                                // na lokaciju RSP-s
                                // Ne utiče na flag-ove
                                case "PUSH":
                                if (argums.Length == 1)
                                {
                                    uint s = dst.Size;
                                    if (s == 2 || s == 4 || s == 8)
                                    { 
                                        Memory.WriteToMemory(RSP - s, s, dst.GetValue());
                                        printRegisters();
                                        printMemory();
                                        RSP.Value -= s;
                                    }
                                    else throw new ArgumentException();
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: POP DEST
                            // Skidanje vrijednosti veličine s u bajtovima sa vrha steka
                            // i postavljanje na destination lokaciju
                            // Ne utiče na flag-ove
                            case "POP":
                                if (argums.Length == 1 && dst.Type != ArgumentType.Immediate)
                                {
                                    uint s = dst.Size;
                                    if (s == 2 || s == 4 || s == 8)
                                    {
                                        dst.SetValue((ulong)Memory.ReadFromMemory(RSP, s));
                                        printRegisters();
                                        printMemory();
                                        RSP.Value += s;
                                    }
                                    else throw new ArgumentException();
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: PUSHA
                            //Postavljanje LowWord-a registara opšte namjene na stek
                            //Ne utiče na flag-ove
                            case "PUSHA":
                                if (argums.Length == 0)
                                {
                                    Memory.WriteToMemory(RSP - 2, 2, RAX.AX);
                                    Memory.WriteToMemory(RSP - 4, 2, RCX.CX);
                                    Memory.WriteToMemory(RSP - 6, 2, RDX.DX);
                                    Memory.WriteToMemory(RSP - 8, 2, RBX.BX);
                                    Memory.WriteToMemory(RSP - 10, 2, RSP.SP);
                                    Memory.WriteToMemory(RSP - 12, 2, RBP.BP);
                                    Memory.WriteToMemory(RSP - 14, 2, RSI.SI);
                                    Memory.WriteToMemory(RSP - 16, 2, RDI.DI);
                                    printRegisters();
                                    RSP.Value -= 16;

                                }
                                else throw new Exception();
                                RIP++;
                                break;
                            // Sintaksa: PUSHAD
                            //Postavljanje LowDoubleWord-a registara opšte namjene na stek
                            //Ne utiče na flag-ove
                            case "PUSHAD":
                                if (argums.Length == 0)
                                {
                                    Memory.WriteToMemory(RSP - 4, 4, RAX.EAX);
                                    Memory.WriteToMemory(RSP - 8, 4, RCX.ECX);
                                    Memory.WriteToMemory(RSP - 12, 4, RDX.EDX);
                                    Memory.WriteToMemory(RSP - 16, 4, RBX.EBX);
                                    Memory.WriteToMemory(RSP - 20, 4, RSP.ESP);
                                    Memory.WriteToMemory(RSP - 24, 4, RBP.EBP);
                                    Memory.WriteToMemory(RSP - 28, 4, RSI.ESI);
                                    Memory.WriteToMemory(RSP - 32, 4, RDI.EDI);
                                    printRegisters();
                                    RSP.Value -= 32;
                                }
                                else throw new Exception();
                                RIP++;
                                break;
                            // Sintaksa: POPA
                            //Skidanje LowWord-a registara opšte namjene sa steka
                            //Ne utiče na flag-ove
                            case "POPA":
                                if (argums.Length == 0)
                                {
                                    RDI.DI = (ushort) Memory.ReadFromMemory(RSP, 2);
                                    RSI.SI = (ushort) Memory.ReadFromMemory(RSP + 2, 2);
                                    RBP.BP = (ushort) Memory.ReadFromMemory(RSP + 4, 2);
                                    //RSP.SP = (ushort) Memory.ReadFromMemory(RSP + 6, 2);
                                    RBX.BX = (ushort) Memory.ReadFromMemory(RSP + 8, 2);
                                    RDX.DX = (ushort) Memory.ReadFromMemory(RSP + 10, 2);
                                    RCX.CX = (ushort) Memory.ReadFromMemory(RSP + 12, 2);
                                    RAX.AX = (ushort) Memory.ReadFromMemory(RSP + 14, 2);
                                    printRegisters();
                                    RSP.Value += 16;
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: POPAD
                            //Skidanje LowDoubleWord-a registara opšte namjene sa steka
                            //Ne utiče na flag-ove
                            case "POPAD":
                                if (argums.Length == 0)
                                { 
                                    RDI.EDI = (uint)Memory.ReadFromMemory(RSP, 4);
                                    RSI.ESI = (uint)Memory.ReadFromMemory(RSP + 4, 4);
                                    RBP.EBP = (uint)Memory.ReadFromMemory(RSP + 8, 4);
                                    //RSP.ESP = (uint)Memory.ReadFromMemory(RSP + 12, 4);
                                    RBX.EBX = (uint)Memory.ReadFromMemory(RSP + 16, 4);
                                    RDX.EDX = (uint)Memory.ReadFromMemory(RSP + 20, 4);
                                    RCX.ECX = (uint)Memory.ReadFromMemory(RSP + 24, 4);
                                    RAX.EAX = (uint)Memory.ReadFromMemory(RSP + 28, 4);
                                    printRegisters();
                                    RSP.Value += 32;
                                }
                                else throw new ArgumentException();
                                RIP++;
                                break;
                            // Sintaksa: STC
                            // Postavljanje carry flag-a
                            case "STC":
                                SetRFlags(RFLAGS.CarryFlag);
                                RIP++;
                                break;
                            // Sintaksa: STD
                            // Postavljanje direction flag-a
                            case "STD": 
                                SetRFlags(RFLAGS.DirectionFlag);
                                RIP++;
                                break;
                            // Sintaksa: CLC
                            // Brisanje carry flag-a
                            case "CLC": 
                                ClearRFlags(RFLAGS.CarryFlag);
                                RIP++;
                                break;
                            // Sintaksa: CLD
                            // Brisanje direction flag-a
                            case "CLD":
                                ClearRFlags(RFLAGS.DirectionFlag);
                                RIP++;
                                break;
                            // Sintaksa: CMP DEST, SRC
                            // Vrši samo promjenu flag-ova, poređenje vrši oduzimanjem operanada
                            // Promjena flag-ova kod CMP instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF=1 ako oduzimanje povlači posudjivanje sa niža 4 bita; Inače je CF=0
                            // OF=1 ako oduzimanje rezultuje arithmetic under/ overflow; Inače je OF=0
                            case "CMP":
                                UpdateRFlags((ulong)dst.GetValue() - (ulong)src.GetValue(), dst.Size, RFLAGS.OverflowFlag | RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.AdjustFlag | RFLAGS.ParityFlag | RFLAGS.CarryFlag);
                                RIP++;
                                printRegisters();
                                break;
                            // Sintaksa: TEST DEST, SRC
                            // Vrši samo promjenu flag-ova, koristi se u kombinaciji sa ulsovnim grananjem
                            // Promjena flag-ova kod TEST instrukcije:
                            // ZF=1 ako je rezultat 0; Inače je ZF=0
                            // SF=1 ako je rezultat negativan; Inače je SF=0
                            // PF=1 ako je rezultat paran u nižih 8 bita; Inače je PF=0
                            // CF = OF =0
                            // Test instrukcija vrši bitsko ANd nad operandima
                            case "TEST":
                                ClearRFlags(RFLAGS.OverflowFlag | RFLAGS.CarryFlag);
                                UpdateRFlags(dst.GetValue() & src.GetValue(), dst.Size, RFLAGS.SignFlag | RFLAGS.ZeroFlag | RFLAGS.ParityFlag);
                                RIP++;
                                printRegisters();
                                break;

                            default: throw new NotSupportedException("Instruction unknown or not supported.");
                        }

                    }
                }
            }
        }
        // Funkcija koja implementira rad sa skokovima (jumps)
        private void ExecuteJump(string mnemonic, ref string[] instructions, string label)
        {
            bool shouldJump = false;

            if (mnemonic == "JMP")
            {
                shouldJump = true;
            }
            else if (mnemonic == "JE" || mnemonic == "JZ")
            {
                // Skok ako je jednako ( tj. ZF=1)
                shouldJump = RFLAGS.HasFlag(RFLAGS.ZeroFlag);
            }
            else if (mnemonic == "JNE" || mnemonic == "JNZ")
            {
                // Skok ako nije jednako (tj. ZF=0)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.ZeroFlag);
            }
            else if (mnemonic == "JA" || mnemonic == "JNBE") 
            {
                // Skok ako nije manje ili jednako (tj. CF=0 i ZF=0)
                // Unsigned cmp
                shouldJump = !RFLAGS.HasFlag(RFLAGS.CarryFlag) && !RFLAGS.HasFlag(RFLAGS.ZeroFlag);
            }
            else if (mnemonic == "JB" || mnemonic == "JC" || mnemonic == "JNAE") 
            {
                // Skok ako nije veće ili jednako (tj. CF=1)
                // Unsigned cmp
                shouldJump = RFLAGS.HasFlag(RFLAGS.CarryFlag);
            }
            else if (mnemonic == "JNG" || mnemonic == "JLE") 
            {
                // Skok ako nije veće (tj. ZF=1 ili SF != OF)
                // Signed cmp 
                shouldJump = RFLAGS.HasFlag(RFLAGS.ZeroFlag) || (RFLAGS.HasFlag(RFLAGS.SignFlag) != RFLAGS.HasFlag(RFLAGS.OverflowFlag));
            }
            else if (mnemonic == "JNC" || mnemonic == "JNB" || mnemonic == "JAE") 
            {
                // Skok ako nema prenosa (tj. CF=0)
                // Unsigned cmp
                shouldJump = !RFLAGS.HasFlag(RFLAGS.CarryFlag);
            }
            else if (mnemonic == "JNA" || mnemonic == "JBE") 
            {
                // Skok ako nije iznad (tj. CF=1 ili ZF=1)
                // Unsigned cmp
                shouldJump = RFLAGS.HasFlag(RFLAGS.CarryFlag) || RFLAGS.HasFlag(RFLAGS.ZeroFlag);
            }
            else if (mnemonic == "JGE" || mnemonic == "JNL")  
            {
                // Skok ako je veće ili jednako (tj. SF=OF)
                // Signed cmp
                shouldJump = RFLAGS.HasFlag(RFLAGS.SignFlag) == RFLAGS.HasFlag(RFLAGS.OverflowFlag);
            }
            else if (mnemonic == "JG" || mnemonic == "JNLE")  
            {
                // Skok ako je veće (tj. ZF=0 i SF=OF)
                // Signed cmp
                shouldJump = !RFLAGS.HasFlag(RFLAGS.ZeroFlag) && (RFLAGS.HasFlag(RFLAGS.SignFlag) == RFLAGS.HasFlag(RFLAGS.OverflowFlag));
            }
            else if (mnemonic == "JL" || mnemonic == "JNGE")  
            {
                // Skok ako je manje (tj. SF != OF)
                // Signed cmp
                shouldJump = RFLAGS.HasFlag(RFLAGS.SignFlag) != RFLAGS.HasFlag(RFLAGS.OverflowFlag);
            }
            else if (mnemonic == "JECXZ")
            {
                // Skok ako je vrijednost RCX registra 0 
                shouldJump = RCX.Value == 0;
            }
            else if (mnemonic == "JCXZ")
            {
                // Skok ako je vrijednost CX registra 0
                shouldJump = RCX.CX == 0;
            }
            else if (mnemonic == "JNO")
            {
                // Skok ako nema prekoračenja (overflow) (tj. OF=0)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.OverflowFlag);
            }
            else if (mnemonic == "JNP" || mnemonic == "JPO")
            {
                // Skok ako je neparno (tj. PF=0)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.ParityFlag);
            }
            else if (mnemonic == "JNS")
            {
                // Skok ako je SF 0 (tj. SF=0)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.SignFlag);
            }
            else if (mnemonic == "JO")
            {
                // Skok ako je došlo do prekoračenja - overflow (tj. OF=1)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.OverflowFlag);
            }
            else if (mnemonic == "JP" || mnemonic == "JPE")
            {
                // Skok ako je parno (tj. PF=1)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.ParityFlag);
            }
            else if (mnemonic == "JS")
            {
                // Skok ako je SF setovan na 1 (tj. SF=1)
                shouldJump = !RFLAGS.HasFlag(RFLAGS.SignFlag);
            }
            else
            {
                throw new Exception("Neispravna JUMP instrukcija.");
            }

            if (shouldJump) GoTo(ref instructions, label);
            else RIP++;
        }

        /// <summary>
        /// Pokušaj skoka na sljedeću instrukciju fajla, nakon specificiraog broja linije line
        /// </summary>
        /// <param name="instructions">Niz svih instrukcija u .asm fajlu</param>
        /// <param name="line">Broj linije (počevši od 1) na koju treba skočiti</param>
        /// <returns>Da li je pronađena sljedeća instrukcija ili ne</returns>
        public bool GoTo(ref string[] instructions, int line)
        {
            for (int i = line; i < instructions.Length; i++)
            {
                string instruction = NormalizeInstruction(instructions[i]);
                // Preskakanje praznih linija i labela
                if (instruction.Length > 0 && !instruction.EndsWith(":")) 
                {
                    RIP = (uint)i + 1;
                    return true;
                }
            }
            RIP = (uint)line + 1;
            return false;
        }

        /// <summary>
        /// Pokušaj skoka na sljedeću instrukciju nakon specificirane labele
        /// </summary>
        /// <param name="instructions">Niz svih instrukcija u .asm fajlu</param>
        /// <param name="label">Labela na koju treba skočiti</param>
        /// <returns>Da li je sljedeća instrukcija (nakon specificirane labele) pronađena ili ne</returns>
        public bool GoTo(ref string[] instructions, string label)
        {
            bool foundLabel = false;
            int labelIndex = 0;
            for (; labelIndex < instructions.Length; labelIndex++)
            {
                if (NormalizeInstruction(instructions[labelIndex]).Equals(label + ":"))
                {
                    foundLabel = true;
                    break;
                }
            }

            if (foundLabel)
            {
                return GoTo(ref instructions, labelIndex + 1);
            }
            else return false;
        }

        /// <summary>
        /// Resetovanje vrijednosti registara, flag-ova i memorije
        /// </summary>
        public void Reset()
        {
            RAX.Value = 0;
            RBX.Value = 0;
            RCX.Value = 0;
            RDX.Value = 0;
            RSI.Value = 0;
            RDI.Value = 0;
            RSP.Value = (ulong)Memory.Length;
            RBP.Value = 0;
            RIP = 1;
            RFLAGS = RFLAGS.ZeroFlag;
            Memory = new byte[Memory.Length];
        }
        #endregion
        // Funkcija koja uklanja komentare i nepotrebne whitespace-e
        // iz tekuće linije koda i vraća tako modifikovan string kao instrukciju
        private string NormalizeInstruction(string instruction)
        {
            // Uklanjanje komentara iz tekuće instrukcije
            int commentIndex = instruction.IndexOf(';');
            if (commentIndex > -1)
            {
                instruction = instruction.Remove(commentIndex);
            }

            // Konverzija u uppercase i uklanjanje nepotrebnih whitespace-a
            instruction = Regex.Replace(instruction.ToUpperInvariant(), @"\s+", " ").Trim();

            // Provjera nedozvoljenih karaktera
            if (Regex.IsMatch(instruction, @"[!@#$%^&()={}\\|/?<>.~`""'_]"))
            {
                throw new InvalidOperationException(string.Format("Nedozvoljen karakter u {0} redu .asm fajla.", RIP));
            }

            return instruction;
        }
    }
}
