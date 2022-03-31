NModbus
=======




|           |Build Status|
|-----------|:----------:|
|**MS .NET**|[![Build status](https://ci.appveyor.com/api/projects/status/a4r06a0owl6xf9ar/branch/NetworkedSlave?svg=true)](https://ci.appveyor.com/project/nmodbus/nmodbus/branch/NetworkedSlave)|

NModbus is a C# implementation of the Modbus protocol.
Provides connectivity to Modbus server compatible devices and applications.
Supports serial ASCII, serial RTU, TCP, and UDP protocols.

History
=======

The NModbus4 project appears to have gone quiet. This is a fork of that project.

- NModbus is a fork of NModbus4 (https://github.com/NModbus4/NModbus4).
- NModbus4 is fork of NModbus(https://code.google.com/p/nmodbus).

NModbus differs from NModbus4 in following:

- Modbus server devices are now added to a network which is represented by `IModbusServerInstance`.
- Heavier use of interfaces.
- Custom function code handlers can be added to server devices.


Goals
=======
- Improve Modbus Server support (e.g. support multiple server devices on the same physical transport).

Install
=======

To install NModbus, run the following command in the Package Manager Console

    PM> Install-Package NModbus

Documentation
=======
Documentation is available [here](https://nmodbus.github.io/api/NModbus.html).

License
=======
NModbus is licensed under the [MIT license](LICENSE.txt).
