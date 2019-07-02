using RDotNet;
using RDotNet.NativeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            string rPath = "C:\\Progra~1\\R\\R-36~1.0\\bin\\x64";
            string rHome = "C:\\Progra~1\\R\\R-36~1.0";
            //rPath = "C:\\Progra~1\\R\\R-35~1.2\\bin\\x64";
            //rHome = "C:\\Progra~1\\R\\R-35~1.2";
            //rPath = "C:\\Progra~1\\R\\R-34~1.4\\bin\\x64";
            //rHome = "C:\\Progra~1\\R\\R-34~1.4";
            //rPath = "C:\\Progra~1\\R\\R-34~1.4\\bin\\i386";
            //rHome = "C:\\Progra~1\\R\\R-34~1.4";
            //rPath = "C:\\Progra~1\\R\\R-36~1.0\\bin\\i386";
            //rHome = "C:\\Progra~1\\R\\R-36~1.0";
            REngine.SetEnvironmentVariables(rPath, rHome);
            var log = NativeUtility.SetEnvironmentVariablesLog;
            Console.WriteLine("********************************");
            Console.WriteLine(log);
            Console.WriteLine("********************************");
            using (REngine e = REngine.GetInstance())
            {
                ReproGH99(e);
                //ReproAutoPrint(e);
            }
            //ReproDiscussion435478();
        }

        private static void ReproAutoPrint(REngine engine)
        {
            engine.Initialize();
            SymbolicExpression expression;
            engine.AutoPrint = false;

            Console.WriteLine("engine.AutoPrint = false");

            Console.WriteLine("In: mtc <-head(mtcars)");
            engine.Evaluate("mtc <-head(mtcars)");
            Console.WriteLine("In: a <- mtc ");
            engine.Evaluate("a <- mtc ");
            Console.WriteLine("In: mtc");
            engine.Evaluate("mtc");
            Console.WriteLine("In: print(mtc)");
            engine.Evaluate("print(mtc)");

            engine.AutoPrint = true;
            Console.WriteLine("engine.AutoPrint = true");

            Console.WriteLine("In: mtc <-head(mtcars)");
            engine.Evaluate("mtc <-head(mtcars)");
            Console.WriteLine("In: a <- mtc ");
            engine.Evaluate("a <- mtc ");
            Console.WriteLine("In: mtc");
            engine.Evaluate("mtc");
            Console.WriteLine("In: print(mtc)");
            engine.Evaluate("print(mtc)");

        }

        private static void ReproGH99(REngine engine)
        {
            // https://github.com/jmp75/rdotnet/issues/99
            SymbolicExpression expression;
            engine.Initialize();
            engine.AutoPrint = true;

            engine.Evaluate("install.packages('climatol')");
            engine.Evaluate("library(climatol)");

            expression = engine.GetSymbol("x");
            Console.WriteLine("Values as characters:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("y");
            Console.WriteLine("Values as factor:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("c1");
            Console.WriteLine("Values direct from column:");
            Console.WriteLine(expression.AsCharacter()[0]);
            Console.WriteLine(expression.AsCharacter()[1]);
            Console.WriteLine("*********************");

            Console.WriteLine("");
            Console.WriteLine("*********************");
            Console.WriteLine("Now going for a second round");
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("x");
            Console.WriteLine("Values as characters:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("y");
            Console.WriteLine("Values as factor:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("c1");
            Console.WriteLine("Values direct from column:");
            Console.WriteLine(expression.AsCharacter()[0]);
            Console.WriteLine(expression.AsCharacter()[1]);
            Console.WriteLine("*********************");


        }


        private static void ReproGH97(REngine engine)
        {
            // https://github.com/jmp75/rdotnet/issues/97
            SymbolicExpression expression;
            engine.Initialize();
            engine.Evaluate("x <- data.frame(c1 = c('a', 'b'), stringsAsFactors = FALSE)");
            engine.Evaluate("y <- data.frame(x = c('a', 'b'), stringsAsFactors = TRUE)");
            engine.Evaluate("c1 <- x$c1");

            expression = engine.GetSymbol("x");
            Console.WriteLine("Values as characters:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("y");
            Console.WriteLine("Values as factor:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("c1");
            Console.WriteLine("Values direct from column:");
            Console.WriteLine(expression.AsCharacter()[0]);
            Console.WriteLine(expression.AsCharacter()[1]);
            Console.WriteLine("*********************");

            Console.WriteLine("");
            Console.WriteLine("*********************");
            Console.WriteLine("Now going for a second round");
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("x");
            Console.WriteLine("Values as characters:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("y");
            Console.WriteLine("Values as factor:");
            Console.WriteLine(expression.AsDataFrame()[0][0]);
            Console.WriteLine(expression.AsDataFrame()[0][1]);
            Console.WriteLine("*********************");
            expression = engine.GetSymbol("c1");
            Console.WriteLine("Values direct from column:");
            Console.WriteLine(expression.AsCharacter()[0]);
            Console.WriteLine(expression.AsCharacter()[1]);
            Console.WriteLine("*********************");


        }


        private static void ReproIssue169(REngine engine)
        {
            engine.Evaluate("library(mirt)");
            engine.Evaluate("x=mirt(Science,1)");
            S4Object obj111 = engine.GetSymbol("x").AsS4();
            engine.Evaluate("ff=fscores(x, response.pattern=c(1,0,0,0))");
            GenericVector dataset111 = engine.GetSymbol("ff").AsList();
            NumericVector v = dataset111[0].AsNumeric();
            double firstval = v[0];
        }

        private static void ReproStackOverflow_34355201(REngine engine)
        {
            engine.AutoPrint = true;
            //samples taken from ?fscores man page in package mirt
            engine.Evaluate("library(mirt)");
            // 'Science' is a prepackage sample data in mirt; you can use 'engine.CreateDataFrame' in C# to create your own if need be.
            engine.Evaluate("mod <- mirt(Science, 1)");
            engine.Evaluate("class(mod)");
            S4Object modcs = engine.GetSymbol("mod").AsS4();

            // TODO - noticed 2015-12 that R.NET 1.6.5, HasSlot causes a stack imbalance warning. To unit test.
            // Normally should do:
            // if (modcs.HasSlot("Fit"))

            IDictionary<string, string> slotTypes = modcs.GetSlotTypes();
            if (slotTypes.Keys.Contains("Fit"))
            {
                GenericVector fit = modcs["Fit"].AsList();
                // should check logLik in fit.Names;
                double logLik = fit["logLik"].AsNumeric()[0];
            }
            engine.Evaluate("tabscores <- fscores(mod, full.scores = FALSE)");
            engine.Evaluate("head(tabscores)");
            engine.Evaluate("class(tabscores)");
            NumericMatrix tabscorescs = engine.GetSymbol("tabscores").AsNumericMatrix();
        }

        private static void ReproDiscussion435478()
        {
            REngine.SetEnvironmentVariables(rPath: null, rHome: @"C:\Program Files\Dell");//@"c:\Program Files\R\R-3.1.0");
            REngine engine = REngine.GetInstance();
            engine.Evaluate("library('RODBC')");
            var connect = engine.Evaluate("RODBC::odbcConnect").AsFunction();
            engine.Dispose();
        }

        private static void ReproGraph2D()
        {
            double[] x = new[] { 1.0, 4, 3, 5, 8 };
            double[,] y = new double[,] {
                { 1.0, 4, 3, 5, 8 },
                { 5.0, 4, 2, 7, 9 }
            };
            plotGraphR_2D(x, y);
        }

        public static void plotGraphR_2D(IEnumerable<double> x, double[,] y)
        {
            //string Rpath = @"C:\Program Files\R\R-3.1.0\bin\x64";

            //REngine.SetEnvironmentVariables(Rpath);
            REngine engine = REngine.GetInstance();

            var v1 = engine.CreateNumericVector(x);
            var v2 = engine.CreateNumericMatrix(y);

            if (engine.IsRunning == false)
            {
                engine.Initialize();
            }

            engine.SetSymbol("v1", v1);
            engine.SetSymbol("v2", v2);

            engine.Evaluate("require('ggplot2')");
            engine.Evaluate("library('ggplot2')");
            engine.Evaluate("my_data <- data.frame(v2)");
            engine.Evaluate("colnames(my_data) <- c('Price', 'Quantity')");
            //engine.Evaluate("myChart <- ggplot() + geom_line(data = my_data, my_data$Price)"); // THIS DOESN'T WORK
            engine.Evaluate("myChart <- ggplot(my_data, aes(x=Price, y=Quantity)) + geom_line()");
            engine.Evaluate("print(myChart)");


            //engine.Evaluate("plot(my_data$Price)"); // THIS WORKS
        }
    }
}
