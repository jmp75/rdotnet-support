using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressTest
{
   class Program
   {
      static void Main(string[] args)
      {
         var e = REngine.GetInstance();
         e.Initialize();
         int n = 200000;
         var memBefore = GC.GetTotalMemory(true);
         var sexpressions = new SymbolicExpression[n];
         for (int i = 0; i < n; i++)
         {
            sexpressions[i] = e.Evaluate("'abc'");
         }
         var mem2 = GC.GetTotalMemory(true);
         sexpressions = null;
         GC.Collect();
         GC.Collect();
         var mem3 = GC.GetTotalMemory(true);
      }
   }
}
