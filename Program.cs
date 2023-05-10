using System;

namespace ASM_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {   //Instanciramo jedan objekat klase Inerpreter
            Interpreter interpreter = new Interpreter();
            try
            {
                bool a = interpreter.CheckCommandLineArgs(args);
              
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main exception:");
                Console.WriteLine(ex.Message);
            }

        }
    }
}
