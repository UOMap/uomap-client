using System;
using System.Collections.Generic;
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
        public bool IsActiveWindow { get; set; }

        public IntPtr OpenHandle { get; set; }

        public Character Character { get; set; }

        public int CharacterAddress { get; set; }
        public int PositionAddress { get; set; }
        public int ServerAddress { get; set; }

        public GameWindow(IntPtr h, string t)
        {
            Handle = h;
            Title = t;
            Character = new Character();
        }

        public bool IsInitialized
        {
            get
            {
                return (CharacterAddress != 0) && (PositionAddress != 0) && (ServerAddress != 0);
            }
        }
    }
}
