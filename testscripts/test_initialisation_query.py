# c:\Program Files\IronPython 2.7\ipy.exe -X:TabCompletion -X:ColorfulConsole

import clr
clr.AddReference("System.Core")
import System.Linq
clr.ImportExtensions(System.Linq) # This needs to happen *before* the functions are defined.
import System
clr.AddReferenceToFileAndPath(r'C:\src\codeplex\rdotnet\R.NET\bin\Debug\RDotNet.dll')
import RDotNet
clr.ImportExtensions(RDotNet)
from RDotNet import *
from System import GC
from System.Linq import Enumerable
REngine.SetEnvironmentVariables()
engine = REngine.CreateInstance('blah') ; engine.Initialize();

e = REngine.GetInstance()

blah = e.DangerousGetHandle("R_NilValue")
Marshal.ReadIntPtr(blah).ToString()


e.Evaluate("data(cpus, package='MASS')")
e.Evaluate("library(tree)")
e.Evaluate("cpus.ltr <- tree(log10(perf) ~ syct+mmin+mmax+cach+chmin+chmax, cpus)")
irtr = e.Evaluate("ir.tr <- tree(Species ~., iris)")

def asCharacter(sexp):
	return SymbolicExpressionExtension.AsCharacter(sexp)

def asNumeric(sexp):
	return SymbolicExpressionExtension.AsNumeric(sexp)

def toArray(enumerable):
	return System.Linq.Enumerable.ToArray(enumerable)

def dotNetGcTotalMem():
	GC.Collect()
	return System.GC.GetTotalMemory(False)

def rGcTotalMem():
	GC.Collect();
	engine.ForceGarbageCollection();
	engine.ForceGarbageCollection();
	mem = Enumerable.First(asNumeric(engine.Evaluate("memory.size()")))
	return mem

dotNetGcTotalMem()
rGcTotalMem()
sexp = engine.Evaluate("format(1:5e5)")
dotNetGcTotalMem()
rGcTotalMem()
rGcTotalMem()
blah = toArray(asCharacter(sexp))
dotNetGcTotalMem()
rGcTotalMem()
