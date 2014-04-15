using RDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MultipleAppDomains
{
   class Program
   {
      private static REngine engine;
      private static string libDir = @"..\..\..\packages\R.NET.1.5.7\lib\net40";
      // https://rdotnet.codeplex.com/discussions/538512
      [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
      static void Main(string[] args)
      {

         TestPermissions.PermissionDemo();


         REngine.SetEnvironmentVariables();
         var ps = new PermissionSet(PermissionState.None);
         ps.AddPermission(new SecurityPermission(PermissionState.Unrestricted));
         ps.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
         ps.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
         ps.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));

         var appDomain = AppDomain.CreateDomain(
             "sandbox",
             new Evidence(),
             AppDomain.CurrentDomain.SetupInformation,
             ps);

         appDomain.DoCallBack(LoadAssemblies);

         appDomain.DoCallBack(CreateREngine);
      }

      private static void LoadAssemblies()
      {
         LoadDll(Path.Combine(libDir, "RDotNet.dll"));
      }

      private static void LoadDll(string libFileName)
      {
         var absoluteLibPath = Path.GetFullPath(libFileName);

         //if (File.Exists(Environment.CurrentDirectory + "\\" + Path.GetFileName(libFileName)) == false)
         //   File.Copy(absoluteLibPath, Environment.CurrentDirectory + "\\" + Path.GetFileName(libFileName));

         Assembly.LoadFile(absoluteLibPath);
      }

      private static void CreateREngine()
      {
         engine = REngine.GetInstance();
      }

   }
}
