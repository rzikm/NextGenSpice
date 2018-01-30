// NumericCore.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "NumericCore.h"
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
	MatrixWrapper(T* data, size_t size) : data{ data }, side{ size } {  }
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
void do_work_ge(Prec* m, int size, Callback callback)
{
	MatrixWrapper<Prec> mat(m, size +1);

	for (int i = 0; i < size; i++) {
		// Search for maximum in this column
		Prec maxEl = abs(mat(i, i));
		int maxRow = i;
		for (int k = i + 1; k < size; k++) {
			if (abs(mat(k, i)) > maxEl) {
				maxEl = abs(mat(k, i));
				maxRow = k;
			}
		}

		// Swap maximum row with current row (column by column)
		for (int k = i; k < size + 1; k++) {
			Prec tmp = mat(maxRow, k);
			mat(maxRow, k) = mat(i, k);
			mat(i, k) = tmp;
			callback();
		}

		// Make all rows below this one 0 in current column
		for (int k = i + 1; k < size; k++) {
			Prec c = -mat(k, i) / mat(i, i);
			for (int j = i; j < size + 1; j++) {
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

template<typename Prec>
void do_work_gda(Prec *m, int size, Prec* solutions, Callback callback)
{
	size_t it = 0;
	Prec* partial = new Prec[size];
	MatrixWrapper<Prec> mat(m, size +1);
	auto zero = static_cast<Prec>(0);
	Prec error;
	do
	{
		// update results
		for (int i = 0; i < size; i++)
		{
			partial[i] = -mat(i, size);
			for (int j = 0; j < size; j++)
			{
				partial[i] += solutions[i] * mat(i, j);
			}
		}
		//		copy(partial, partial + size, ostream_iterator<Prec>(cout, " "));
		//		cout << endl;

				// update solutions
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				solutions[i] -= mat(j, i) * partial[i] * 0.07;
			}
		}

		callback();
		error = accumulate(partial, partial + size, zero, [](Prec acc, Prec a)
		{
			return a*a + acc;
		});
		++it;
	} while (error > 0.00000001);
	cout << "Iterations: " << it << endl;
	delete[] partial;
}


template <typename Prec>
void matrix_mult(int size, Prec* values, Prec* target, MatrixWrapper<Prec> mat)
{
	for (int i = 0; i < size; i++)
	{
		target[i] = 0;
		for (int j = 0; j < size; j++)
		{
			target[i] += values[j] * mat(i, j);
		}
	}
}

template <typename Prec>
Prec matrix_form(int size, Prec* left, Prec* right, MatrixWrapper<Prec> &mat)
{
	Prec ret = 0;
	for (int i = 0; i < size; i++)
	{
		Prec tmp = 0;
		for (int j = 0; j < size; j++)
		{
			tmp += right[j] * mat(i, j);
		}
		ret += tmp * left[i];
	}
	return ret;
}

template <typename Prec>
Prec dot_product(int size, Prec* left, Prec* right)
{
	Prec ret = 0;
	for (size_t i = 0; i < size; i++)
	{
		ret += left[i] * right[i];
	}
	return ret;
}

template<typename Prec>
void do_work_cjgda(Prec *m, int size, Prec* solutions, Callback callback)
{
	size_t it = 0;
	MatrixWrapper<Prec> mat(m, size +1);
	vector<Prec> r(size);
	vector<Prec> r2(size);
	vector<Prec> p(size);
	vector<Prec> p2(size);
	vector<Prec> x(size);
	vector<Prec> x2(size);

	for (size_t i = 0; i < size; i++)
	{
		r[i] = mat(i, size);
		for (size_t j = 0; j < size; j++)
		{
			r[i] -= mat(i, j) * x[j];
		}
		p[i] = r[i];
	}

	Prec rdot = dot_product(size, r.data(), r.data());

	for (it = 0; it < size; ++it)
	{
		Prec alpha = rdot / matrix_form(size, p.data(), p.data(), mat);

		for (size_t i = 0; i < size; ++i)
		{
			x2[i] = x[i] + alpha*p[i];
			for (size_t j = 0; j < size; j++)
			{
				r[i] -= alpha* mat(i, j) * p[j];
			}
		}

		auto r2dot = dot_product(size, r.data(), r.data());
		if (r2dot < 0.0000000001) break;

		Prec beta = r2dot / rdot;

		for (size_t i = 0; i < size; ++i)
		{
			p2[i] = r[i] + beta*p[i];
		}

		rdot = r2dot;
		swap(p, p2);
		swap(x, x2);
	}
	copy(x2.begin(), x2.end(), solutions);
	cout << "Iterations: " << it + 1 << endl;
}


NUMERICCORE_API void __stdcall Run_d(double* m, int size, Callback callback)
{
	do_work_ge(m, size, callback);
}

NUMERICCORE_API void __stdcall Run_qd(qd_real* m, int size, Callback callback)
{
	do_work_ge(m, size, callback);
}

NUMERICCORE_API void __stdcall Run_d_gda(double* m, int size, double* solutions, Callback callback)
{
	do_work_gda(m, size, solutions, callback);
}


NUMERICCORE_API void __stdcall Run_qd_gda(qd_real* m, int size, qd_real* solutions, Callback callback)
{
	do_work_gda(m, size, solutions, callback);
}

NUMERICCORE_API void __stdcall Run_d_cjgda(double* m, int size, double* solutions, Callback callback)
{
	do_work_cjgda(m, size, solutions, callback);
}

NUMERICCORE_API void __stdcall Run_qd_cjgda(qd_real* m, int size, qd_real* solutions, Callback callback)
{
	do_work_cjgda(m, size, solutions, callback);
}


template<typename Prec>
void do_gauss_solve(Prec* mat, Prec *b, int size)
{
	MatrixWrapper<Prec> m(mat, size);

	// we start from node 1, because 0 is the ground/reference (0V)
	for (int i = 1; i < size - 1; i++)
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
	for (int i = size - 1; i > 0; i--)
	{
		if (b[i] == 0)
		{
			continue;
		}
		// normalize
		b[i] /= m(i, i);
		//m(i, i) = 1;
		// backward elimination
		for (int k = i - 1; k >= 0; k--)
		{
			b[k] -= m(k, i) * b[i];
			//m[k, i) = 0;
		}
	}
}

NUMERICCORE_API void __stdcall gauss_solve_double(double* mat, double* b, int size)
{
	do_gauss_solve(mat, b, size);
}

NUMERICCORE_API void __stdcall gauss_solve_qd(qd_real* mat, qd_real* b, int size)
{
	do_gauss_solve(mat, b, size);
}

NUMERICCORE_API void __stdcall gauss_solve_dd(dd_real* mat, dd_real* b, int size)
{
	do_gauss_solve(mat, b, size);
}

