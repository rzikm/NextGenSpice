#include "NumericCore.h"
#include <sstream>
#include <string>

#include <comutil.h>
#include <codecvt>

#include <qd/dd_real.h>

NUMERICCORE_API void dd_add(dd_real& self, dd_real& val)
{
	self += val;
}

NUMERICCORE_API void dd_sub(dd_real& self, dd_real& val)
{
	self -= val;
}

NUMERICCORE_API void dd_mul(dd_real& self, dd_real& val)
{
	self *= val;
}

NUMERICCORE_API void dd_div(dd_real& self, dd_real& val)
{
	self /= val;
}

NUMERICCORE_API void dd_sqrt(dd_real& self)
{
	self = sqrt(self);
}

NUMERICCORE_API BSTR dd_to_string(dd_real& self)
{
	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	auto str = self.to_string(30);
	std::wstring ws = converter.from_bytes(str);

	return ::SysAllocString(ws.c_str());
}