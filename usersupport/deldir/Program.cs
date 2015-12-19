using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using CommonSupportLib;
using System.Collections;
using System.Threading;
using System.Reflection;
using RDotNet.Devices;

namespace deldir
{
   class Program
   {
      static void Main(string[] args)
      {
         REngine.SetEnvironmentVariables();
         using (REngine e = REngine.GetInstance())
         {
            //DoTest(engine);
            //ReproWorkitem45(engine);
            //ReproWorkitem22(engine);

            ReproDiscussion30824095(e);
            //ReproInMemoryDataFrameCreation(e);
            //ReproMultipleAppDomains(e);
            //ReproDiscussion540017(e);
            //ReproDiscussion539094(engine);
            //ReproDiscussion537259(engine);
            //ReproDiscussion539094(e);
            //ReproDiscussion537259(e);
            //ReproMultipleAppDomains(e);
            //TestMultiThreads(engine);
         }
      }

      private static void ReproDiscussion30824095(REngine e)
      {
          e.Evaluate("library(cluster)");
          e.Evaluate("library(rrcov)");

          // plot from R
          //to show outlier with method : classic & robust Mve 

          e.Evaluate("n <- 100 ; spread <- (n/2 - abs(1:n - n/2))/n * 4");
          e.Evaluate("X <- data.frame(1:n + spread * rnorm(n), 2 * 1:n + spread * rnorm(n))");

          int xAxis = 1;
          int yAxis = 2;
          e.Evaluate("x<-X[," + xAxis + "] ");
          e.Evaluate("y<-X[," + yAxis + "] ");
          e.Evaluate("shape <- cov(X)");
          e.Evaluate("center<- colMeans(X)");
          e.Evaluate("d2.95 <- qchisq(0.95, df = 2)");
          //e.Evaluate("gr<- grid(lty=3,col='lightgray', equilogs = 'TRUE')");
          //dataform.rconn.Evaluate("mtext('with classical (red) and robust (blue)')");
          e.Evaluate("plot(x,y, main='Draw Ellipse ', pch=19,col='black', type='p')");
          e.Evaluate("elp<- unname(ellipsoidPoints(shape, d2.95,center))");
          e.Evaluate(" lines(elp, col='red' , lty=7 , lwd=2)");
          //e.Evaluate("lines(e)");
          //e.Evaluate("lines(ellipsoidPoints(mve@cov, d2 = d2.95, loc=mve@center), col='blue', lty='7' , lwd='2') ");
          // axGraphicsDevice2.RemoveFromConnector();
          e.Evaluate("dev.off()");
      }

      // https://rdotnet.codeplex.com/discussions/569196
      private static void ReproDiscussion569196(REngine engine)
      {
         // UserReported(engine);         
         engine.Evaluate("library(forecast)");
         engine.Evaluate("snaiveRes <- snaive(wineind)");
         engine.Evaluate("str(snaiveRes)");
         
      }

      private static DataFrame UserReported(REngine engine)
      {
         // Incomplete data and repro info.
         // See https://rdotnet.codeplex.com/discussions/569196
         NumericVector PreprocessedValue = null;
         DataFrame PredictedData = null;

         // Some info was missing. Make up.
         string StartDate = "2001-01-01";
         double[] PreProcessedList = new[] { 1.1, 7.3, 4.5, 7.4, 11.23, 985.44 };
         string days = "2"; string interval = "3";


         PreprocessedValue = engine.CreateNumericVector(PreProcessedList);
         // Assign the Utilization value to R variable UtilValue
         engine.SetSymbol("PreprocessedValue", PreprocessedValue);
         engine.Evaluate("library(forecast)");
         engine.Evaluate("StartDate <- as.Date('" + StartDate + "') + " + days);
         engine.Evaluate("size = length(seq(from=as.Date('" + StartDate + "'), by='" + "day" + "', to=as.Date(StartDate)))");
         engine.Evaluate("startDate <- as.POSIXct('" + StartDate + "')");
         engine.Evaluate("endDate <- StartDate + as.difftime(size, units='" + "days" + "')");
         engine.Evaluate("PredictDate = seq(from=StartDate, by=" + interval + "*60, to=endDate)");
         engine.Evaluate("freq <- ts(PreprocessedValue, frequency = 20)");
         engine.Evaluate("forecastnavie <-snaive(freq, Datapoints)");
         engine.Evaluate("PredictValue = (forecastnavie$mean)");
         engine.Evaluate("PredictedData = cbind(PredictValue, data.frame(PredictDate))");
         PredictedData = engine.Evaluate("PredictedData").AsDataFrame();
         return PredictedData;
      }


      private static void ReproInMemoryDataFrameCreation(REngine e)
      {

         e.Evaluate("f <- function(a) {if (length(a)!= 1) stop('What goes on?')}");
         var f = e.Evaluate("f").AsFunction();
         try
         {
            e.Evaluate("f(letters[1:3])");
         }
         catch (EvaluationException)
         {
         }
         f.Invoke(e.CreateCharacterVector(new[] { "blah" }));
         f.Invoke(e.CreateCharacterVector(new[] { "blah", "blah" }));

         // IEnumerable[] columns, string[] columnNames = null, string[] rowNames = null, bool checkRows = false, bool checkNames = true, bool stringsAsFactors = true);
         var columns = new[] {
            new[]{1,2,3,4,5},
            new[]{1,2,3,4,5},
            new[]{1,2,3,4,5}
         };
         var df = e.CreateDataFrame(columns, new[] { "a", "b", "c" });
         columns[1] = new[] { 1, 2, 3 };
         object blah;
         try
         {
            df = e.CreateDataFrame(columns, new[] { "a", "b", "c" });
            blah = df[1, 1];
         }
         catch
         {
         }
         df = e.CreateDataFrame(columns, new[] { "a", "b", "c" });
         blah = df[1, 1];
      }

      private static void ReproDiscussion540017(REngine e)
      {
          e.Evaluate("library(sn)");
          e.Evaluate("data(wines, package='sn')");
          e.Evaluate("X <- model.matrix(~ phenols + wine, data=wines)");
          e.Evaluate("fit <- msn.mle(x=X, y=cbind(wines$acidity, wines$alcohol), opt.method='BFGS')");
          var beta = e.Evaluate("fit$dp$beta").AsNumericMatrix();
          var value = beta[0,1];
          var betaNames = e.Evaluate("rownames(fit$dp$beta)").AsCharacter().ToArray();
      }

      public class Job : MarshalByRefObject
      {

         // uses R.NET here
         public void Execute()
         {
            var engine = InitREngine();
            engine.Evaluate("x <- 5");
         }

         // initializes REngine
         private REngine InitREngine()
         {
            var engine = REngine.GetInstance(initialize: false);
            engine.Initialize(null, new NullCharacterDevice());  // real char device?
            AppDomain.CurrentDomain.DomainUnload += (EventHandler)((o, e) => engine.Dispose());
            return engine;
         }
      }

       private static void ReproMultipleAppDomains(REngine e)
       {
          TestAppDomain("test1");  // works
          TestAppDomain("test2");  // hangs at the last line in Job.Execute()
       }
       private static void TestAppDomain(string jobName)
       {
          var domainSetup = new AppDomainSetup();
          domainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
          AppDomain ad = AppDomain.CreateDomain(jobName, AppDomain.CurrentDomain.Evidence, domainSetup);

          var type = typeof(Job);
          var jd = (Job)ad.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true, BindingFlags.CreateInstance, null,
                              new object[] { }, null, null);
          jd.Execute();
          AppDomain.Unload(ad);
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

      private static void ReproWorkitem41(REngine engine)
      {
         var fname = "c:/tmp/rgraph.png";
         engine.Evaluate("library(ggplot2)");
         engine.Evaluate("library(scales)");
         engine.Evaluate("library(plyr)");
         engine.Evaluate("d <- data.frame( x = rnorm(1000), y = rnorm(1000), z = rnorm(1000))");
         engine.Evaluate("p <- ggplot(d, aes(x = x, y = y, color = z)) + geom_point(size=4.5, shape=19)");
         // Use:
         engine.Evaluate("png('" + fname + "')");
         engine.Evaluate("print(p)");
         // but not:
         // engine.Evaluate("p");
         // engine.Evaluate("dev.copy(png, '" + fname + "')");
         // the statement engine.Evaluate("p") does not behave the same as p (or print(p)) directly in the R console.
         engine.Evaluate("dev.off()");
      }

      private static void ReproTreeEnquiry(REngine e)
      {
         e.Evaluate("library(tree)");
         var irtr = e.Evaluate("ir.tr <- tree(Species ~., iris)");
         // the following will print a human readable tree to the console output
         e.Evaluate("print(ir.tr)");
         var aList = irtr.AsList(); // May work only with the latest dev code
         // for R.NET 1.5.5 you may need to do instead:
         aList = e.Evaluate("as.list(tree(Species ~., iris))").AsList();
         var theDataFrame = aList[0].AsDataFrame();
         // Further processing of theDataFrame, etc.
      }

      private static void ReproDiscussion539094(REngine e)
      {
         e.Evaluate("library(Rcpp)");
         e.Evaluate("setwd('c:/tmp')");
         e.Evaluate("sourceCpp('fibonacci.cpp')");
         var x = e.Evaluate("fibonacci(7)").AsNumeric();
      }

      private static void ReproDiscussion537259(REngine e)
      {
         e.Evaluate("library(rJava)");
         e.Evaluate(".jinit()");
         e.Evaluate("f <- .jnew('java/awt/Frame','Hello')");
         e.Evaluate(".jcall(f,,'setVisible',TRUE)");
      }

      private static void ReproWorkitem22(REngine engine)
      {
         var statements = @"
parameters <- data.frame(name=paste('x', 1:4, sep=''), 
  min = c(1, -10, 1, 0.5), 
  value= c(700, 0, 100, 2), 
  max = c(1500, 5, 500, 4), 
  stringsAsFactors=FALSE)

f <- function(i, p) {
  runif(1, p[i,'min'], p[i,'max'])
}

";
         var df = engine.Evaluate(statements);
         var rndParams = "parameters$value <- as.numeric(lapply(1:nrow(parameters), FUN=f, parameters))";
         engine.Evaluate("set.seed(0)");
         for (int i = 0; i < 1000; i++)
         {
            engine.Evaluate(rndParams);
            //engine.Evaluate("print(parameters)");
            GetDataFrame("parameters", engine);
         }
      }

      private static void GetDataFrame(string sexpression, REngine engine)
      {
         var dataFrame = engine.Evaluate(sexpression).AsDataFrame();
         SimpleHyperCube s = convert(dataFrame);
      }

      private static SimpleHyperCube convert(DataFrame dataFrame)
      {
         dynamic df = dataFrame;
         object[] colnames = Enumerable.ToArray(SymbolicExpressionExtension.AsVector(df.names));
         object[] tmp = Enumerable.ToArray(SymbolicExpressionExtension.AsVector(df.name));
         var varnames = Array.ConvertAll(tmp, x => (string)x);
         SimpleHyperCube s = new SimpleHyperCube(varnames);
         var rows = dataFrame.GetRows();
         foreach (var vn in varnames)
         {
            dynamic row = rows.First(x => ((string)((dynamic)x).name == vn));
            s.SetMinMaxValue(vn, row.min, row.max, row.value);
         }
         return s;
      }

      public class SimpleHyperCube
      {
         public SimpleHyperCube(string[] varnames)
         {
         }

         public void SetMinMaxValue(string name, double min, double max, double value)
         {
         }
      }


      private static void ReproWorkitem43(REngine engine)
      {
         Random r = new Random(0);
         int N = 500;
         int n1 = 207;
         int n2 = 623;
         var arGroup1Intensities = new double[N][];
         var arGroup2Intensities = new double[N][];
         for (int i = 0; i < N; i++)
         {
            arGroup1Intensities[i] = new double[n1];
            arGroup2Intensities[i] = new double[n2];
            for (int j = 0; j < n1; j++)
               arGroup1Intensities[i][j] = r.NextDouble();
            for (int j = 0; j < n2; j++)
               arGroup2Intensities[i][j] = r.NextDouble();
         }
         var res = new GenericVector[N];
         NumericVector vGroup1, vGroup2;
         for (int i = 0; i < N; i++)
         {
            vGroup1 = engine.CreateNumericVector(arGroup1Intensities[i]);
            Console.WriteLine(vGroup1.Length);
            if (i % 10 == 4)
            {
               engine.ForceGarbageCollection();
               engine.ForceGarbageCollection();
            }
            vGroup2 = engine.CreateNumericVector(arGroup2Intensities[i]);
            Console.WriteLine(vGroup2.Length);
            engine.SetSymbol("group1", vGroup1);
            engine.SetSymbol("group2", vGroup2);
            GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
            res[i] = testResult;
         }
      }


      #region workitem45

      // https://rdotnet.codeplex.com/workitem/45

      private static bool Started = false;
      private static ArrayList List = new ArrayList();
      private static int[] Counts = new int[256];
      private static Thread Graphics;
      private static IntegerVector group1, group2;
      private static ArrayList KeepAlive = new ArrayList();
      private static byte[] Add = new byte[1];

      private static void Repro45Thread(object rEngine)
      {
         REngine engine = (REngine)rEngine;
         for (int lines = 1; lines < 100; lines++)
         {
            //Entropy.Data Source = new Entropy.Data();
            Random Rand = new Random(0);
            for (int nums = 1; nums < 10; nums++)
            {
               //List.Add(Source.Retzme());
               Rand.NextBytes(Add);
               List.Add(Add[0]);
            }
         }
         int[] Nums = new int[List.Count];
         int Size = 0;
         foreach (byte Number in List)
         {
            Size++;
            Counts[Convert.ToInt32(Number)] = (Counts[Convert.ToInt32(Number)]) + 1;
            Nums[Size - 1] = Convert.ToInt32(Number);
         }
         Size = 0;
         if (KeepAlive.Count != 0)
         {
            //engine.Close();
            engine.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //engine = REngine.CreateInstance("R", new[] { "-q" });
         }
         else
         {
            //GC.KeepAlive(REngine.SetDllDirectory(@"C:\Program Files\R\R-3.0.1\bin\i386"));
            //engine = REngine.CreateInstance("R", new[] { "-q" });
         }
         GC.KeepAlive(engine);
         KeepAlive.Add(engine);
         group1 = engine.CreateIntegerVector(Counts);
         engine.SetSymbol("group1", group1);
         group2 = engine.CreateIntegerVector(Nums);
         GC.KeepAlive(group1);
         GC.KeepAlive(group2);
         engine.SetSymbol("group2", group2);
         engine.Evaluate("library(base)");
         engine.Evaluate("library(stats)");
         engine.Evaluate("x <- group1");
         engine.Evaluate("y <- group2");
         engine.Evaluate("windows( width=10, height=8, pointsize=8)");
         engine.Evaluate("par(yaxp = c( 0, 100, 9))");
         engine.Evaluate("par(xaxp = c( 0, 255, 24))");
         engine.Evaluate("par(cex = 1.0)");
         //Eng.Evaluate("bins=seq(0,255,by=1.0)");
         //Eng.Evaluate("hist(x:y, breaks=50, col=c(\"blue\"))");
         engine.Evaluate("plot(x, type=\"h\", col=c(\"red\"))");
         //engine.Close();
         //engine.Dispose();
         return;
      }

      public static bool ReproWorkitem45(REngine engine, int numThreads = 1)
      {
         for (int i = 0; i < numThreads; i++)
         {
            Graphics = new Thread(Repro45Thread);
            Graphics.Start(engine);
         }
         //Started = true;
         return true;
      }

      #endregion

      #region concurrency tests
      private static void TestMultiThreads(REngine engine)
      {
         engine.Evaluate("x <- rnorm(10000)");
         var blah = engine.GetSymbol("x").AsNumeric().ToArray();

         double[][] res = new double[2][];
         // Can two threads access in parallel the same
         Parallel.For(0, 2, i => readNumericX(i, res, engine));

         engine.Evaluate("x <- list()");
         engine.Evaluate("x[[1]] <- rnorm(10000)");
         engine.Evaluate("x[[2]] <- rnorm(10000)");

         // Can two threads access in parallel the same list
         // Usually bombs, though passes sometimes.
         // The console output would report the following:
         //Error: unprotect_ptr: pointer not found
         //Error: R_Reprotect: only 3 protected items, can't reprotect index 9
         //Error: unprotect_ptr: pointer not found
         //Parallel.For(0, 2, i => readNumericList(i, res, engine));

         // And in seqence, but from other threads - seems to work consistently
         Parallel.For(0, 1, i => readNumericList(i, res, engine));
         Parallel.For(1, 2, i => readNumericList(i, res, engine));

         Console.WriteLine(res[1][1]);
      }

      private static void readNumericList(long i, double[][] res, REngine engine)
      {
         res[i] = engine.Evaluate("x[["+(i+1).ToString()+"]]").AsNumeric().ToArray();
      }

      private static void readNumericX(long i, double[][] res, REngine engine)
      {
         res[i] = engine.GetSymbol("x").AsNumeric().ToArray();
      }

      #endregion
   }
}
