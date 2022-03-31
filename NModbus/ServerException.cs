using System;
using System.Diagnostics.CodeAnalysis;
using NModbus.Message;

namespace NModbus
{
#if NET46
    using System.Runtime.Serialization;
    using System.Security.Permissions;
#endif

    /// <summary>
    ///     Represents slave errors that occur during communication.
    /// </summary>
#if NET46
    [Serializable]
#endif
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerException instead.")]
    public class SlaveException : ServerException { }

    /// <summary>
    ///     Represents server errors that occur during communication.
    /// </summary>
#if NET46
    [Serializable]
#endif
    public class ServerException : Exception
    {
        private const string ServerAddressPropertyName = "ServerAdress";
        private const string FunctionCodePropertyName = "FunctionCode";
        private const string ServerExceptionCodePropertyName = "ServerExceptionCode";

        private readonly ServerExceptionResponse _serverExceptionResponse;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerException" /> class.
        /// </summary>
        public ServerException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal ServerException(ServerExceptionResponse serverExceptionResponse)
        {
            _serverExceptionResponse = serverExceptionResponse;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
        internal ServerException(string message, ServerExceptionResponse serverExceptionResponse)
            : base(message)
        {
            _serverExceptionResponse = serverExceptionResponse;
        }

#if NET46
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        ///     The class name is null or
        ///     <see cref="P:System.Exception.HResult"></see> is zero (0).
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected ServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                _serverExceptionResponse = new ServerExceptionResponse(
                    info.GetByte(ServerAddressPropertyName),
                    info.GetByte(FunctionCodePropertyName),
                    info.GetByte(ServerExceptionCodePropertyName));
            }
        }
#endif

        /// <summary>
        ///     Gets a message that describes the current exception.
        /// </summary>
        /// <value>
        ///     The error message that explains the reason for the exception, or an empty string.
        /// </value>
        public override string Message
        {
            get
            {
                string responseString;
                responseString = _serverExceptionResponse != null ? string.Concat(Environment.NewLine, _serverExceptionResponse) : string.Empty;
                return string.Concat(base.Message, responseString);
            }
        }

        /// <summary>
        ///     Gets the response function code that caused the exception to occur, or 0.
        /// </summary>
        /// <value>The function code.</value>
        public byte FunctionCode => _serverExceptionResponse != null ? _serverExceptionResponse.FunctionCode : (byte)0;

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerExceptionCode instead.")]
        public byte SlaveExceptionCode => ServerExceptionCode;
        /// <summary>
        ///     Gets the server exception code, or 0.
        /// </summary>
        /// <value>The server exception code.</value>
        public byte ServerExceptionCode => _serverExceptionResponse != null ? _serverExceptionResponse.ServerExceptionCode : (byte)0;


        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerAddress instead.")]
        public byte SlaveAddress => ServerAddress;
        /// <summary>
        ///     Gets the server address, or 0.
        /// </summary>
        /// <value>The server address.</value>
        public byte ServerAddress => _serverExceptionResponse != null ? _serverExceptionResponse.ServerAddress : (byte)0;

#if NET46
        /// <summary>
        ///     When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see>
        ///     with information about the exception.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Argument info is validated, rule does not understand AND condition.")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null && _serverExceptionResponse != null)
            {
                info.AddValue(ServerAddressPropertyName, _serverExceptionResponse.ServerAddress);
                info.AddValue(FunctionCodePropertyName, _serverExceptionResponse.FunctionCode);
                info.AddValue(ServerExceptionCodePropertyName, _serverExceptionResponse.ServerExceptionCode);
            }
        }
#endif
    }
}
