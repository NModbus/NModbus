NModbus
=======

NModbus is a C# implementation of the Modbus protocol.
Provides connectivity to Modbus slave compatible devices and applications.
Supports serial ASCII, serial RTU, TCP, and UDP protocols.

History
=======

The NModbus4 project appears to have gone quiet. This is a fork of that project.

- NModbus is a fork of NModbus4 (https://github.com/NModbus4/NModbus4).
- NModbus4 is fork of NModbus(https://code.google.com/p/nmodbus).

NModbus differs from NModbus4 in following:

- Modbus slave devices are now added to a network which is represented by `IModbusSlaveInstance`.
- Heavier use of interfaces.
- Custom function code handlers can be added to slave devices.


Goals
=======
- Improve Modbus Slave support (e.g. support multiple slave devices on the same physical transport).

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
