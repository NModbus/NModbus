using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
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
                StartModbusSerialRtuSlaveNetwork().GetAwaiter().GetResult();
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
                var factory = new ModbusFactory();

                IModbusMaster master = factory.CreateRtuMaster(adapter);

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

                var factory = new ModbusFactory();

                // create modbus master
                IModbusSerialMaster master = factory.CreateAsciiMaster(adapter);

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
                var factory = new ModbusFactory();

                IModbusMaster master = factory.CreateMaster(client);

                // read five input values
                ushort startAddress = 100;
                ushort numInputs = 5;
                bool[] inputs = master.ReadInputs(0, startAddress, numInputs);

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

                var factory = new ModbusFactory();

                var master = factory.CreateMaster(client);

                ushort startAddress = 1;

                // write three coils
                master.WriteMultipleCoils(0, startAddress, new bool[] { true, false, true });
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

                var factory = new ModbusFactory();

                var adapter = new SerialPortAdapter(slavePort);

                // create modbus slave
                IModbusSlaveNetwork slaveNetwork = factory.CreateAsciiSlaveNetwork(adapter);

                IModbusSlave slave1 = factory.CreateSlave(1);
                IModbusSlave slave2 = factory.CreateSlave(2);

                slaveNetwork.AddSlave(slave1);
                slaveNetwork.AddSlave(slave2);

                slaveNetwork.ListenAsync().GetAwaiter().GetResult();
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

                var adapter = new SerialPortAdapter(slavePort);

                var factory = new ModbusFactory();

                // create modbus slave
                var slaveNetwork = factory.CreateRtuSlaveNetwork(adapter);

                IModbusSlave slave1 = factory.CreateSlave(1);
                IModbusSlave slave2 = factory.CreateSlave(2);

                slaveNetwork.AddSlave(slave1);
                slaveNetwork.AddSlave(slave2);

                slaveNetwork.ListenAsync().GetAwaiter().GetResult();
            }
        }

        public static async Task StartModbusSerialRtuSlaveNetwork()
        {
            using (SerialPort slavePort = new SerialPort("COM5"))
            {
                // configure serial port
                slavePort.BaudRate = 19200;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.Even;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                IModbusFactory factory = new ModbusFactory();

                var adapter = new SerialPortAdapter(slavePort);

                IModbusSlaveNetwork modbusSlaveNetwork = factory.CreateRtuSlaveNetwork(adapter);

                IModbusSlave slave1 = factory.CreateSlave(1);
                IModbusSlave slave2 = factory.CreateSlave(2);

                modbusSlaveNetwork.AddSlave(slave1);
                modbusSlaveNetwork.AddSlave(slave2);

                await modbusSlaveNetwork.ListenAsync();

                await Task.Delay(1);
            }
        }

        public static void StartModbusSerialRtuSlaveWithCustomStore()
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

                var factory = new ModbusFactory();

                // create modbus slave
                var slaveNetwork = factory.CreateRtuSlaveNetwork(adapter);

                var dataStore = new SlaveStorage();

                dataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil discretes: {args.Operation} starting at {args.StartingAddress}");
                dataStore.CoilInputs.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil inputs: {args.Operation} starting at {args.StartingAddress}");
                dataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Input registers: {args.Operation} starting at {args.StartingAddress}");
                dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusSlave slave1 = factory.CreateSlave(1, dataStore);

                slaveNetwork.AddSlave(slave1);

                slaveNetwork.ListenAsync().GetAwaiter().GetResult();
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
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();

            IModbusFactory factory = new ModbusFactory();

            IModbusSlaveNetwork network = factory.CreateSlaveNetwork(slaveTcpListener);

            IModbusSlave slave1 = factory.CreateSlave(1);
            IModbusSlave slave2 = factory.CreateSlave(2);

            network.AddSlave(slave1);
            network.AddSlave(slave2);

            network.ListenAsync().GetAwaiter().GetResult();

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
                var factory = new ModbusFactory();

                IModbusSlaveNetwork network = factory.CreateSlaveNetwork(client);

                IModbusSlave slave1 = factory.CreateSlave(1);
                IModbusSlave slave2 = factory.CreateSlave(2);

                network.AddSlave(slave1);
                network.AddSlave(slave2);

                network.ListenAsync().GetAwaiter().GetResult();

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

            var factory = new ModbusFactory();

            var network = factory.CreateSlaveNetwork(slaveTcpListener);

            IModbusSlave slave = factory.CreateSlave(slaveId);

            network.AddSlave(slave);

            var listenTask = network.ListenAsync();

            // create the master
            TcpClient masterTcpClient = new TcpClient(address.ToString(), port);
            IModbusMaster master = factory.CreateMaster(masterTcpClient);

            ushort numInputs = 5;
            ushort startAddress = 100;

            // read five register values
            ushort[] inputs = master.ReadInputRegisters(0, startAddress, numInputs);

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

                var factory = new ModbusFactory();

                var transport = factory.CreateAsciiTransport(slaveAdapter);

                var network = factory.CreateSlaveNetwork(transport);

                var slave = factory.CreateSlave(slaveId);

                network.AddSlave(slave);

                var listenTask = network.ListenAsync();

                var masterAdapter = new SerialPortAdapter(masterPort);

                var masterTransport = factory.CreateAsciiTransport(masterAdapter);

                // create modbus master
                IModbusSerialMaster master = factory.CreateMaster(masterTransport);

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

                var factory = new ModbusFactory();

                var adapter = new SerialPortAdapter(port);

                IModbusRtuTransport transport = factory.CreateRtuTransport(adapter);

                // create modbus master
                IModbusSerialMaster master = factory.CreateMaster(transport);

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
