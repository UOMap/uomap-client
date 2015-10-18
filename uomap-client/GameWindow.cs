using System;
using System.Text;

namespace uomap_client
{
    public class GameWindow
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Is the client window in focus?
        /// </summary>

        public IntPtr OpenHandle { get; set; }

        public int CharacterAddress { get; set; }
        public int PositionAddress { get; set; }
        public int ServerAddress { get; set; }

        public bool ClientClosed { get; set; }

        public GameWindow(IntPtr h, string t)
        {
            Handle = h;
            Title = t;
        }

        public bool SameHandle(IntPtr compare)
        {
            return Handle.ToInt32() == compare.ToInt32();
        }

        public bool IsInitialized
        {
            get
            {
                return (CharacterAddress != 0) && (PositionAddress != 0) && (ServerAddress != 0);
            }
        }

        private string _server;
        private string _name;

        private bool _invalidated;

        public string Server 
        { 
            get
            {
                return _server;
            }
            set
            {
                if(_server != value)
                {
                    _server = value;
                    _invalidated = true;
                }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    _invalidated = true;
                }
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int F { get; set; }
        public bool Moved { get; set; }
        public bool IsActive { get; set; }

        public bool Invalidated
        {
            get
            {
                return _invalidated;
            }
            set
            {
                _invalidated = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Server);
        }

        public string ToJson()
        {
            var sb = new StringBuilder();

            sb.Append("{");

            sb.AppendFormat("\"name\":\"{0}\",", _name);
            sb.AppendFormat("\"server\":\"{0}\",", _server);
            sb.AppendFormat("\"x\":{0},", X);
            sb.AppendFormat("\"y\":{0},", Y);
            sb.AppendFormat("\"z\":{0},", Z);
            sb.AppendFormat("\"f\":{0},", F);
            sb.AppendFormat("\"active\":{0}", IsActive ? "true" : "false");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
