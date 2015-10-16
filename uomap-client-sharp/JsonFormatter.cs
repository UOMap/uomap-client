using System;
using System.Collections.Generic;
using System.Text;

namespace uomap_client
{
    class JsonFormatter
    {
        public static string BuildJson(List<GameWindow> windows)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach(var window in windows)
            {
                sb.Append("{");
                sb.AppendFormat("\"name\":\"{0}\",", window.Character.Name);
                sb.AppendFormat("\"server\":\"{0}\",", window.Character.Server);
                sb.AppendFormat("\"x\":{0},", window.Character.X);
                sb.AppendFormat("\"y\":{0},", window.Character.Y);
                sb.AppendFormat("\"z\":{0},", window.Character.Z);
                sb.AppendFormat("\"f\":{0},", window.Character.F);
                sb.AppendFormat("\"active\":{0}", window.IsActiveWindow ? "true" : "false");
                sb.Append("}");
                sb.Append(",");
            }           

            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}
