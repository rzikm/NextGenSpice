// NumericCore.cpp : Defines the exported functions for the DLL application.
//

#include "numerics.native.h"
#include <cstdlib>
#include <vector>
#include <algorithm>
#include <qd/qd_real.h>
#include <numeric>
#include <iterator>
#include <memory>


using Callback = void(__stdcall *)();

template <typename T>
class MatrixWrapper
{
public:
	MatrixWrapper(T* data, size_t size) : data{data}, side{size}
	{
	}

	T& operator()(size_t i, size_t j)
	{
		return data[i * side + j];
	}

private:
	T* data;
	size_t side;
};

using namespace std;

template <typename Prec>
void do_gauss_solve(Prec* mat, Prec* b, int size)
{
	MatrixWrapper<Prec> m(mat, size);

	for (int i = 0; i < size - 1; i++)
	{
		// Search for maximum in this column
		Prec maxEl = abs(m(i, i));
		int maxRow = i;
		for (int k = i + 1; k < size; k++)
		{
			if (abs(m(k, i)) > maxEl)
			{
				maxEl = abs(m(k, i));
				maxRow = k;
			}
		}

		// Swap maximum row with current row (column by column)
		for (int k = i; k < size; k++)
		{
			swap(m(maxRow, k), m(i, k));
		}
		// swap in b vector
		{
			swap(b[maxRow], b[i]);
		}

		// eliminate current variable in all columns
		for (int k = i + 1; k < size; k++)
		{
			if (m(k, i) == Prec()) continue; // skip elimination for zero columns

			Prec c = -m(k, i) / m(i, i);
			for (int j = i; j < size; j++)
			{
				if (i == j)
					m(k, j) = Prec();
				else
					m(k, j) += c * m(i, j);
			}
			// b vector
			b[k] += c * b[i];
		}
	}


	// GaussElimSolve equation Ax=b for an upper triangular matrix A
	for (int i = size - 1; i >= 0; i--)
	{
		if (b[i] == 0)
		{
			continue;
		}
		// normalize
		b[i] /= m(i, i);
		// m(i, i) = 1;

		// backward elimination
		for (int k = i - 1; k >= 0; k--)
		{
			b[k] -= m(k, i) * b[i];
			// m[k, i) = 0;
		}
	}
}

NUMERICSNATIVE_API void __stdcall gauss_solve_double(double* mat, double* b, int size)
{
	do_gauss_solve(mat, b, size);
}

NUMERICSNATIVE_API void __stdcall gauss_solve_qd(qd_real* mat, qd_real* b, int size)
{
	do_gauss_solve(mat, b, size);
}

NUMERICSNATIVE_API void __stdcall gauss_solve_dd(dd_real* mat, dd_real* b, int size)
{
	do_gauss_solve(mat, b, size);
}
