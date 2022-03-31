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
                ModbusSocketSerialClientReadRegisters();
                ModbusSocketSerialClientWriteRegisters();
                ModbusSocketSerialClientReadRegisters();
                await Task.Run(() => { });
                //ModbusTcpClientReadInputs();
                //SimplePerfTest();
                //ModbusSerialRtuClientWriteRegisters();
                //ModbusSerialAsciiClientReadRegisters();
                //ModbusTcpClientReadInputs();
                //StartModbusAsciiServer();
                //ModbusTcpClientReadInputsFromModbusServer();
                //ModbusSerialAsciiClientReadRegistersFromModbusServer();
                //StartModbusTcpServer();
                //StartModbusUdpServer();
                //StartModbusAsciiServer();
                //await StartModbusSerialRtuServerNetwork(cts.Token);
                //await StartModbusSerialRtuServerWithCustomMessage(cts.Token);
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
        ///     Simple Modbus serial RTU client write holding registers example.
        /// </summary>
        public static void ModbusSerialRtuClientWriteRegisters()
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
                IModbusClient client = factory.CreateRtuClient(port);

                byte serverId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 1, 2, 3 };

                // write three registers
                client.WriteMultipleRegisters(serverId, startAddress, registers);
            }
        }
        /// <summary>
        /// Simple write to socket connector sending RTU messages
        /// </summary>
        public static void ModbusSocketSerialClientWriteRegisters()
        {


            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                // configure socket
                var serverIP = IPAddress.Parse("192.168.2.100");
                var serverFullAddr = new IPEndPoint(serverIP, 9000);
                sock.Connect(serverFullAddr);

                var factory = new ModbusFactory();
                IModbusClient client = factory.CreateClient(sock);

                byte serverId = 1;
                ushort startAddress = 100;
                ushort[] registers = new ushort[] { 10, 20, 30 };

                // write three registers
                client.WriteMultipleRegisters(serverId, startAddress, registers);
            }
        }
        /// <summary>
        /// Simple Read registers using socket and expecting RTU fromatted response messages.
        /// </summary>
        public static void ModbusSocketSerialClientReadRegisters()
        {


            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                // configure socket
                var serverIP = IPAddress.Parse("192.168.2.100");
                var serverFullAddr = new IPEndPoint(serverIP, 9000);
                sock.Connect(serverFullAddr);

                var factory = new ModbusFactory();
                IModbusClient client = factory.CreateClient(sock);

                byte serverId = 1;
                ushort startAddress = 100;
                ushort[] registers = client.ReadHoldingRegisters(serverId, startAddress, 3);
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine($"Input {(startAddress + i)}={registers[i]}");
                }

            }
        }
        /// <summary>
        ///     Simple Modbus serial ASCII client read holding registers example.
        /// </summary>
        public static void ModbusSerialAsciiClientReadRegisters()
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
                IModbusSerialClient client = factory.CreateAsciiClient(port);

                byte serverId = 1;
                ushort startAddress = 1;
                ushort numRegisters = 5;

                // read five registers		
                ushort[] registers = client.ReadHoldingRegisters(serverId, startAddress, numRegisters);

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
        ///     Simple Modbus TCP client read inputs example.
        /// </summary>
        public static void ModbusTcpClientReadInputs()
        {
            using (TcpClient tcpClient = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                IModbusClient client = factory.CreateClient(tcpClient);

                // read five input values
                ushort startAddress = 100;
                ushort numInputs = 5;
                bool[] inputs = client.ReadInputs(0, startAddress, numInputs);

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
        ///     Simple Modbus UDP client write coils example.
        /// </summary>
        public static void ModbusUdpClientWriteCoils()
        {
            using (UdpClient udpClient = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 502);
                udpClient.Connect(endPoint);

                var factory = new ModbusFactory();

                var client = factory.CreateClient(udpClient);

                ushort startAddress = 1;

                // write three coils
                client.WriteMultipleCoils(0, startAddress, new bool[] { true, false, true });
            }
        }

        /// <summary>
        ///     Simple Modbus serial ASCII server example.
        /// </summary>
        public static void StartModbusSerialAsciiServer()
        {
            using (SerialPort serverPort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                serverPort.BaudRate = 9600;
                serverPort.DataBits = 8;
                serverPort.Parity = Parity.None;
                serverPort.StopBits = StopBits.One;
                serverPort.Open();

                var factory = new ModbusFactory();
                IModbusServerNetwork serverNetwork = factory.CreateAsciiServerNetwork(serverPort);

                IModbusServer server1 = factory.CreateServer(1);
                IModbusServer server2 = factory.CreateServer(2);

                serverNetwork.AddServer(server1);
                serverNetwork.AddServer(server2);

                serverNetwork.ListenAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        ///     Simple Modbus serial RTU server example.
        /// </summary>
        public static void StartModbusSerialRtuServer()
        {
            using (SerialPort serverPort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                serverPort.BaudRate = 9600;
                serverPort.DataBits = 8;
                serverPort.Parity = Parity.None;
                serverPort.StopBits = StopBits.One;
                serverPort.Open();

                var factory = new ModbusFactory();
                var serverNetwork = factory.CreateRtuServerNetwork(serverPort);

                IModbusServer server1 = factory.CreateServer(1);
                IModbusServer server2 = factory.CreateServer(2);

                serverNetwork.AddServer(server1);
                serverNetwork.AddServer(server2);

                serverNetwork.ListenAsync().GetAwaiter().GetResult();
            }
        }

        private class HmiBufferRequestmessage : IModbusMessage
        {
            public byte FunctionCode { get; set; }


            [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerAddress instead.")]
            public byte SlaveAddress
            {
                get => ServerAddress;
                set => ServerAddress = value;
            }
            public byte ServerAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                ServerAddress = frame[0];
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

            [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerAddress instead.")]
            public byte SlaveAddress
            {
                get => ServerAddress;
                set => ServerAddress = value;
            }
            public byte ServerAddress { get; set; }

            public byte[] MessageFrame { get; private set; }

            public byte[] ProtocolDataUnit { get; private set; }

            public ushort TransactionId { get; set; }

            public void Initialize(byte[] frame)
            {
                ServerAddress = frame[0];
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

            [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use HandleServerRequest instead.")]
            public IModbusMessage HandleSlaveRequest(IModbusMessage request, ISlaveDataStore dataStore)
            {
                return HandleServerRequest(request, dataStore);
            }

            public IModbusMessage HandleServerRequest(IModbusMessage request, IServerDataStore dataStore)
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
        /// Simple Modbus serial RTU server example.
        /// </summary>
        public static async Task StartModbusSerialRtuServerWithCustomMessage(CancellationToken cancellationToken)
        {
            using (SerialPort serverPort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                serverPort.BaudRate = 57600;
                serverPort.DataBits = 8;
                serverPort.Parity = Parity.Even;
                serverPort.StopBits = StopBits.One;
                serverPort.Open();

                var adapter = new SerialPortAdapter(serverPort);

                var functionServices = new IModbusFunctionService[]
                {
                    new HmiBufferFunctionService()
                };

                var factory = new ModbusFactory(functionServices, true, new ConsoleModbusLogger(LoggingLevel.Debug));

                // create modbus server
                var serverNetwork = factory.CreateRtuServerNetwork(adapter);

                var acTechDataStore = new ServerStorage();

                acTechDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Input registers: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var danfossStore = new ServerStorage();

                danfossStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Input registers: {args.Operation} starting at {args.StartingAddress}");
                danfossStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusServer actechServer = factory.CreateServer(21, acTechDataStore);
                IModbusServer danfossServer = factory.CreateServer(1, danfossStore);

                serverNetwork.AddServer(actechServer);
                serverNetwork.AddServer(danfossServer);

                await serverNetwork.ListenAsync(cancellationToken);
            }
        }

        public static async Task StartModbusSerialRtuServerNetwork(CancellationToken cancellationToken)
        {
            using (SerialPort serverPort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                serverPort.BaudRate = 57600;
                serverPort.DataBits = 8;
                serverPort.Parity = Parity.Even;
                serverPort.StopBits = StopBits.One;
                serverPort.Open();

                IModbusFactory factory = new ModbusFactory();
                IModbusServerNetwork modbusServerNetwork = factory.CreateRtuServerNetwork(serverPort);

                serverPort.ReadTimeout = 50;
                serverPort.WriteTimeout = 500;

                var acTechDataStore = new ServerStorage();

                //acTechDataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil discretes: {args.Operation} starting at {args.StartingAddress}");
                //acTechDataStore.CoilInputs.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil  inputs: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Input registers: {args.Operation} starting at {args.StartingAddress}");
                acTechDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"ACTECH Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var casHmiDataStore = new ServerStorage();

                casHmiDataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"CASHMI Input registers: {args.Operation} starting at {args.StartingAddress}");
                casHmiDataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"CASHMI Holding registers: {args.Operation} starting at {args.StartingAddress}");

                var danfossStore = new ServerStorage();

                danfossStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Input registers: {args.Operation} starting at {args.StartingAddress}");
                danfossStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"DANFOSS Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusServer server1 = factory.CreateServer(21, acTechDataStore);
                IModbusServer server2 = factory.CreateServer(55, casHmiDataStore);

                IModbusServer server3 = factory.CreateServer(1, danfossStore);

                modbusServerNetwork.AddServer(server1);
                //modbusServerNetwork.AddServer(server2);
                modbusServerNetwork.AddServer(server2);
                modbusServerNetwork.AddServer(server3);

                await modbusServerNetwork.ListenAsync(cancellationToken);

                await Task.Delay(1, cancellationToken);
            }
        }

        public static async Task StartModbusSerialRtuServerWithCustomStore()
        {
            using (SerialPort serverPort = new SerialPort(PrimarySerialPortName))
            {
                // configure serial port
                serverPort.BaudRate = 9600;
                serverPort.DataBits = 8;
                serverPort.Parity = Parity.None;
                serverPort.StopBits = StopBits.One;
                serverPort.Open();

                var factory = new ModbusFactory();
                var serverNetwork = factory.CreateRtuServerNetwork(serverPort);

                var dataStore = new ServerStorage();

                dataStore.CoilDiscretes.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil discretes: {args.Operation} starting at {args.StartingAddress}");
                dataStore.CoilInputs.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Coil inputs: {args.Operation} starting at {args.StartingAddress}");
                dataStore.InputRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Input registers: {args.Operation} starting at {args.StartingAddress}");
                dataStore.HoldingRegisters.StorageOperationOccurred += (sender, args) => Console.WriteLine($"Holding registers: {args.Operation} starting at {args.StartingAddress}");

                IModbusServer server1 = factory.CreateServer(1, dataStore);

                serverNetwork.AddServer(server1);

                await serverNetwork.ListenAsync();
            }
        }

        /// <summary>
        ///     Simple Modbus serial USB ASCII server example.
        /// </summary>
        public static void StartModbusSerialUsbAsciiServer()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus serial USB RTU server example.
        /// </summary>
        public static void StartModbusSerialUsbRtuServer()
        {
            // TODO
        }

        /// <summary>
        ///     Simple Modbus TCP server example.
        /// </summary>
        public static void StartModbusTcpServer()
        {
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP server
            TcpListener serverTcpListener = new TcpListener(address, port);
            serverTcpListener.Start();

            IModbusFactory factory = new ModbusFactory();

            IModbusServerNetwork network = factory.CreateServerNetwork(serverTcpListener);

            IModbusServer server1 = factory.CreateServer(1);
            IModbusServer server2 = factory.CreateServer(2);

            network.AddServer(server1);
            network.AddServer(server2);

            network.ListenAsync().GetAwaiter().GetResult();

            // prevent the main thread from exiting
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        ///     Simple Modbus UDP server example.
        /// </summary>
        public static void StartModbusUdpServer()
        {
            using (UdpClient client = new UdpClient(502))
            {
                var factory = new ModbusFactory();
                IModbusServerNetwork network = factory.CreateServerNetwork(client);

                IModbusServer server1 = factory.CreateServer(1);
                IModbusServer server2 = factory.CreateServer(2);

                network.AddServer(server1);
                network.AddServer(server2);

                network.ListenAsync().GetAwaiter().GetResult();

                // prevent the main thread from exiting
                Thread.Sleep(Timeout.Infinite);
            }
        }

        /// <summary>
        ///     Modbus TCP client and server example.
        /// </summary>
        public static void ModbusTcpClientReadInputsFromModbusServer()
        {
            byte serverId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP server
            TcpListener serverTcpListener = new TcpListener(address, port);
            serverTcpListener.Start();

            var factory = new ModbusFactory();
            var network = factory.CreateServerNetwork(serverTcpListener);

            IModbusServer server = factory.CreateServer(serverId);

            network.AddServer(server);

            var listenTask = network.ListenAsync();

            // create the client
            TcpClient clientTcpClient = new TcpClient(address.ToString(), port);
            IModbusClient client = factory.CreateClient(clientTcpClient);

            ushort numInputs = 5;
            ushort startAddress = 100;

            // read five register values
            ushort[] inputs = client.ReadInputRegisters(0, startAddress, numInputs);

            for (int i = 0; i < numInputs; i++)
            {
                Console.WriteLine($"Register {(startAddress + i)}={(inputs[i])}");
            }

            // clean up
            clientTcpClient.Close();
            serverTcpListener.Stop();

            // output
            // Register 100=0
            // Register 101=0
            // Register 102=0
            // Register 103=0
            // Register 104=0
        }

        /// <summary>
        ///     Modbus serial ASCII client and server example.
        /// </summary>
        public static void ModbusSerialAsciiClientReadRegistersFromModbusServer()
        {
            using (SerialPort clientPort = new SerialPort(PrimarySerialPortName))
            using (SerialPort serverPort = new SerialPort(SecondarySerialPortName))
            {
                // configure serial ports
                clientPort.BaudRate = serverPort.BaudRate = 9600;
                clientPort.DataBits = serverPort.DataBits = 8;
                clientPort.Parity = serverPort.Parity = Parity.None;
                clientPort.StopBits = serverPort.StopBits = StopBits.One;
                clientPort.Open();
                serverPort.Open();

                // create modbus server on seperate thread
                byte serverId = 1;
                var factory = new ModbusFactory();
                var transport = factory.CreateAsciiTransport(serverPort);
                var network = factory.CreateServerNetwork(transport);
                var server = factory.CreateServer(serverId);
                network.AddServer(server);

                var listenTask = network.ListenAsync();

                var clientTransport = factory.CreateAsciiTransport(clientPort);
                IModbusSerialClient client = factory.CreateClient(clientTransport);

                client.Transport.Retries = 5;
                ushort startAddress = 100;
                ushort numRegisters = 5;
                ushort[] registers = client.ReadHoldingRegisters(serverId, startAddress, numRegisters);

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
                IModbusSerialClient client = factory.CreateClient(transport);

                byte serverId = 1;
                ushort startAddress = 1008;
                uint largeValue = UInt16.MaxValue + 5;

                ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 0);
                ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 2);

                // write large value in two 16 bit chunks
                client.WriteMultipleRegisters(serverId, startAddress, new ushort[] { lowOrderValue, highOrderValue });

                // read large value in two 16 bit chunks and perform conversion
                ushort[] registers = client.ReadHoldingRegisters(serverId, startAddress, 2);
                uint value = ModbusUtility.GetUInt32(registers[1], registers[0]);
            }
        }
    }
}
