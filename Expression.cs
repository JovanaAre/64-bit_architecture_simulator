using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ASM_Interpreter
{
    // Klasa za rad sa izrazima
    public class Expression
    {   
        // Regex-i za odgovarajuće konverzije proslijeđenog izraza (argumenta)
        private static readonly Regex hexidecimalRegex = new Regex(@"([a-f0-9]*)h", RegexOptions.Compiled);
        private static readonly Regex binaryRegex = new Regex(@"([0-1]*)b", RegexOptions.Compiled);
        private static readonly Regex multiplyRegex = new Regex(@"(-?\d+\.?\d*)\*(-?\d+\.?\d*)", RegexOptions.Compiled);
        private static readonly Regex addRegex = new Regex(@"(-?\d+\.?\d*)\+(-?\d+\.?\d*)", RegexOptions.Compiled);
        private static readonly Regex subtractRegex = new Regex(@"(-?\d+\.?\d*)-(-?\d+\.?\d*)", RegexOptions.Compiled);

        /// <summary>
        /// Izračunava specificiranu instrukciju i vraća numerički rezultat
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static ulong Evaluate(string expr)
        {
            // Uklanja sve whitespace-e iz izraza
            expr = Regex.Replace(expr.ToLowerInvariant(), @"\s+", string.Empty);

            // Konvertuje binarni sadržaj u decimalni
            expr = Do(binaryRegex, expr, (x) => (Convert.ToUInt64(x.Groups[1].Value, 2)).ToString());

            // Konvertuje heksadecimalni sadržaj u decimalni
            expr = Do(hexidecimalRegex, expr, (x) => (Convert.ToUInt64(x.Groups[1].Value, 16)).ToString());

            // Računa index * scale
            expr = Do(multiplyRegex, expr, (x) => (Convert.ToUInt64(x.Groups[1].Value) * Convert.ToUInt32(x.Groups[2].Value)).ToString());

            // Računa bilo kakvo dodavanje u izrazu ukoliko ga ima
            expr = Do(addRegex, expr, (x) => (Convert.ToUInt64(x.Groups[1].Value) + Convert.ToUInt32(x.Groups[2].Value)).ToString());

            // Računa bilo kakvo oduzimanje u izrazu ukoliko ga ima
            expr = Do(subtractRegex, expr, (x) => (Convert.ToUInt64(x.Groups[1].Value) - Convert.ToUInt32(x.Groups[2].Value)).ToString());

            //konvertovanje izraza u ulong vrijednost
            return Convert.ToUInt64(expr);
        }
        // Funkcija koja vraća string dobijen izračunavanjem proslijeđenog izraza
        // na osnovu odgovarajućeg Regex-a i formule za izračunavanje koja odgovara
        // logici proslijeđenog Regex-a
        private static string Do(Regex regex, string formula, Func<Match, string> func)
        {
            MatchCollection collection = regex.Matches(formula);
            if (collection.Count == 0) return formula;
            for (int i = 0; i < collection.Count; i++) formula = formula.Replace(collection[i].Groups[0].Value, func(collection[i]));
            return Do(regex, formula, func);
        }
    }
}
