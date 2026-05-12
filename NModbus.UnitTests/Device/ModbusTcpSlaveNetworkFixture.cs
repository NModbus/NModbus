using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NModbus.Device;
using NModbus.Logging;
using Xunit;

namespace NModbus.UnitTests.Device
{
    /// <summary>
    /// Regression coverage for issue #135: when a master TCP connection closed,
    /// <c>ModbusTcpSlaveNetwork.OnMasterConnectionClosedHandler</c> tore the
    /// connection out of the masters dictionary and disposed it, but never
    /// unsubscribed its own delegate from the connection's
    /// <c>ModbusMasterTcpConnectionClosed</c> event. The handler also threw
    /// <see cref="ArgumentException"/> from inside an event invocation if the
    /// endpoint had already been removed (e.g. by a concurrent dispose path),
    /// which is unsafe behavior for an event sink.
    /// </summary>
    public class ModbusTcpSlaveNetworkFixture
    {
        private static IModbusFactory Factory => new ModbusFactory();

        [Fact]
        public void OnMasterConnectionClosedHandler_UnknownEndPoint_DoesNotThrow()
        {
            using (var listener = new ListenerHandle())
            using (var network = new ModbusTcpSlaveNetwork(listener.Listener, Factory, NullModbusLogger.Instance))
            {
                // Invoke the private handler with an endpoint that was never registered.
                // Before the fix this threw ArgumentException; after the fix it logs and returns.
                MethodInfo handler = typeof(ModbusTcpSlaveNetwork).GetMethod(
                    "OnMasterConnectionClosedHandler",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(handler);

                object eventArgs = new TcpConnectionEventArgs("127.0.0.1:0");

                Exception captured = null;
                try
                {
                    handler.Invoke(network, new[] { (object)null, eventArgs });
                }
                catch (TargetInvocationException tie)
                {
                    captured = tie.InnerException;
                }

                Assert.Null(captured);
            }
        }

        [Fact]
        public async Task ClosingClientConnection_DetachesEventAndRemovesFromMasters()
        {
            using (var listener = new ListenerHandle())
            using (var network = new ModbusTcpSlaveNetwork(listener.Listener, Factory, NullModbusLogger.Instance))
            using (var cts = new CancellationTokenSource())
            {
                Task listenTask = network.ListenAsync(cts.Token);

                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(IPAddress.Loopback, listener.Port).ConfigureAwait(false);

                    // Wait until the slave network registers the connection.
                    IDictionary masters = await WaitForMasterCountAsync(network, expected: 1).ConfigureAwait(false);
                    Assert.Single(masters);

                    object connection = FirstValue(masters);
                    EventHandler<TcpConnectionEventArgs> beforeSubscribers = GetClosedEventSubscribers(connection);
                    Assert.NotNull(beforeSubscribers);
                    Assert.Single(beforeSubscribers.GetInvocationList());

                    // Closing the client triggers HandleRequestAsync's 0-byte path which fires the event.
                    client.Close();

                    masters = await WaitForMasterCountAsync(network, expected: 0).ConfigureAwait(false);
                    Assert.Empty(masters);

                    EventHandler<TcpConnectionEventArgs> afterSubscribers = GetClosedEventSubscribers(connection);
                    Assert.Null(afterSubscribers);
                }

                cts.Cancel();
                try { await listenTask.ConfigureAwait(false); }
                catch (OperationCanceledException) { /* expected */ }
                catch (SocketException) { /* AcceptTcpClientAsync may surface this when the listener is stopped */ }
            }
        }

        private static async Task<IDictionary> WaitForMasterCountAsync(ModbusTcpSlaveNetwork network, int expected)
        {
            FieldInfo field = typeof(ModbusTcpSlaveNetwork).GetField(
                "_masters",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var masters = (IDictionary)field.GetValue(network);

            for (int i = 0; i < 50; i++)
            {
                if (masters.Count == expected)
                {
                    return masters;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }

            return masters;
        }

        private static object FirstValue(IDictionary masters)
        {
            foreach (object value in masters.Values)
            {
                return value;
            }

            return null;
        }

        private static EventHandler<TcpConnectionEventArgs> GetClosedEventSubscribers(object connection)
        {
            FieldInfo field = connection.GetType().GetField(
                "ModbusMasterTcpConnectionClosed",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (EventHandler<TcpConnectionEventArgs>)field.GetValue(connection);
        }

        private sealed class ListenerHandle : IDisposable
        {
            public ListenerHandle()
            {
                Listener = new TcpListener(IPAddress.Loopback, 0);
                Listener.Start();
                Port = ((IPEndPoint)Listener.LocalEndpoint).Port;
            }

            public TcpListener Listener { get; }

            public int Port { get; }

            public void Dispose()
            {
                try { Listener.Stop(); } catch { /* ignore */ }
            }
        }
    }
}
