using RDotNet;
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
            REngine.SetEnvironmentVariables();
            using (REngine e = REngine.GetInstance())
            {
                ReproStackOverflow_34355201(e);
            }
            //ReproDiscussion435478();
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
