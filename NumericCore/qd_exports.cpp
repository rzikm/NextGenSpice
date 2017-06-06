#include "NumericCore.h"
#include <sstream>
#include <string>

#include <comutil.h>
#include <codecvt>

#include <qd/qd_real.h>

NUMERICCORE_API void qd_add(qd_real& self, qd_real& val)
{
	self += val;
}

NUMERICCORE_API void qd_sub(qd_real& self, qd_real& val)
{
	self -= val;
}

NUMERICCORE_API void qd_mul(qd_real& self, qd_real& val)
{
	self *= val;
}

NUMERICCORE_API void qd_div(qd_real& self, qd_real& val)
{
	self /= val;
}

NUMERICCORE_API void qd_sqrt(qd_real& self)
{
	self = sqrt(self);
}

NUMERICCORE_API BSTR qd_to_string(qd_real& self)
{
	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	auto str = self.to_string(60);
	std::wstring ws = converter.from_bytes(str);

	return ::SysAllocString(ws.c_str());
}