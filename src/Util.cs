using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Interpreter
{   
    // Klasa koja implementira pomoćne metode za rad sa memorijom
    public static class Util
    {   
        // Funkcija koja provjerava da li data pozicija ulazi u opseg memorije
        // Kao i data pozicija + veličina podatka kako bi upis ili čitanje bili mogući ili ne
        private static bool CheckMemoryBounds(UInt64 position, UInt64 size, UInt64 arrayLimit)
        {
            return position >= arrayLimit || position + size >= arrayLimit;
        }
        // Funkcija koja čita i vraća podatak veličine size, sa lokacije position u memoriji
        public static UInt64 ReadFromMemory(this byte[] array, UInt64 position, UInt64 size)
        {
            var output = 0;

            // Provjera maksimalnog opsega memorije
            if (CheckMemoryBounds(position, size, (UInt64)array.Length))
            {
                Console.WriteLine("Pristup izvan opsega memorije.");
                return (UInt64)(output);
            }

            for (var i = (int)size - 1; i >= 0; --i)
            {
                output = (output << 8) | array[(int)position + i];
            }

            return (UInt64)(output);
        }
        // Funkcija koja upisuje podatak input veličine size u memoriju na lokaciju position
        public static bool WriteToMemory(this byte[] array, UInt64 position, UInt64 size, UInt64 input)
        {
            // Provjera maksimalnog opsega memorije
            if (CheckMemoryBounds(position, size, (UInt64)array.Length))
            {
                return false;
            }

            for (var i = 0; i < (int)size; ++i)
            {
                array[(int)position + i] = (byte)input;
                input >>= 8;
            }

            return true;
        }
        // Funkcija koja upisuje string input na lokaciju position u memoriji
        public static bool WriteStringToMemory(this byte[] array, UInt64 position, string input)
        {
            // Provjera maksimalnog opsega memorije
            if (CheckMemoryBounds(position, (UInt64)input.Length + 1, (UInt64)array.Length))
            {
                return false;
            }

            // Upis svih karaktera
            for (var i = 0; i < input.Length; ++i)
            {
                array[(int)position + i] = (byte)input[i];
            }

            // Za posljednji karakter stringa postavljamo znak za terminaciju stringa
            array[(int)position + input.Length] = 0;

            return true;
        }
        // Funkcija koja čita string maksimalne veličine maxSize sa lokacije position
        // u memoriji i smiješta ga u promjenljivu output
        public static bool ReadStringFromMemory(this byte[] array, UInt64 position, UInt64 maxSize, out string output)
        {
            var cString = new StringBuilder();

            // Čitanje stringa dok se ne dostigne znak za terminaciju ili
            // maksimalna veličina memorije
            for (; ; ++position)
            {
                if (position >= (UInt64)array.Length)
                {
                    output = null;
                    return false;
                }

                if (cString.Length == (int)maxSize)
                {
                    break;
                }
                else if (array[position] != 0)
                {
                    cString.Append((char)array[position]);
                }
                else
                {
                    break;
                }
            }

            output = cString.ToString();
            return true;
        }

    }
}
