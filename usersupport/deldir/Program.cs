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
            ReproDiscussion528955(engine);
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

   }
}
