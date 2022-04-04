using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using NModbus.Extensions.Enron;
using NModbus.Serial;
using NModbus.Utility;

namespace Samples
{
    using System.Linq;
    using System.Runtime.CompilerServices;
    using NModbus.Logging;

    /// <summary>
    ///     Demonstration of NModbus
    /// </summary>
    public class Driver
    {
        private const string PrimarySerialPortName = "COM4";
        private const string SecondarySerialPortName = "COM2";

        private static async Task<int> Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();

            try
            {
                //ModbusSocketSerialMasterReadRegisters();
                //ModbusSocketSerialMasterWriteRegisters();
                //ModbusSocketSerialMasterReadRegisters();
                await Task.Run(() => { });
				        //ModbusTcpMasterReadInputs();
				        //SimplePerfTest();
				        //ModbusSerialRtuMasterWriteRegisters();
				        //ModbusSerialAsciiMasterReadRegisters();
				        //ModbusTcpMasterReadInputs();
								ModbusTcpMasterReadHoldingRegisters32();
                //StartModbusAsciiSlave();
                //ModbusTcpMasterReadInputsFromModbusSlave();
                //ModbusSerialAsciiMasterReadRegistersFromModbusSlave();
                //StartModbusTcpSlave();
                //StartModbusUdpSlave();
                //StartModbusAsciiSlave();
                //await StartModbusSerialRtuSlaveNetwork(cts.Token);
                //await StartModbusSerialRtuSlaveWithCustomMessage(cts.Token);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return 0;
        }

        /// <summary>
        ///     Simple Modbus serial RTU master write holding registers example.
        /// </summary>
        public static void ModbusSerialRtuMasterWriteRegisters()
        {
            using (SerialPort port = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(port);

                byte slaveId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 1, 2, 3 };

                // write three registers
                master.WriteMultipleRegisters(slaveId, startAddress, registers);
            }
        }
        /// <summary>
        /// Simple write to socket connector sending RTU messages
        /// </summary>
        public static void ModbusSocketSerialMasterWriteRegisters()
        {


            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                // configure socket
                var serverIP = IPAddress.Parse("192.168.2.100");
                var serverFullAddr = new IPEndPoint(serverIP, 9000);
                sock.Connect(serverFullAddr);

                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(sock);

                byte slaveId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 10, 20, 30 };

                // write three registers
                master.WriteMultipleRegisters(slaveId, startAddress, registers);
            }
        }
        /// <summary>
        /// Simple Read registers using socket and expecting RTU fromatted response messages.
        /// </summary>
        public static void ModbusSocketSerialMasterReadRegisters()
        {


            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                // configure socket
                var serverIP = IPAddress.Parse("192.168.2.100");
                var serverFullAddr = new IPEndPoint(serverIP, 9000);
                sock.Connect(serverFullAddr);

                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(sock);

                byte slaveId = 1;
                ushort startAddress = 100;
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 3);
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine($"Input {(startAddress + i)}={registers[i]}");
                }
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII master read holding registers example.
        /// </summary>
        public static void ModbusSerialAsciiMasterReadRegisters()
        {
            using (SerialPort port = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var factory = new ModbusFactory();
                IModbusSerialMaster master = factory.CreateAsciiMaster(port);

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
        ///     Simple Modbus TCP master read inputs example.
        /// </summary>
        public static void ModbusTcpMasterReadHoldingRegisters32()
        {
            using (TcpClient client = new TcpClient("10.16.12.50", 502))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);


								byte slaveId = 1;
								ushort startAddress = 7165;
                ushort numInputs = 5;
								UInt32 www = 0x42c80083;

								master.WriteSingleRegister32(slaveId, startAddress, www);
								uint[] registers = master.ReadHoldingRegisters32(slaveId, startAddress, numInputs);

				        for (int i = 0; i < numInputs; i++)
				        {
									Console.WriteLine($"Input {(startAddress + i)}={registers[i]}");
				        }
						}
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
            using (SerialPort slavePort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                var factory = new ModbusFactory();
                IModbusSlaveNetwork slaveNetwork = factory.CreateAsciiSlaveNetwork(slavePort);

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
            using (SerialPort slavePort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                var factory = new ModbusFactory();
                var slaveNetwork = factory.CreateRtuSlaveNetwork(slavePort);

                IModbusSlave slave1 = factory.CreateSlave(1);
                IModbusSlave slave2 = factory.CreateSlave(2);

                slaveNetwork.AddSlave(slave1);
                slaveNetwork.AddSlave(slave2);

                slaveNetwork.ListenAsync().GetAwaiter().GetResult();
            }
        }

        private class HmiBufferRequestmessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }

            public byte SlaveAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                SlaveAddress = frame[0];
                FunctionCode = frame[1];

                MessageFrame = frame
                    .Take(frame.Length - 2)
                    .ToArray();

                ProtocolDataUnit = frame
                    .Skip(1)
                    .ToArray();
            }
        }

        private class HmiBufferResponseMessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }

            public byte SlaveAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                SlaveAddress = frame[0];
                FunctionCode = frame[1];

                MessageFrame = frame
                    .Take(frame.Length - 2)
                    .ToArray();

                ProtocolDataUnit = frame
                    .Skip(1)
                    .ToArray();
            }
        }

        private class HmiBufferFunctionService : IModbusFunctionService
        {
            public byte FunctionCode => 45;

            public IModbusMessage CreateRequest(byte[] frame)
            {
                Console.WriteLine($"HMI Buffer Message Receieved - {frame.Length} bytes");

                var request = new HmiBufferRequestmessage();

                request.Initialize(frame);

                return request;
            }

            public IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore)
            {
                Console.WriteLine("HMI Buffer Message Receieved");

                throw new NotImplementedException();
            }

            public int GetRtuRequestBytesToRead(byte[] frameStart)
            {
                byte registerCountMSB = frameStart[4];
                byte registerCountLSB = frameStart[5];

                int numberOfRegisters = ( registerCountMSB << 8) + registerCountLSB;

                Console.WriteLine($"Got Hmi Buffer Request for {numberOfRegisters} registers.");

                return (numberOfRegisters * 2) + 1;
            }

            public int GetRtuResponseBytesToRead(byte[] frameStart)
            {
                return 4;
            }
        }


        
        /// <summary>
        /// Simple Modbus serial RTU slave example.
        /// </summary>
        public static async Task StartModbusSerialRtuSlaveWithCustomMessage(CancellationToken cancellationToken)
        {
            using (SerialPort slavePort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                slavePort.BaudRate = 57600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.Even;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                var adapter = new SerialPortAdapter(slavePort);

                var functionServices = new IModbusFunctionService[]
                {
                    new HmiBufferFunctionService()
                };

                var factory = new ModbusFactory(functionServices, true, new ConsoleModbusLogger(LoggingLevel.Debug));

                // create modbus slave
                var slaveNetwork = factory.CreateRtuSlaveNetwork(adapter);

                var acTechDataStore = new SlaveStorage();

                acTechDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Input registers: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var danfossStore = new SlaveStorage();

                danfossStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Input registers: {args.Operation} starting at {args.StartingAddress}");
                danfossStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusSlave actechSlave = factory.CreateSlave(21, acTechDataStore);
                IModbusSlave danfossSlave = factory.CreateSlave(1, danfossStore);

                slaveNetwork.AddSlave(actechSlave);
                slaveNetwork.AddSlave(danfossSlave);

                await slaveNetwork.ListenAsync(cancellationToken);
            }
        }

        public static async Task StartModbusSerialRtuSlaveNetwork(CancellationToken cancellationToken)
        {
            using (SerialPort slavePort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                slavePort.BaudRate = 57600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.Even;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                IModbusFactory factory = new ModbusFactory();
                IModbusSlaveNetwork modbusSlaveNetwork = factory.CreateRtuSlaveNetwork(slavePort);

                slavePort.ReadTimeout = 50;
                slavePort.WriteTimeout = 500;

                var acTechDataStore = new SlaveStorage();

                //acTechDataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil discretes: {args.Operation} starting at {args.StartingAddress}");
                //acTechDataStore.CoilInputs.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil  inputs: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Input registers: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var casHmiDataStore = new SlaveStorage();

                casHmiDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"CASHMI Input registers: {args.Operation} starting at {args.StartingAddress}");
                casHmiDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"CASHMI Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var danfossStore = new SlaveStorage();

                danfossStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Input registers: {args.Operation} starting at {args.StartingAddress}");
                danfossStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusSlave slave1 = factory.CreateSlave(21, acTechDataStore);
                IModbusSlave slave2 = factory.CreateSlave(55, casHmiDataStore);

                IModbusSlave slave3 = factory.CreateSlave(1, danfossStore);

                modbusSlaveNetwork.AddSlave(slave1);
                //modbusSlaveNetwork.AddSlave(slave2);
                modbusSlaveNetwork.AddSlave(slave2);
                modbusSlaveNetwork.AddSlave(slave3);

                await modbusSlaveNetwork.ListenAsync(cancellationToken);

                await Task.Delay(1, cancellationToken);
            }
        }

        public static async Task StartModbusSerialRtuSlaveWithCustomStore()
        {
            using (SerialPort slavePort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                slavePort.BaudRate = 9600;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.StopBits = StopBits.One;
                slavePort.Open();

                var factory = new ModbusFactory();
                var slaveNetwork = factory.CreateRtuSlaveNetwork(slavePort);

                var dataStore = new SlaveStorage();

                dataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil discretes: {args.Operation} starting at {args.StartingAddress}");
                dataStore.CoilInputs.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil inputs: {args.Operation} starting at {args.StartingAddress}");
                dataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Input registers: {args.Operation} starting at {args.StartingAddress}");
                dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusSlave slave1 = factory.CreateSlave(1, dataStore);

                slaveNetwork.AddSlave(slave1);

                await slaveNetwork.ListenAsync();
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
            using (SerialPort masterPort = new SerialPort(PrimarySerialPortName))
            using (SerialPort slavePort = new SerialPort(SecondarySerialPortName))
            {
                // configure serial ports
                masterPort.BaudRate = slavePort.BaudRate = 9600;
                masterPort.DataBits = slavePort.DataBits = 8;
                masterPort.Parity = slavePort.Parity = Parity.None;
                masterPort.StopBits = slavePort.StopBits = StopBits.One;
                masterPort.Open();
                slavePort.Open();

                // create modbus slave on seperate thread
                byte slaveId = 1;
                var factory = new ModbusFactory();
                var transport = factory.CreateAsciiTransport(slavePort);
                var network = factory.CreateSlaveNetwork(transport);
                var slave = factory.CreateSlave(slaveId);
                network.AddSlave(slave);

                var listenTask = network.ListenAsync();

                var masterTransport = factory.CreateAsciiTransport(masterPort);
                IModbusSerialMaster master = factory.CreateMaster(masterTransport);

                master.Transport.Retries = 5;
                ushort startAddress = 100;
                ushort numRegisters = 5;
                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                for (int i = 0; i < numRegisters; i++)
                    Console.WriteLine($"Register {(startAddress + i)}={registers[i]}");
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
            using (SerialPort port = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                var factory = new ModbusFactory();
                IModbusRtuTransport transport = factory.CreateRtuTransport(port);
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
