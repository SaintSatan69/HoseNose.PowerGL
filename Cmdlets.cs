using System.Management.Automation;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace HoseRenderer
{
    /// <summary>
    /// A cmdlet that will connect to the engine websocket API and return the connection
    /// </summary>
    [Cmdlet(VerbsCommon.New,"PowerGLWebSocket")]
    public class NewPowerGLWebSocket : Cmdlet
    {
        [Parameter(Position=0,Mandatory = true)]
        [ValidateNotNullOrEmpty,ValidateNotNullOrWhiteSpace]
        public string TargetComputer
        {
            get { return _TargetComputer; }
            set { _TargetComputer = value; }
        }
        [Parameter(Position=1, Mandatory = true)]
        [ValidateRange(1024,65000)]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private string _TargetComputer;
        private int _Port;
        protected override void ProcessRecord()
        {
            ClientWebSocket _websocket = new();
            WriteDebug($"Connecting to {_TargetComputer}:{_Port}");
            _websocket.ConnectAsync(new Uri($"ws://{_TargetComputer}:{_Port}/api/shapes/"),CancellationToken.None).Wait();
            WriteDebug("Connection Successfull");
            WriteObject( _websocket );
        }
    }
    /// <summary>
    /// A cmdlet to write to the given websocket with the given payload, otherwise generates the payload and the websocket connection 
    /// </summary>
    [Cmdlet(VerbsCommunications.Write,"PowerGLWebSocket")]
    public class WritePowerGLWebSocket : Cmdlet
    {
        //TODO FINISH IMPLEMENTING THE WRITE-POWERGLWEBSOCKET
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "SocketParameterized")]
        public WebSocket WebSocket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }
        [Parameter(Position = 1,Mandatory = true, ParameterSetName = "SocketNotProvided")]
        public string TargetComputer
        {
            get { return _TargetComputer; }
            set { _TargetComputer = value; }
        }
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "SocketNotProvided")]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        [Parameter(Position = 3, ParameterSetName = "RequestPipedOrProvided",ValueFromPipeline = true)]
        [Parameter(Position = 3, ParameterSetName = "SocketParameterized", ValueFromPipeline = true)]
        [Parameter(Position = 3, ParameterSetName = "SocketNotProvided", ValueFromPipeline = true)]
        public  HttpObject InputObject
        {
            get { return _InputObject; }
            set { _InputObject = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        public uint ShapeNumber
        {
            get { return _Shapenum; }
            set { _Shapenum = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        [ValidateSet(new string[] { "Position","Size","Stretch","Shear","Restitution", "Momentum", "Rotation" })]
        public string Property
        {
            get { return _Property; }
            set { _Property = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        public float X
        {
            get { return _ValueX; }
            set { _ValueX = value; }
        }
        [Parameter(ParameterSetName = "SocketParameterized")]
        [Parameter(ParameterSetName = "SocketNotProvided")]
        [Parameter(ParameterSetName = "RequestDataNotProvided")]
        public float Y
        {
            get { return _ValueY; }
            set { _ValueY = value; }
        }
        [Parameter(ParameterSetName = "SocketParameterized")]
        [Parameter(ParameterSetName = "SocketNotProvided")]
        [Parameter(ParameterSetName = "RequestDataNotProvided")]
        public float Z
        {
            get { return _ValueZ; }
            set { _ValueZ = value; }
        }

        private WebSocket? _Socket;
        private String? _TargetComputer;
        private int _Port;
        private HttpObject? _InputObject;
        private string? _Property;
        private float _ValueX;
        private float _ValueY;
        private float _ValueZ;
        private uint _Shapenum;

        protected override void ProcessRecord()
        {
            bool CloseWhenDone = false;
            if (_Socket == null)
            {
                if (_TargetComputer != null && _Port != 0)
                {
                    ClientWebSocket _websocket = new();
                    WriteDebug($"Connecting to {_TargetComputer}:{_Port}");
                    _websocket.ConnectAsync(new Uri($"ws://{_TargetComputer}:{_Port}/api/shapes/"), CancellationToken.None).Wait();
                    WriteDebug("Connection Successfull");
                    _Socket = _websocket;
                    CloseWhenDone = true;
                }
                else
                {
                    WriteError(new ErrorRecord(new Exception("TargetComputer or Port have ended up being null"),"1",ErrorCategory.InvalidData,_Socket));
                }
            }
            if (_InputObject == null)
            {
                WriteDebug("Making Payload");
                _InputObject = new HttpObject(_Shapenum,_Property,_ValueX,_ValueY,_ValueZ);
            }
            if (_InputObject.Property == "Rotation")
            {
                _InputObject.Property = "Rotate";
            }
            WriteDebug("Sending Payload to endpoint");
            byte[] WebSocketRequestBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_InputObject));

            _Socket.SendAsync(new byte[] { (byte)WebSocketRequestBytes.Length },WebSocketMessageType.Text,false,CancellationToken.None).Wait();

            _Socket.SendAsync(WebSocketRequestBytes, WebSocketMessageType.Text, false, CancellationToken.None).Wait() ;
            byte[] recievebuff = new byte[1024];
            var result = _Socket.ReceiveAsync(recievebuff,CancellationToken.None).Result;
            WriteObject(Encoding.UTF8.GetString(recievebuff));
            if (CloseWhenDone)
            {
                _Socket.SendAsync(new byte[10],WebSocketMessageType.Close,true,CancellationToken.None);
            }
        }
    }
}
