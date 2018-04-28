#include "numerics.native.h"
#include <sstream>
#include <string>

#include <comutil.h>
#include <codecvt>

#include <qd/dd_real.h>

NUMERICSNATIVE_API void dd_add(dd_real& self, dd_real& val)
{
	self += val;
}

NUMERICSNATIVE_API void dd_sub(dd_real& self, dd_real& val)
{
	self -= val;
}

NUMERICSNATIVE_API void dd_mul(dd_real& self, dd_real& val)
{
	self *= val;
}

NUMERICSNATIVE_API void dd_div(dd_real& self, dd_real& val)
{
	self /= val;
}

NUMERICSNATIVE_API void dd_sqrt(dd_real& self)
{
	self = sqrt(self);
}

NUMERICSNATIVE_API BSTR dd_to_string(dd_real& self)
{
	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	auto str = self.to_string(30);
	std::wstring ws = converter.from_bytes(str);

	return SysAllocString(ws.c_str());
}


NUMERICSNATIVE_API void d_add(double& self, double val)
{
	self += val;
}

NUMERICSNATIVE_API void d_mul(double& self, double val)
{
	self *= val;
}

NUMERICSNATIVE_API void d_sub(double& self, double val)
{
	self -= val;
}

NUMERICSNATIVE_API void d_div(double& self, double val)
{
	self /= val;
}
