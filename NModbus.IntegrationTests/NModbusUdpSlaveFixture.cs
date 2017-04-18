using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusUdpSlaveFixture
    {
        [Fact]
        public void ModbusUdpSlave_EnsureTheSlaveShutsDownCleanly()
        {
            UdpClient client = new UdpClient(ModbusMasterFixture.Port);
            using (var slave = ModbusUdpSlave.CreateUdp(1, client))
            {
                var handle = new AutoResetEvent(false);

                var backgroundThread = new Thread(state =>
                {
                    handle.Set();
                    slave.Listen();
                });

                backgroundThread.IsBackground = true;
                backgroundThread.Start();

                handle.WaitOne();
                Thread.Sleep(100);
            }
        }

        [Fact]
        public void ModbusUdpSlave_NotBound()
        {
            UdpClient client = new UdpClient();
            ModbusSlave slave = ModbusUdpSlave.CreateUdp(1, client);
            Assert.Throws<InvalidOperationException>(() => slave.Listen());
        }

        [Fact]
        public void ModbusUdpSlave_MultipleMasters()
        {
            Random randomNumberGenerator = new Random();
            bool master1Complete = false;
            bool master2Complete = false;
            UdpClient masterClient1 = new UdpClient();
            masterClient1.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
            ModbusIpMaster master1 = ModbusIpMaster.CreateIp(masterClient1);

            UdpClient masterClient2 = new UdpClient();
            masterClient2.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
            ModbusIpMaster master2 = ModbusIpMaster.CreateIp(masterClient2);

            UdpClient slaveClient = CreateAndStartUdpSlave(ModbusMasterFixture.Port, DataStoreFactory.CreateTestDataStore());

            Thread master1Thread = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(randomNumberGenerator.Next(1000));
                    Debug.WriteLine("Read from master 1");
                    Assert.Equal(new ushort[] { 2, 3, 4, 5, 6 }, master1.ReadHoldingRegisters(1, 5));
                }
                master1Complete = true;
            });

            Thread master2Thread = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(randomNumberGenerator.Next(1000));
                    Debug.WriteLine("Read from master 2");
                    Assert.Equal(new ushort[] { 3, 4, 5, 6, 7 }, master2.ReadHoldingRegisters(2, 5));
                }
                master2Complete = true;
            });

            master1Thread.Start();
            master2Thread.Start();

            while (!master1Complete || !master2Complete)
            {
                Thread.Sleep(200);
            }

            slaveClient.Close();
            masterClient1.Close();
            masterClient2.Close();
        }

        [Fact]
        public void ModbusUdpSlave_MultiThreaded()
        {
            var dataStore = DataStoreFactory.CreateDefaultDataStore();
            dataStore.CoilDiscretes.Add(false);

            using (var slave = CreateAndStartUdpSlave(502, dataStore))
            {
                var workerThread1 = new Thread(ReadThread);
                var workerThread2 = new Thread(ReadThread);
                workerThread1.Start();
                workerThread2.Start();

                workerThread1.Join();
                workerThread2.Join();
            }
        }

        [Fact(Skip = "TODO consider supporting this scenario")]
        public void ModbusUdpSlave_SingleMasterPollingMultipleSlaves()
        {
            DataStore slave1DataStore = new DataStore();
            slave1DataStore.CoilDiscretes.Add(true);

            DataStore slave2DataStore = new DataStore();
            slave2DataStore.CoilDiscretes.Add(false);

            using (UdpClient slave1 = CreateAndStartUdpSlave(502, slave1DataStore))
            using (UdpClient slave2 = CreateAndStartUdpSlave(503, slave2DataStore))
            using (UdpClient masterClient = new UdpClient())
            {
                masterClient.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
                ModbusIpMaster master = ModbusIpMaster.CreateIp(masterClient);

                for (int i = 0; i < 5; i++)
                {
                    // we would need to create an overload taking in a port argument
                    Assert.True(master.ReadCoils(0, 1)[0]);
                    Assert.False(master.ReadCoils(1, 1)[0]);
                }
            }
        }

        private static void ReadThread(object state)
        {
            var masterClient = new UdpClient();
            masterClient.Connect(ModbusMasterFixture.DefaultModbusIPEndPoint);
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

        private UdpClient CreateAndStartUdpSlave(int port, DataStore dataStore)
        {
            UdpClient slaveClient = new UdpClient(port);
            ModbusSlave slave = ModbusUdpSlave.CreateUdp(slaveClient);
            slave.DataStore = dataStore;
            Thread slaveThread = new Thread(slave.Listen);
            slaveThread.Start();

            return slaveClient;
        }
    }
}
