using Rug.Osc;

namespace OracControl.Services
{
    public class ServerStateChanged
    {
        public OscSocketState? SenderState;

        public OscSocketState? ListenerState;
    }
}
