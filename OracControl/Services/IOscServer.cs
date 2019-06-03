using System;
using System.Net;

namespace OracControl.Services
{
    public interface IOscServer
    {
        event EventHandler<OscMessage> OscMessageArrived;
        event EventHandler<ServerStateChanged> ServerStateChanged;

        void Reconnect(IPAddress oracAdress, int oracPort, int listenerPort);
        void Send(string address, params object[] args);
    }
}