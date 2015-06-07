using RDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mycsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdatePathEnvVar();
            CppInterop interop = new CppInterop(initializeR:true);

            // Using PInvoke only
            int[] myInt = interop.CreateArray();

            // If using R.NET classes, SEXP can be directly manipulated.
            IntegerVector nv = interop.ProcessToR(new[] { 1, 2, 3, 4, 5, 4, 3, 4, 5, 6 });
            int blah = interop.ProcessNumVec(nv);
        }

        private static void UpdatePathEnvVar()
        {
            // this is hard coded to update the paths to add the location of R.dll and mycpp.dll to the PATH env var.
            // You may need to adapt to your setup if e.g. you have Windows 32 bits
            REngine.SetEnvironmentVariables(); // will usually find path to R.dll on Windows
            //\tmp\mycsharp\bin\Debug
            var d = Environment.CurrentDirectory;
            string nativeDllDir = Path.Combine(d, @"..\..\..\x64\Debug");
            if (!Directory.Exists(nativeDllDir)) throw new Exception("Directory not found: " + nativeDllDir);
            UpdatePath(nativeDllDir);
        }

        private static void UpdatePath(string nativeDllDir)
        {
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + 
                Path.PathSeparator + nativeDllDir);
        }
    }
}
