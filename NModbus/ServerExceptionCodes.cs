﻿using System;

namespace NModbus
{

    /// <summary>
    ///  Modbus slave exception codes
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerExceptionCodes instead.")]
    public static class SlaveExceptionCodes
    {
        public const byte IllegalFunction = 1;
        public const byte IllegalDataAddress = 2;
        public const byte IllegalDataValue = 3;
        public const byte SlaveDeviceFailure = 4;
        public const byte Acknowledge = 5;
        public const byte SlaveDeviceBusy = 6;
    }


    /// <summary>
    ///  Modbus server exception codes
    /// </summary>
    public static class ServerExceptionCodes
    {
        /// <summary>
        /// The function code received in the query is not an allowable action for the server.  This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected.  It could also indicate that the server is in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values. If a Poll Program Complete command was issued, this code indicates that no program function preceded it.
        /// </summary>
        public const byte IllegalFunction = 1;

        /// <summary>
        /// The data address received in the query is not an allowable address for the server. More specifically, the combination of reference number and transfer length is invalid. For a controller with 100 registers, a request with offset 96 and length 4 would succeed, a request with offset 96 and length 5 will generate exception 02.
        /// </summary>
        public const byte IllegalDataAddress = 2;

        /// <summary>
        /// A value contained in the query data field is not an allowable value for the server.  This indicates a fault in the structure of remainder of a complex request, such as that the implied length is incorrect. It specifically does NOT mean that a data item submitted for storage in a register has a value outside the expectation of the application program, since the MODBUS protocol is unaware of the significance of any particular value of any particular register.
        /// </summary>
        public const byte IllegalDataValue = 3;

        /// <summary>
        /// An unrecoverable error occurred while the server was attempting to perform the requested action.
        /// </summary>
        public const byte ServerDeviceFailure = 4;

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// 
        /// The server has accepted the request and is processing it, but a long duration of time will be required to do so.T
        /// his response is returned to prevent a timeout error from occurring in the client.The client can next issue a 
        /// Poll Program Complete message to determine if processing is completed.
        /// </summary>
        public const byte Acknowledge = 5;

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// The server is engaged in processing a long-duration program command.The client should retransmit 
        /// the message later when the server is free.
        /// </summary>
        public const byte ServerDeviceBusy = 6;
    }
}