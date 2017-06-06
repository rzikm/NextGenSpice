// NumericCore.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "NumericCore.h"
#include <cstdlib>
#include <vector>
#include <qd/qd_real.h>


using Callback = void(__stdcall *)();

template <typename T>
class MatrixWrapper
{
public:
	MatrixWrapper(T* data, size_t size) : data{ data }, side{ size +1 } {  }
	T& operator()(size_t i, size_t j)
	{
		return data[i*side + j];
	}
private:
	T *data;
	size_t side;
};

using namespace std;

//qd_real abs(qd_real r)
//{
//	return r < 0 ? -r : r;
//}

template<typename Prec>
void do_work(Prec* m, int size, Callback callback)
{
	MatrixWrapper<Prec> mat(m, size);

	for (int i = 0; i<size; i++) {
		// Search for maximum in this column
		Prec maxEl = abs(mat(i, i));
		int maxRow = i;
		for (int k = i + 1; k<size; k++) {
			if (abs(mat(k, i)) > maxEl) {
				maxEl = abs(mat(k, i));
				maxRow = k;
			}
		}

		// Swap maximum row with current row (column by column)
		for (int k = i; k<size + 1; k++) {
			Prec tmp = mat(maxRow,k);
			mat(maxRow, k) = mat(i, k);
			mat(i, k) = tmp;
			callback();
		}

		// Make all rows below this one 0 in current column
		for (int k = i + 1; k<size; k++) {
			Prec c = -mat(k, i) / mat(i, i);
			for (int j = i; j<size + 1; j++) {
				if (i == j) {
					mat(k, j) = 0.0;
				}
				else {
					mat(k, j) += c * mat(i, j);
				}
			}
		}
	}

	// Solve equation Ax=b for an upper triangular matrix A
	for (int i = size - 1; i >= 0; i--) {
		mat(i, size) = mat(i, size) / mat(i, i);
		for (int k = i - 1; k >= 0; k--) {
			mat(k, size) -= mat(k, i) * mat(i, size);
		}
		callback();
	}
}

NUMERICCORE_API void __stdcall Run_d(double* m, int size, Callback callback)
{
	do_work(m, size, callback);
}

NUMERICCORE_API void __stdcall Run_qd(qd_real* m, int size, Callback callback)
{
	do_work(m, size, callback);
}

