### Purpose

Sample code for an answer to [a stackoverflow question](http://stackoverflow.com/q/30686866/2752565).

This aims to demonstrate a few things:
* how to call C functions from C#, using 
* basics of how to manage R data structures from C.
* optionally, using R.NET to be a facade to R native data structures, if the C API has such arguments or return values.

## Compiling

* Sample is designed on Windows with Visual Studio 2013 Express. Can be made to run on Linux, but not done.
* If you are in Visual Studio, choose configuration Debug and platform x64
* You need to have a suitable Rdll.lib that includes the R API functions you need to call. 
** If you have Visual Studio installed and no previous experience with export library files you should try to generate such a .lib file using [create_lib.cmd](https://github.com/jmp75/rClr/blob/master/src/create_lib.cmd) after cloning or downloading [rClr](https://github.com/jmp75/rClr)
* you need to adapt a few paths in mycpp.vcxproj (via visual studio or text editor):

```xml
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'">
    <IncludePath>C:\Program Files\R\R-3.2.0\include;$(IncludePath)</IncludePath>
    <LibraryPath>$(VC_LibraryPath_x64);$(WindowsSDK_LibraryPath_x64);C:\src\github_jm\rClr\src\libfiles\x64\</LibraryPath>
  </PropertyGroup>
```

```xml
	<AdditionalDependencies>Rdll.lib;kernel32.lib;user32.lib;gdi32.lib;winspool.lib;comdlg32.lib;advapi32.lib;shell32.lib;ole32.lib;oleaut32.lib;uuid.lib;odbc32.lib;odbccp32.lib;%(AdditionalDependencies)</AdditionalDependencies>
```

After that, the c++ project should compile.

To compile the C# project, you need to let Visual Studio install its dependency R.NET.

## Running

If you are in Visual Studio, set configuration Debug and platform x64. Set mycsharp as the startup project. running in debug mode should run straight away. The sample code includes trying to add a couple of directories to the PATH environment variable. 