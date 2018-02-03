#ifndef NUMERICCORE_H
#define NUMERICCORE_H

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NUMERICCORE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NUMERICCORE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.

#ifdef NUMERICCORE_EXPORTS
#define NUMERICCORE_API extern "C" __declspec(dllexport)
#else
#define NUMERICCORE_API __declspec(dllimport)
#endif
#endif // NUMERICCORE_H
