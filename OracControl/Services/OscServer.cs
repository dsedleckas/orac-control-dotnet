using System;
using System.Net;
using System.Threading.Tasks;
using Rug.Osc;

namespace OracControl.Services
{
    public class OscServer : IDisposable, IOscServer
    {
        private volatile OscSender _sender;
        private volatile bool _moduleLoaded = false;
        private volatile bool _menuLoaded = false;
        private volatile OscReceiver _receiver;
        private volatile Task _receiveTask;
        public event EventHandler<OscMessage> OscMessageArrived;
        public event EventHandler<ServerStateChanged> ServerStateChanged;

        public OscServer()
        {
        }

        public async void Reconnect(
            IPAddress oracAddress,
            int oracPort,
            int listenerPort)
        {
            try
            {
                ReconnectListener(listenerPort);
                await ReconnectSender(oracAddress, oracPort);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reconnecting, try again: {e.Message}");
            }
        }

        private void ReconnectListener(int port)
        {
            SendServerState();
            if (_receiver?.State == OscSocketState.Connected)
            {
                _receiver.Close();
                SendServerState();
            }

            _receiveTask?.Wait();
            SendServerState();
            _receiveTask?.Dispose();
            _receiver?.Dispose();

            _receiver = new OscReceiver(port);
            _receiver.Connect();
            SendServerState();
            _receiveTask = Task.Run(async () => await ListenLoop());
        }

        private async Task ReconnectSender(IPAddress address, int port)
        {
            while (_receiver.State != OscSocketState.Connected)
            {
                await Task.Delay(1000);
                SendServerState();
            }

            SendServerState();
            if (_sender?.State == OscSocketState.Connected)
            {
                _sender.Close();
                SendServerState();
            }
            _sender?.Dispose();
            _sender = new OscSender(address, port);
            SendServerState();
            _sender.Connect();
            SendServerState();
            _moduleLoaded = false;
            _menuLoaded = false;
            Send("/Connect", _receiver.Port);
            Send("/ModuleNext", _receiver.Port);
            Send("/ModulePrev", _receiver.Port);


        }

        public void Send(string address, params object[] args)
        {
            try
            {
                var msg = new Rug.Osc.OscMessage(address, args);
                //Console.WriteLine($"Sending: {msg.ToString()}");
                _sender.Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send OSC message, {e.Message}");
            }
        }
        private async Task ListenLoop()
        {
            while (_receiver.State != OscSocketState.Connected)
            {
                await Task.Delay(1000);
                SendServerState();
            }

            while (_receiver.State == OscSocketState.Connected)
            {
                try
                {
                    var packet = _receiver.Receive();
                    //Console.WriteLine($"Received from ORAC: {packet.ToString()} {packet.GetType()}");
                    if (packet is Rug.Osc.OscMessage m)
                    {
                        OscMessageArrived?.Invoke(this,
                            new OscMessage
                            {
                                Address = m.Address,
                                Arg = m.ToArray()
                            });

                        SendServerState();
                        var change = false;
                        if (!_moduleLoaded && m.Address == "/module")
                        {
                            _moduleLoaded = true;
                            change = true;
                        }

                        if (!_menuLoaded && m.Address == "/text" && (int)m.ToArray()[0] == 5)
                        {
                            _menuLoaded = true;
                            change = true;
                        }

                        if (change && _menuLoaded && _moduleLoaded)
                        {
                            // Send special message to all of the components that connect has completed
                            OscMessageArrived?.Invoke(this,
                                new OscMessage
                                {
                                    Address = "/ConnectComplete"
                                });
                        }
                    }
                }
                catch (Exception e) when (e.Message != "The receiver socket has been disconnected")
                {
                    Console.WriteLine($"Error! {e.Message}");
                }
                catch (Exception e) when (e.Message == "The receiver socket has been disconnected")
                {
                }
            }
        }

        public void Dispose()
        {
            _receiver?.Close();
            _sender?.Close();
            _receiveTask?.Wait();
            _receiver?.Dispose();
            _sender?.Close();
        }

        private void SendServerState()
        {
            ServerStateChanged?.Invoke(this,
                new Services.ServerStateChanged
                {
                    ListenerState = _receiver?.State,
                    SenderState = _sender?.State
                });
        }
    }
}
