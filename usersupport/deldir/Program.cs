using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using CommonSupportLib;

namespace deldir
{
   class Program
   {
      static void Main(string[] args)
      {
         SupportHelper.SetupPath();
         using (REngine engine = REngine.CreateInstance("RDotNet"))
         {
            engine.Initialize();
            //DoTest(engine);
            ReproDiscussion532760(engine);
         }
      }

      private static void DoTest(REngine engine)
      {
         var setupStr = @"library(deldir)
set.seed(421)
x <- runif(20)
y <- runif(20)
z <- deldir(x,y)
w <- tile.list(z)

z <- deldir(x,y,rw=c(0,1,0,1))
w <- tile.list(z)

z <- deldir(x,y,rw=c(0,1,0,1),dpl=list(ndx=2,ndy=2))
w <- tile.list(z)
";

         engine.Evaluate(setupStr);
         var res = new List<List<Tuple<double, double>>>();
         var n = engine.Evaluate("length(w)").AsInteger()[0];
         for (int i = 1; i <= n; i++)
         {
            var x = engine.Evaluate("w[[" + i + "]]$x").AsNumeric().ToArray();
            var y = engine.Evaluate("w[[" + i + "]]$y").AsNumeric().ToArray();
            var t = x.Zip(y, (first, second) => Tuple.Create(first, second)).ToList();
            res.Add(t);
         }
      }

      private static void ReproIssue77(REngine engine)
      {
         object expr = engine.Evaluate("function(k) substitute(bar(x) = k)");
         Console.WriteLine(expr ?? "null");
      }

      private static void ReproDiscussion528955(REngine engine)
      {
         engine.Evaluate("a <- 1");
         engine.Evaluate("a <- a+1");
         NumericVector v1 = engine.GetSymbol("a").AsNumeric();
         bool eq = 2.0 == v1[0];
         engine.Evaluate("a <- a+1");
         NumericVector v2 = engine.GetSymbol("a").AsNumeric();
         eq = 3.0 == v2[0];
      }

      private static void ReproDiscussion532760(REngine engine)
      {
         // https://rdotnet.codeplex.com/discussions/532760
         //> x <- data.frame(1:1e6, row.names=format(1:1e6))
         //> object.size(x)
         //60000672 bytes
         //> object.size(rownames(x))
         //56000040 bytes

         engine.Evaluate("x <- data.frame(1:1e6, row.names=format(1:1e6))");
         var x = engine.GetSymbol("x").AsDataFrame();
         engine.ForceGarbageCollection();
         engine.ForceGarbageCollection();
         var memoryInitial = engine.Evaluate("memory.size()").AsNumeric().First();

         var netMemBefore = GC.GetTotalMemory(true);

         var blah = x.RowNames;
         //var blah = engine.Evaluate("rownames(x)").AsCharacter().ToArray();
         blah = null;

         GC.Collect();
         engine.ForceGarbageCollection();
         engine.ForceGarbageCollection();
         var memoryAfterAlloc = engine.Evaluate("memory.size()").AsNumeric().First();

         var netMemAfter = GC.GetTotalMemory(false);

      }

      /*
1.5.5
		         //var blah = x.RowNames;
		memoryInitial	50.16	double
		netMemBefore	87860	long
		blah	null	string[]
		memoryAfterAlloc	62.08	double
		netMemAfter	0	long

		
         var blah = engine.Evaluate("rownames(x)").AsCharacter().ToArray();
		memoryInitial	50.15	double
		netMemBefore	87948	long
		blah	null	string[]
		memoryAfterAlloc	65.95	double
		netMemAfter	0	long

		
		
		1.5.6, not waiting for final total memory (otherwise hangs, as with above)
		
		memoryInitial	50.16	double
		netMemBefore	88608	long
		blah	null	string[]
		memoryAfterAlloc	65.95	double
		netMemAfter	98738480	long

		
		memoryInitial	50.16	double
		netMemBefore	88572	long
		blah	null	string[]
		memoryAfterAlloc	62.08	double
		netMemAfter	99319384	long

		
		       */
   }
}
