using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device
{
    public class ModbusSerialSlaveNetwork : ModbusSlaveNetwork
    {
        private readonly IModbusSerialTransport _serialTransport;

        public ModbusSerialSlaveNetwork(IModbusSerialTransport transport) 
            : base(transport)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            _serialTransport = transport;
        }

        private IModbusSerialTransport SerialTransport
        {
            get { return _serialTransport; }
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
                        IModbusMessage response =  ApplyRequest(request);

                        if (response != null)
                        {
                            Transport.Write(response);
                        }
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