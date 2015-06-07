#pragma once

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <R.h>
#include <Rinternals.h>
#include <Rmath.h>
#include <R_ext/Applic.h>
#include <R_ext/Lapack.h>
#include <Rdefines.h>

#ifdef __cplusplus
extern "C" {
#endif
	__declspec(dllexport) int c_api_call_getlength();
	__declspec(dllexport) void c_api_call(int * result);
	__declspec(dllexport) SEXP make_int_sexp(int n, int * values);
	__declspec(dllexport) int sexp_input(SEXP input);
#ifdef __cplusplus
}
#endif
