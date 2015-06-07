using DynamicInterop;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mycsharp
{
    public class CppInterop : UnmanagedDll
    {
        public CppInterop(bool initializeR)
            : base("mycpp.dll")
        {
            // Use initialize as true in this example - may need to use false if your code has already initialized it.
            this.engine = REngine.GetInstance(initialize: initializeR);
        }

        public IntegerVector ProcessToR(int[] values)
        {
            var func = this.GetFunction<_make_int_sexp>("make_int_sexp");
            return new MyIntegerVector(engine, func(values.Length, values));
        }

        public int[] CreateArray()
        {
            // These two following references could be cached for efficiency later on.
            var getLength = this.GetFunction<_c_api_call_getlength>("c_api_call_getlength");
            var getValues = this.GetFunction<_c_api_call>("c_api_call");

            // As far as I know it is preferable, if not needed, to handle arrays in two steps:
            // we need to know the expected length to allocate in C# buffer memory.
            int expectedLength = getLength();
            int[] result = new int[expectedLength];
            getValues(result);
            return result;
        }

        public int ProcessNumVec(IntegerVector nv)
        {
            var func = this.GetFunction<_sexp_input>("sexp_input");
            return (func(nv.DangerousGetHandle()));
        }

        private REngine engine;

        // The interop declaration, using delegate for function pointers.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr _make_int_sexp(int length, int[] values);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int _sexp_input(IntPtr sexp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int _c_api_call_getlength();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void _c_api_call([In, Out] [MarshalAs(UnmanagedType.LPArray)] int[] values);


        /// <summary>
        /// A simple inheritor for R.NET's IntegerVector, just to access a non-public constructor. 
        /// This is because IntegerVector's was not 
        /// designed initially to handle SEXP coming from arbitrary libraries.
        /// May change in the future.
        /// </summary>
        private class MyIntegerVector : IntegerVector
        {
            public MyIntegerVector(REngine engine, IntPtr nativeSEXP) : base(engine, nativeSEXP)
            {
            }
        }
    }
}
