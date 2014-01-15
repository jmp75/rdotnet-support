using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;

namespace deldir
{
   class Program
   {
      static void Main(string[] args)
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

         SetupPath();
         using (REngine engine = REngine.CreateInstance("RDotNet"))
         {
            engine.Initialize();
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
      }

      public static void SetupPath()
      {
         var oldPath = System.Environment.GetEnvironmentVariable("PATH");

         var rPath = System.Environment.Is64BitProcess ? @"C:\Program Files\R\R-3.0.2\bin\x64" : @"C:\Program Files\R\R-3.0.2\bin\i386";

         if (Directory.Exists(rPath) == false)
            throw new DirectoryNotFoundException(string.Format("Could not found the specified path to the directory containing R.dll: {0}", rPath));

         var newPath = string.Format("{0}{1}{2}", rPath, System.IO.Path.PathSeparator, oldPath);
         System.Environment.SetEnvironmentVariable("PATH", newPath);
      }

   }
}
