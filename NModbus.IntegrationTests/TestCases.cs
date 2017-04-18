using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Data;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
    internal class TestCases
    {
        public static void Serial()
        {
            using (var masterPort = new SerialPort("COM2"))
            using (var slavePort = new SerialPort("COM1"))
            {
                // configure serial ports
                masterPort.BaudRate = slavePort.BaudRate = 9600;
                masterPort.DataBits = slavePort.DataBits = 8;
                masterPort.Parity = slavePort.Parity = Parity.None;
                masterPort.StopBits = slavePort.StopBits = StopBits.One;
                masterPort.Open();
                slavePort.Open();

                using (var slave = ModbusSerialSlave.CreateRtu(1, slavePort))
                {
                    StartSlave(slave);

                    // create modbus master
                    using (var master = ModbusSerialMaster.CreateRtu(masterPort))
                    {
                        ReadRegisters(master);
                    }
                }
            }
        }

        public static void Tcp()
        {
            var slaveClient = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), 502);
            using (var slave = ModbusTcpSlave.CreateTcp((byte)1, slaveClient))
            {
                StartSlave(slave);

                IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });
                var masterClient = new TcpClient(address.ToString(), 502);

                using (var master = ModbusIpMaster.CreateIp(masterClient))
                {
                    ReadRegisters(master);
                }
            }
        }

        public static void Udp()
        {
            var slaveClient = new UdpClient(502);
            using (var slave = ModbusUdpSlave.CreateUdp(slaveClient))
            {
                StartSlave(slave);

                var masterClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 502);
                masterClient.Connect(endPoint);

                using (var master = ModbusIpMaster.CreateIp(masterClient))
                {
                    ReadRegisters(master);
                }
            }
        }

        public static void StartSlave(ModbusSlave slave)
        {
            slave.DataStore = DataStoreFactory.CreateTestDataStore();
            var slaveThread = new Thread(slave.Listen);
            slaveThread.Start();
        }

        public static void ReadRegisters(IModbusMaster master)
        {
            var result = master.ReadHoldingRegisters(1, 0, 5);

            for (int i = 0; i < 5; i++)
            {
                if (result[i] != i + 1)
                {
                    throw new Exception();
                }
            }
        }
    }
}
