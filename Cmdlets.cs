using System.Configuration;
using System.Management.Automation;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
namespace HoseRenderer
{
    [Cmdlet(VerbsCommon.New,"PowerGLWebSocket")]
    public class NewPowerGLWebSocket : Cmdlet
    {
        [Parameter(Position=0,Mandatory = true)]
        public string TargetCompuer
        {
            get { return _TargetComputer; }
            set { _TargetComputer = value; }
        }
        [Parameter(Position=1, Mandatory = true)]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private string _TargetComputer;
        private int _Port;
        //TODO FINISH IMPLEMENTING THE NEW-POWERGLWEBSOCKET
    }
    
    [Cmdlet(VerbsCommunications.Write,"PowerGLWebSocket")]
    public class WritePowerGLWebSocket : Cmdlet
    {
        //TODO FINISH IMPLEMENTING THE WRITE-POWERGLWEBSOCKET
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "SocketParameterized")]
        WebSocket WebSocket
        {
            get { return _Socket; }
            set { _Socket = value; }
        }
        [Parameter(Position = 1,Mandatory = true, ParameterSetName = "SocketNotProvided")]
        string TargetComputer
        {
            get { return _TargetComputer; }
            set { _TargetComputer = value; }
        }
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "SocketNotProvided")]
        int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "RequestPipedOrProvided",ValueFromPipeline = true)]
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "SocketParameterized", ValueFromPipeline = true)]
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "SocketNotProvided", ValueFromPipeline = true)]
        HttpObject InputObject
        {
            get { return _InputObject; }
            set { _InputObject = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        uint ShapeNumber
        {
            get { return _Shapenum; }
            set { _Shapenum = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        [ValidateSet(new string[] { "Position","Size","Stretch","Shear","Restitution", "Momentum", "Rotation" })]
        string Property
        {
            get { return _Property; }
            set { _Property = value; }
        }
        [Parameter(Mandatory = true, ParameterSetName = "SocketParameterized")]
        [Parameter(Mandatory = true, ParameterSetName = "SocketNotProvided")]
        [Parameter(Mandatory = true, ParameterSetName = "RequestDataNotProvided")]
        float X
        {
            get { return _ValueX; }
            set { _ValueX = value; }
        }
        [Parameter(ParameterSetName = "SocketParameterized")]
        [Parameter(ParameterSetName = "SocketNotProvided")]
        [Parameter(ParameterSetName = "RequestDataNotProvided")]
        float Y
        {
            get { return _ValueY; }
            set { _ValueY = value; }
        }
        [Parameter(ParameterSetName = "SocketParameterized")]
        [Parameter(ParameterSetName = "SocketNotProvided")]
        [Parameter(ParameterSetName = "RequestDataNotProvided")]
        float Z
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
            if (_InputObject == null)
            {
                _InputObject = new HttpObject(_Shapenum,_Property,_ValueX,_ValueY,_ValueZ);
            }
        }
    }
}
