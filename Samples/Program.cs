using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus.Data;
using NModbus.Device;
using NModbus.Serial;
using NModbus.Utility;

namespace Samples
{
    /// <summary>
    ///     Demonstration of NModbus
    /// </summary>
    public class Driver
    {
        private static void Main(string[] args)
        {
            try
            {
                //ModbusTcpMasterReadInputs();
                //SimplePerfTest();
                //ModbusSerialRtuMasterWriteRegisters();
                //ModbusSerialAsciiMasterReadRegisters();
                //ModbusTcpMasterReadInputs();
                //StartModbusAsciiSlave();
                //ModbusTcpMasterReadInputsFromModbusSlave();
                //ModbusSerialAsciiMasterReadRegistersFromModbusSlave();
                //StartModbusTcpSlave();
                //StartModbusUdpSlave();
                //StartModbusAsciiSlave();
                //StartModbusSerialSlaveNetwork().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        ///     Simple Modbus serial RTU master write holding registers example.
        /// </summary>
        public static void ModbusSerialRtuMasterWriteRegisters()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(adapter);

                byte slaveId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 1, 2, 3 };

                // write three registers
                master.WriteMultipleRegisters(slaveId, startAddress, registers);
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII master read holding registers example.
        /// </summary>
        public static void ModbusSerialAsciiMasterReadRegisters()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                IModbusSerialMaster master = ModbusSerialMaster.CreateAscii(adapter);

                byte slaveId = 1;
                ushort startAddress = 1;
                ushort numRegisters = 5;

                // read five registers		
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                for (int i = 0; i < numRegisters; i++)
                {
                    Console.WriteLine($"Register {startAddress + i}={registers[i]}");
                }
            }

            // output: 
            // Register 1=0
            // Register 2=0
            // Register 3=0
            // Register 4=0
            // Register 5=0
        }

        /// <summary>
        ///     Simple Modbus TCP master read inputs example.
        /// </summary>
        public static void ModbusTcpMasterReadInputs()
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

                // read five input values
                ushort startAddress = 100;
                ushort numInputs = 5;
                bool[] inputs = master.ReadInputs(startAddress, numInputs);

                for (int i = 0; i < numInputs; i++)
                {
                    Console.WriteLine($"Input {(startAddress + i)}={(inputs[i] ? 1 : 0)}");
                }
            }

            // output: 
            // Input 100=0
            // Input 101=0
            // Input 102=0
            // Input 103=0
            // Input 104=0
        }

        /// <summary>
        ///     Simple Modbus UDP master write coils example.
        /// </summary>
        public static void ModbusUdpMasterWriteCoils()
        {
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 502);
                client.Connect(endPoint);

                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

                ushort startAddress = 1;

                // write three coils
                master.WriteMultipleCoils(startAddress, new bool[] { true, false, true });
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII slave example.
        /// </summary>
        public static void StartModbusSerialAsciiSlave()
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                byte unitId = 1;

                var adapter = new SerialPortAdapter(slavePort);
                // create modbus slave
                ModbusSlave slave = ModbusSerialSlave.CreateAscii(unitId, adapter);

                slave.ListenAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        ///     Simple Modbus serial RTU slave example.
        /// </summary>
        public static void StartModbusSerialRtuSlave()
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                byte unitId = 1;

                var adapter = new SerialPortAdapter(slavePort);
                // create modbus slave
                ModbusSlave slave = ModbusSerialSlave.CreateRtu(unitId, adapter);

                slave.ListenAsync().GetAwaiter().GetResult();
            }
        }

        public static async Task StartModbusSerialSlaveNetwork()
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                var adapter = new SerialPortAdapter(slavePort);

                ModbusSlaveNetwork modbusSlaveNetwork = ModbusSerialSlaveNetwork.CreateRtu(adapter);

                ModbusSlave slave1 = ModbusSlave.Create(1);
                ModbusSlave slave2 = ModbusSlave.Create(2);

                slave1.DataStore.HoldingRegisters[1] = 1;
                slave2.DataStore.HoldingRegisters[1] = 2;

                await modbusSlaveNetwork.AddSlaveAsync(slave1);
                await modbusSlaveNetwork.AddSlaveAsync(slave2);

                await modbusSlaveNetwork.ListenAsync();
            }
        }

        /// <summary>
        ///     Simple Modbus serial USB ASCII slave example.
        /// </summary>
        public static void StartModbusSerialUsbAsciiSlave()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus serial USB RTU slave example.
        /// </summary>
        public static void StartModbusSerialUsbRtuSlave()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus TCP slave example.
        /// </summary>
        public static void StartModbusTcpSlave()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();

            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener);

            slave.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        ///     Simple Modbus UDP slave example.
        /// </summary>
        public static void StartModbusUdpSlave()
        {
            using (UdpClient client = new UdpClient(502))
            {
                ModbusUdpSlave slave = ModbusUdpSlave.CreateUdp(client);

                slave.ListenAsync().GetAwaiter().GetResult();

                // prevent the main thread from exiting
                Thread.Sleep(Timeout.Infinite);
            }
        }

        /// <summary>
        ///     Modbus TCP master and slave example.
        /// </summary>
        public static void ModbusTcpMasterReadInputsFromModbusSlave()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener);
            var listenTask = slave.ListenAsync();

            // create the master
            TcpClient masterTcpClient = new TcpClient(address.ToString(), port);
            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTcpClient);

            ushort numInputs = 5;
            ushort startAddress = 100;

            // read five register values
            ushort[] inputs = master.ReadInputRegisters(startAddress, numInputs);

            for (int i = 0; i < numInputs; i++)
            {
                Console.WriteLine($"Register {(startAddress + i)}={(inputs[i])}");
            }

            // clean up
            masterTcpClient.Close();
            slaveTcpListener.Stop();

            // output
            // Register 100=0
            // Register 101=0
            // Register 102=0
            // Register 103=0
            // Register 104=0
        }

        /// <summary>
        ///     Modbus serial ASCII master and slave example.
        /// </summary>
        public static void ModbusSerialAsciiMasterReadRegistersFromModbusSlave()
        {
            using (SerialPort masterPort = new SerialPort("COM1"))
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial ports
                masterPort.BaudRate = slavePort.BaudRate = 9600;
                masterPort.DataBits = slavePort.DataBits = 8;
                masterPort.Parity = slavePort.Parity = Parity.None;
                masterPort.StopBits = slavePort.StopBits = StopBits.One;
                masterPort.Open();
                slavePort.Open();

                var slaveAdapter = new SerialPortAdapter(slavePort);
                // create modbus slave on seperate thread
                byte slaveId = 1;
                ModbusSlave slave = ModbusSerialSlave.CreateAscii(slaveId, slaveAdapter);
                var listenTask = slave.ListenAsync();

                var masterAdapter = new SerialPortAdapter(masterPort);
                // create modbus master
                ModbusSerialMaster master = ModbusSerialMaster.CreateAscii(masterAdapter);

                master.Transport.Retries = 5;
                ushort startAddress = 100;
                ushort numRegisters = 5;

                // read five register values
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                for (int i = 0; i < numRegisters; i++)
                {
                    Console.WriteLine($"Register {(startAddress + i)}={registers[i]}");
                }
            }

            // output
            // Register 100=0
            // Register 101=0
            // Register 102=0
            // Register 103=0
            // Register 104=0
        }

        /// <summary>
        ///     Write a 32 bit value.
        /// </summary>
        public static void ReadWrite32BitValue()
        {
            using (SerialPort port = new SerialPort("COM1"))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var adapter = new SerialPortAdapter(port);
                // create modbus master
                ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(adapter);

                byte slaveId = 1;
                ushort startAddress = 1008;
                uint largeValue = UInt16.MaxValue + 5;

                ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 0);
                ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 2);

                // write large value in two 16 bit chunks
                master.WriteMultipleRegisters(slaveId, startAddress, new ushort[] { lowOrderValue, highOrderValue });

                // read large value in two 16 bit chunks and perform conversion
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 2);
                uint value = ModbusUtility.GetUInt32(registers[1], registers[0]);
            }
        }
    }
}
