using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Modbus.IO;
using Modbus.Message;

namespace Modbus.Device
{
    public class ModbusSerialSlaveNetwork : ModbusSlaveNetwork
    {
        protected ModbusSerialSlaveNetwork(ModbusTransport transport) 
            : base(transport)
        {
        }

        public static ModbusSerialSlaveNetwork CreateRtu(IStreamResource streamResource)
        {
            if (streamResource == null)
            {
                throw new ArgumentNullException(nameof(streamResource));
            }

            return new ModbusSerialSlaveNetwork(new ModbusRtuTransport(streamResource));
        }

        private ModbusSerialTransport SerialTransport
        {
            get
            {
                var transport = Transport as ModbusSerialTransport;

                if (transport == null)
                {
                    throw new ObjectDisposedException("SerialTransport");
                }

                return transport;
            }
        }

        public override async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        //TODO: remove deleay once async will be implemented in transport level
                        await Task.Delay(20).ConfigureAwait(false);

                        // read request and build message
                        byte[] frame = SerialTransport.ReadRequest();
                        IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(frame);

                        if (SerialTransport.CheckFrame && !SerialTransport.ChecksumsMatch(request, frame))
                        {
                            string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                            Debug.WriteLine(msg);
                            throw new IOException(msg);
                        }

                        ModbusSlave slave = GetSlave(request.SlaveAddress);

                        // only service requests addressed to our slaves
                        if (slave == null)
                        {
                            Debug.WriteLine($"NModbus Slave Network ignoring request intended for NModbus Slave {request.SlaveAddress}");
                            continue;
                        }

                        // perform action
                        IModbusMessage response = slave.ApplyRequest(request);

                        // write response
                        SerialTransport.Write(response);
                    }
                    catch (IOException ioe)
                    {
                        Debug.WriteLine($"IO Exception encountered while listening for requests - {ioe.Message}");
                        SerialTransport.DiscardInBuffer();
                    }
                    catch (TimeoutException te)
                    {
                        Debug.WriteLine($"Timeout Exception encountered while listening for requests - {te.Message}");
                        SerialTransport.DiscardInBuffer();
                    }

                    // TODO better exception handling here, missing FormatException, NotImplemented...
                }
                catch (InvalidOperationException)
                {
                    // when the underlying transport is disposed
                    break;
                }
            }
        }
    }
}