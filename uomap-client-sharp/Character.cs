using System;
using System.Collections.Generic;
using System.Text;

namespace uomap_client
{
    public class Character
    {
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
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            sb.AppendFormat("\"name\":\"{0}\",", _name);
            sb.AppendFormat("\"server\":\"{0}\",", _server);
            sb.AppendFormat("\"x\":{0},", X);
            sb.AppendFormat("\"y\":{0},", Y);
            sb.AppendFormat("\"z\":{0},", Z);
            sb.AppendFormat("\"f\":{0}", F);

            sb.Append("}");

            return sb.ToString();
        }
    }
}
