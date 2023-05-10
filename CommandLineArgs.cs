using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Interpreter
{
    public partial class Interpreter
    {
        // Funkcija za provjeru argument komandne linije
        // koja zatim pokreće interpretaciju .asm fajla ukoliko je
        // pravilno proslijeđen .asm fajl
        public bool CheckCommandLineArgs(string[] args)
        {
            // Postavljanje apsolutne putanje do .asm fajla
            if (File.Exists(args[0]))
            {
                if (!args[0].EndsWith(".asm"))
                {
                    Console.WriteLine("Ulazni fajl nije .asm fajl.");
                    return false;
                }

                if (args[0].Contains('\\'))
                {
                    ASMFilePath = args[0];
                }
                else
                {
                    ASMFilePath += "\\" + args[0];
                }
            }
            else
            {
                Console.WriteLine($"Fajl {args[0]} ne postoji.");
                return false;
            }

            if (args != null)
            {
                    Interpret_asm();
                    return true;
                
            }
            // Terminiranje izvršavanja
            else
            {
                return false;
            }
        }
    }
}
