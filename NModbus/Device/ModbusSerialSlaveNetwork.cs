using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NModbus.Interfaces;
using NModbus.IO;
using NModbus.Message;

namespace NModbus.Device
{
    public class ModbusSerialSlaveNetwork : ModbusSlaveNetwork
    {
        public ModbusSerialSlaveNetwork(IModbusRtuTransport transport) 
            : base(transport)
        {
        }

        //public static ModbusSerialSlaveNetwork CreateRtu(IStreamResource streamResource)
        //{
        //    if (streamResource == null)
        //    {
        //        throw new ArgumentNullException(nameof(streamResource));
        //    }

        //    return new ModbusSerialSlaveNetwork(new ModbusRtuTransport(streamResource));
        //}

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

                        //Create the request
                        IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(frame);

                        //Check the message
                        if (SerialTransport.CheckFrame && !SerialTransport.ChecksumsMatch(request, frame))
                        {
                            string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                            Debug.WriteLine(msg);
                            throw new IOException(msg);
                        }

                        //Apply the request
                        ApplyRequest(request);
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