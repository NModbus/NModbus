﻿using System;

namespace NModbus.Device
{
    public class TcpConnectionEventArgs : EventArgs
    {
        public TcpConnectionEventArgs(string endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (endPoint == string.Empty)
            {
                throw new ArgumentException(Resources.EmptyEndPoint);
            }

            EndPoint = endPoint;
        }

        public string EndPoint { get; set; }
    }
}
