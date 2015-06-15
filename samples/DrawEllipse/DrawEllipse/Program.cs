using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawEllipse
{
    class Program
    {
        // REF: Testing and proposed alternative for http://stackoverflow.com/questions/30824895/c-sharp-cant-execute-code-from-r
        static void Main(string[] args)
        {
            REngine.SetEnvironmentVariables();
            REngine e = REngine.GetInstance();
                
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
            Console.WriteLine("Pausing - press any key");
            Console.ReadKey();
            e.Evaluate(" lines(elp, col='red' , lty=7 , lwd=2)");
            //e.Evaluate("lines(e)");
            //e.Evaluate("lines(ellipsoidPoints(mve@cov, d2 = d2.95, loc=mve@center), col='blue', lty='7' , lwd='2') ");
            // axGraphicsDevice2.RemoveFromConnector();

            Console.WriteLine("Pausing - press any key");
            Console.ReadKey();
            e.Evaluate("dev.off()");
            e.Dispose();
        }
    }
}
