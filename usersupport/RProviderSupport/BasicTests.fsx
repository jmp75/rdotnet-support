#I "../packages/RProvider.1.0.5/lib"

#r "RDotNet.FSharp"
#r "RDotNet.dll"
#r "RDotNet.NativeLibrary.dll"
#r "RProvider.dll"

open RProvider
open RDotNet
open System
open System.IO

// Initialize the R.NET Engine
let environmentPath = System.Environment.GetEnvironmentVariable("PATH")
//let binaryPath = @"C:\Program Files\R\R-3.0.2\bin\x64"
let binaryPath = if Environment.Is64BitProcess then @"C:\Program Files\R\R-3.0.2\bin\x64" else @"C:\Program Files\R\R-3.0.2\bin\i386"
Environment.SetEnvironmentVariable("PATH", environmentPath+Path.PathSeparator.ToString() + binaryPath)

let r_engine = RDotNet.REngine.CreateInstance("RDotNet")

r_engine.Initialize()
