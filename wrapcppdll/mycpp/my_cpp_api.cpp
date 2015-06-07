#include "my_cpp_api.h"

#define SAMPLE_LENGTH 10
int c_api_call_getlength()
{
	return SAMPLE_LENGTH;
}

SEXP some_R_calculations()
{
	int size = SAMPLE_LENGTH;
	int tmp[SAMPLE_LENGTH] = { 1, 2, 3, 2, 3, 4, 5, 4, 5, 6 };
	return make_int_sexp(size, tmp);
}

void c_api_call(int * result)
{
	SEXP p = some_R_calculations();
	int* int_ptr = INTEGER_POINTER(p);
	int n = length(p);
	for (int i = 0; i < n; i++)
		result[i] = int_ptr[i];
	// We are done with p; irrespective of where some_R_calculations got it, 
	// we can notify the R GC that this function releases it.
	R_ReleaseObject(p);
}

// Source: https://github.com/jmp75/rClr/blob/master/src/rClr.cpp
SEXP make_int_sexp(int n, int * values) {
	SEXP result;
	long i = 0;
	int * int_ptr;
	// If you PROTECT an R object, you should make sure you UNPROTECT. 
	// Better do it in the same function if you can.
	PROTECT(result = NEW_INTEGER(n));
	int_ptr = INTEGER_POINTER(result);
	for (i = 0; i < n; i++) {
		int_ptr[i] = values[i];
	}
	UNPROTECT(1);
	return result;
}

int sexp_input(SEXP input)
{
	// SHOULD DO:
	//if (!isInteger(input))
	//{
	//}
	int n = length(input);
	// SHOULD DO:
	// if (n<1) do something 

	// We could do complicated stuff with R native functions. 
	// For the sake of this sample, just:
	int* values = INTEGER(input);
	return values[0];
}

