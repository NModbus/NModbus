using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusTcpSlaveFixture
    {
        /// <summary>
        /// Tests the scenario when a slave is closed unexpectedly, causing a ConnectionResetByPeer SocketException
        /// We want to handle this gracefully - remove the master from the dictionary
        /// </summary>
        [Fact(Skip = "TestDriver.exe")]
        public void ModbusTcpSlave_ConnectionResetByPeer()
        {
            TcpListener slaveListener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, slaveListener))
            {
                Thread slaveThread = new Thread(slave.Listen);
                slaveThread.IsBackground = true;
                slaveThread.Start();

                Thread.Sleep(500);

                using (Process masterProcess = Process.Start(Path.Combine(
                    Path.GetDirectoryName(Assembly.GetAssembly(typeof(NModbusTcpSlaveFixture)).Location),
                    @"..\..\..\..\tools\nmodbus\TestDriver.exe")))
                {
                    Thread.Sleep(2000);
                    masterProcess.Kill();
                }

                Thread.Sleep(2000);
                Assert.Equal(0, slave.Masters.Count);
            }
        }

        /// <summary>
        /// Tests possible exception when master closes gracefully immediately after transaction
        /// The goal is the test the exception in WriteCompleted when the slave attempts to read another request from an already closed master
        /// </summary>
        [Fact]
        public void ModbusTcpSlave_ConnectionClosesGracefully()
        {
            TcpListener slaveListener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, slaveListener))
            {
                Thread slaveThread = new Thread(slave.Listen);
                slaveThread.IsBackground = true;
                slaveThread.Start();

                var masterClient = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                using (var master = ModbusIpMaster.CreateIp(masterClient))
                {
                    master.Transport.Retries = 0;

                    bool[] coils = master.ReadCoils(1, 1);
                    Assert.Equal(1, coils.Length);

                    Assert.Equal(1, slave.Masters.Count);
                }

                // give the slave some time to remove the master
                Thread.Sleep(50);

                Assert.Equal(0, slave.Masters.Count);
            }
        }

        /// <summary>
        /// Tests possible exception when master closes gracefully and the ReadHeaderCompleted EndRead call returns 0 bytes;
        /// </summary>
        [Fact]
        public void ModbusTcpSlave_ConnectionSlowlyClosesGracefully()
        {
            TcpListener slaveListener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, slaveListener))
            {
                Thread slaveThread = new Thread(slave.Listen);
                slaveThread.IsBackground = true;
                slaveThread.Start();

                var masterClient = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                using (var master = ModbusIpMaster.CreateIp(masterClient))
                {
                    master.Transport.Retries = 0;

                    bool[] coils = master.ReadCoils(1, 1);
                    Assert.Equal(1, coils.Length);

                    Assert.Equal(1, slave.Masters.Count);

                    // wait a bit to let slave move on to read header
                    Thread.Sleep(50);
                }

                // give the slave some time to remove the master
                Thread.Sleep(50);
                Assert.Equal(0, slave.Masters.Count);
            }
        }

        [Fact]
        public void ModbusTcpSlave_MultiThreaded()
        {
            var slaveListener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, slaveListener))
            {
                Thread slaveThread = new Thread(slave.Listen);
                slaveThread.IsBackground = true;
                slaveThread.Start();

                var workerThread1 = new Thread(Read);
                var workerThread2 = new Thread(Read);
                workerThread1.Start();
                workerThread2.Start();

                workerThread1.Join();
                workerThread2.Join();
            }
        }

        private static void Read(object state)
        {
            var masterClient = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
            using (var master = ModbusIpMaster.CreateIp(masterClient))
            {
                master.Transport.Retries = 0;

                var random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    bool[] coils = master.ReadCoils(1, 1);
                    Assert.Equal(1, coils.Length);
                    Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Reading coil value");
                    Thread.Sleep(random.Next(100));
                }
            }
        }
    }
}
