using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Modbus.IntegrationTests.CustomMessages;
using Xunit;

namespace Modbus.IntegrationTests
{
    public abstract class ModbusMasterFixture : IDisposable
    {
        public const int Port = 502;
        public const byte SlaveAddress = 1;
        public const string DefaultMasterSerialPortName = "COM1";
        public const string DefaultSlaveSerialPortName = "COM2";

        public static IPAddress TcpHost { get; } = new IPAddress(new byte[] { 127, 0, 0, 1 });

        public static IPEndPoint DefaultModbusIPEndPoint { get; } = new IPEndPoint(TcpHost, Port);

        public double AverageReadTime => 150;

        protected ModbusMaster Master { get; set; }

        protected SerialPort MasterSerialPort { get; set; }

        protected TcpClient MasterTcp { get; set; }

        protected UdpClient MasterUdp { get; set; }

        protected ModbusSlave Slave { get; set; }

        protected SerialPort SlaveSerialPort { get; set; }

        protected TcpListener SlaveTcp { get; set; }

        protected UdpClient SlaveUdp { get; set; }

        private Thread SlaveThread { get; set; }

        private Process Jamod { get; set; }

        public static SerialPort CreateAndOpenSerialPort(string portName)
        {
            SerialPort port = new SerialPort(portName);
            port.Parity = Parity.None;
            port.Open();

            return port;
        }

        public void SetupSlaveSerialPort()
        {
            SlaveSerialPort = new SerialPort(DefaultSlaveSerialPortName);
            SlaveSerialPort.Parity = Parity.None;
            SlaveSerialPort.Open();
        }

        public void StartSlave()
        {
            SlaveThread = new Thread(Slave.Listen);
            SlaveThread.IsBackground = true;
            SlaveThread.Start();
        }

        public void StartJamodSlave(string program)
        {
            string pathToJamod = Path.Combine(
                Path.GetDirectoryName(Assembly.GetAssembly(typeof(ModbusMasterFixture)).Location), "../../../../tools/jamod");
            string classpath = string.Format(@"-classpath ""{0};{1};{2}""", Path.Combine(pathToJamod, "jamod.jar"), Path.Combine(pathToJamod, "comm.jar"), Path.Combine(pathToJamod, "."));
            ProcessStartInfo startInfo = new ProcessStartInfo("java", string.Format(CultureInfo.InvariantCulture, "{0} {1}", classpath, program));
            Jamod = Process.Start(startInfo);

            Thread.Sleep(4000);
            Assert.False(Jamod.HasExited, "Jamod Serial Ascii Slave did not start correctly.");
        }

        public void Dispose()
        {
            Master?.Dispose();

            Slave?.Dispose();

            if (Jamod != null)
            {
                Jamod.Kill();
                Thread.Sleep(4000);
            }
        }

        [Fact]
        public virtual void ReadCoils()
        {
            bool[] coils = Master.ReadCoils(SlaveAddress, 2048, 8);
            Assert.Equal(new bool[] { false, false, false, false, false, false, false, false }, coils);
        }

        [Fact]
        public virtual void ReadInputs()
        {
            bool[] inputs = Master.ReadInputs(SlaveAddress, 150, 3);
            Assert.Equal(new bool[] { false, false, false }, inputs);
        }

        [Fact]
        public virtual void ReadHoldingRegisters()
        {
            ushort[] registers = Master.ReadHoldingRegisters(SlaveAddress, 104, 2);
            Assert.Equal(new ushort[] { 0, 0 }, registers);
        }

        [Fact]
        public virtual void ReadInputRegisters()
        {
            ushort[] registers = Master.ReadInputRegisters(SlaveAddress, 104, 2);
            Assert.Equal(new ushort[] { 0, 0 }, registers);
        }

        [Fact]
        public virtual void WriteSingleCoil()
        {
            bool coilValue = Master.ReadCoils(SlaveAddress, 10, 1)[0];
            Master.WriteSingleCoil(SlaveAddress, 10, !coilValue);
            Assert.Equal(!coilValue, Master.ReadCoils(SlaveAddress, 10, 1)[0]);
            Master.WriteSingleCoil(SlaveAddress, 10, coilValue);
            Assert.Equal(coilValue, Master.ReadCoils(SlaveAddress, 10, 1)[0]);
        }

        [Fact]
        public virtual void WriteSingleRegister()
        {
            ushort testAddress = 200;
            ushort testValue = 350;

            ushort originalValue = Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0];
            Master.WriteSingleRegister(SlaveAddress, testAddress, testValue);
            Assert.Equal(testValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
            Master.WriteSingleRegister(SlaveAddress, testAddress, originalValue);
            Assert.Equal(originalValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
        }

        [Fact]
        public virtual void WriteMultipleRegisters()
        {
            ushort testAddress = 120;
            ushort[] testValues = new ushort[] { 10, 20, 30, 40, 50 };

            ushort[] originalValues = Master.ReadHoldingRegisters(SlaveAddress, testAddress, (ushort)testValues.Length);
            Master.WriteMultipleRegisters(SlaveAddress, testAddress, testValues);
            ushort[] newValues = Master.ReadHoldingRegisters(SlaveAddress, testAddress, (ushort)testValues.Length);
            Assert.Equal(testValues, newValues);
            Master.WriteMultipleRegisters(SlaveAddress, testAddress, originalValues);
        }

        [Fact]
        public virtual void WriteMultipleCoils()
        {
            ushort testAddress = 200;
            bool[] testValues = new bool[] { true, false, true, false, false, false, true, false, true, false };

            bool[] originalValues = Master.ReadCoils(SlaveAddress, testAddress, (ushort)testValues.Length);
            Master.WriteMultipleCoils(SlaveAddress, testAddress, testValues);
            bool[] newValues = Master.ReadCoils(SlaveAddress, testAddress, (ushort)testValues.Length);
            Assert.Equal(testValues, newValues);
            Master.WriteMultipleCoils(SlaveAddress, testAddress, originalValues);
        }

        [Fact]
        public virtual void ReadMaximumNumberOfHoldingRegisters()
        {
            ushort[] registers = Master.ReadHoldingRegisters(SlaveAddress, 104, 125);
            Assert.Equal(125, registers.Length);
        }

        [Fact]
        public virtual void ReadWriteMultipleRegisters()
        {
            ushort startReadAddress = 120;
            ushort numberOfPointsToRead = 5;
            ushort startWriteAddress = 50;
            ushort[] valuesToWrite = new ushort[] { 10, 20, 30, 40, 50 };

            ushort[] valuesToRead = Master.ReadHoldingRegisters(SlaveAddress, startReadAddress, numberOfPointsToRead);
            ushort[] readValues = Master.ReadWriteMultipleRegisters(SlaveAddress, startReadAddress, numberOfPointsToRead, startWriteAddress, valuesToWrite);
            Assert.Equal(valuesToRead, readValues);

            ushort[] writtenValues = Master.ReadHoldingRegisters(SlaveAddress, startWriteAddress, (ushort)valuesToWrite.Length);
            Assert.Equal(valuesToWrite, writtenValues);
        }

        [Fact]
        public virtual void SimpleReadRegistersPerformanceTest()
        {
            int retries = Master.Transport.Retries;
            Master.Transport.Retries = 5;
            double actualAverageReadTime = CalculateAverage(Master);
            Master.Transport.Retries = retries;
            Assert.True(actualAverageReadTime < AverageReadTime, string.Format(CultureInfo.InvariantCulture, "Test failed, actual average read time {0} is greater than expected {1}", actualAverageReadTime, AverageReadTime));
        }

        [Fact]
        public virtual void ExecuteCustomMessage_ReadHoldingRegisters()
        {
            CustomReadHoldingRegistersRequest request = new CustomReadHoldingRegistersRequest(3, SlaveAddress, 104, 2);
            CustomReadHoldingRegistersResponse response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(request);
            Assert.Equal(new ushort[] { 0, 0 }, response.Data);
        }

        [Fact]
        public virtual void ExecuteCustomMessage_WriteMultipleRegisters()
        {
            ushort testAddress = 120;
            ushort[] testValues = new ushort[] { 10, 20, 30, 40, 50 };
            CustomReadHoldingRegistersRequest readRequest = new CustomReadHoldingRegistersRequest(3, SlaveAddress, testAddress, (ushort)testValues.Length);
            CustomWriteMultipleRegistersRequest writeRequest = new CustomWriteMultipleRegistersRequest(16, SlaveAddress, testAddress, new RegisterCollection(testValues));

            var response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(readRequest);
            ushort[] originalValues = response.Data;
            Master.ExecuteCustomMessage<CustomWriteMultipleRegistersResponse>(writeRequest);
            response = Master.ExecuteCustomMessage<CustomReadHoldingRegistersResponse>(readRequest);
            ushort[] newValues = response.Data;
            Assert.Equal(testValues, newValues);
            writeRequest = new CustomWriteMultipleRegistersRequest(16, SlaveAddress, testAddress, new RegisterCollection(originalValues));
            Master.ExecuteCustomMessage<CustomWriteMultipleRegistersResponse>(writeRequest);
        }

        internal static double CalculateAverage(IModbusMaster master)
        {
            ushort startAddress = 5;
            ushort numRegisters = 5;

            // JIT compile the IL
            master.ReadHoldingRegisters(SlaveAddress, startAddress, numRegisters);

            Stopwatch stopwatch = new Stopwatch();
            long sum = 0;
            double numberOfReads = 50;

            for (int i = 0; i < numberOfReads; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                master.ReadHoldingRegisters(SlaveAddress, startAddress, numRegisters);
                stopwatch.Stop();

                checked
                {
                    sum += stopwatch.ElapsedMilliseconds;
                }
            }

            return sum / numberOfReads;
        }
    }
}
