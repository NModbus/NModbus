NModbus
=======




|           |Build Status|Code Coverage|
|-----------|:----------:|:-----------:|
|**MS .NET**|[![Build status](https://ci.appveyor.com/api/projects/status/a4r06a0owl6xf9ar/branch/NetworkedSlave?svg=true)](https://ci.appveyor.com/project/nmodbus/nmodbus/branch/NetworkedSlave)|[![codecov.io](https://codecov.io/github/NModbus4/NModbus4/coverage.svg?branch=portable-3.0)](https://codecov.io/github/NModbus/NModbus?branch=NetworkedSlave)|

NModbus is a C# implementation of the Modbus protocol.
Provides connectivity to Modbus slave compatible devices and applications.
Supports serial ASCII, serial RTU, TCP, and UDP protocols.

History
=======

The NModbus4 project appears to have gone quiet. This is a fork of that project.

- NModbus is a fork of NModbus4 (https://github.com/NModbus4/NModbus4).
- NModbus4 is fork of NModbus(https://code.google.com/p/nmodbus).

NModbus4 differs from original NModbus in following:

1. removed USB support(FtdAdapter.dll)
2. removed log4net dependency
3. removed Unme.Common.dll dependency
4. assembly renamed to NModbus4.dll
5. target framework changed to .NET 4

Goals
=======
- Improve Modbus Slave support (e.g. support multiple slave devices on the same physical transport).

Install
=======

To install NModbus4, run the following command in the Package Manager Console

    PM> Install-Package NModbus

Documentation
=======
Documentation is available in chm format (NModbus.chm)

License
=======
NModbus4 is licensed under the [MIT license](LICENSE.txt).
