# NextGen SPICE
An extensible SPICE-like electrical circuit simulator implemented in .NET. This project originated as my
Bachelor Thesis at Faculty of Mathematics and Physics. The thesis text is available here TODO:

## Features
NextGen SPICE supports a subset of various SPICE3 features:

### Supported devices
 - Resistors
 - Volatge and Current sources
   - possibly voltage/current controlled
 - Capacitors, inductors
 - Diode
 - BJT Transistor
 - Subcircuits

### Supported analyses
 - Large-signal analyses:
   - Operating Point
   - Transient

### SPICE netlist parser
 - (basic) statements for supported devices

### Other features:
 - Possibility to select between 64 and 128-bit precision type for computation
 - Ability to extend or replace implementation of devices for individual circuit analyses
 - Ability to define custom circuit analysis type
 - Constructing circuits programmatically
   - Application in electrical circuit evolution possible both for circuit parameters and circuit topology
 - Standalone application providing SPICE-like console interface to the simulator (see Documentation
   TODO)

## Installing
The NextGen SPICE project is split into several projects and packages. Reference them based on the
required functionality:
 - **NextGenSpice.Core, NextGenSpice.Numerics** - mandatory - definiton of basic types
 - **NextGenSpice.Parser** - parsing of the SPICE netlist
 - **NextGenSpice.LargeSignal** - implementation of large-signal analyses
 
**Important!** Currently, the simulator uses a C++ implementation of some numeric procedures for
greater speed and support for 128-bit precision. This will improve in the future once better
equation solver is implemented and the double-double type is implemented natively in .NET.

## Usage 

See Tutorials section TODO:

## Roadmap
- online API documentation via DocFx
- move tutorials from thesis into DocFx documentation
- Use more appropriate methods for sparse matrix representation and sparse matrix solver
- ? implement double-double arithmetic in C# in order to drop dependency on native code
- Add tweaks to improve simulation convergencce
- Add new devices
- Add new analysis types
