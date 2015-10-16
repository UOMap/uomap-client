using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace uomap_client
{
    public class Game
    {      

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private const int ProcessQueryInformation = 0x400;
        private const int ProcessVmRead = 0x10;  
                
        public static void FindWindows(List<GameWindow> windows)
        {            
            var child = IntPtr.Zero;
                        
            child = FindWindowEx(IntPtr.Zero, child, "Ultima Online", null);
            while(child != IntPtr.Zero) 
            {   
                if(!WindowTracked(windows, child))
                {
                    var length = GetWindowTextLength(child);
                    var sb = new StringBuilder(length + 1);
                    GetWindowText(child, sb, sb.Capacity);

                    windows.Add(new GameWindow(child, sb.ToString()));
                }

                child = FindWindowEx(IntPtr.Zero, child, "Ultima Online", null);
            }                      
        }

        public static bool WindowTracked(List<GameWindow> windows, IntPtr handle)
        {
            foreach(var window in windows)
            {
                if(window.SameHandle(handle))
                {
                    return true;
                }
            }

            return false;
        }

        public static int Initialize(GameWindow window)
        { 
            uint processId;            

            GetWindowThreadProcessId(window.Handle, out processId);

            if(processId == 0)
            {
                return -1;
            }

            const int access = ProcessQueryInformation|ProcessVmRead;

            window.OpenHandle = OpenProcess(access, true, (int)processId);

            var process = Process.GetProcessById((int)processId);

            string path = process.MainModule.FileName;            

            var reader = new PeHeaderReader(path);

            var processHandle = window.OpenHandle.ToInt32();            

            for (int i = 0; i < reader.ImageSectionHeaders.Length; i++ )
            {
                var currentSection = reader.ImageSectionHeaders[i];

                if(!currentSection.HasFlag(PeHeaderReader.DataSectionFlags.MemoryExecute))
                {
                    continue;
                }

                var address = currentSection.VirtualAddress + reader.OptionalHeader32.ImageBase;
               
                if (window.PositionAddress == 0)
                {
                    short[] pattern = { 0x8B, 0x15, -1, -1, -1, -1, 0x8B, 0x01, 0x8B, 0x40, 0x54, 0x52, 0x8B, 0x15 };

                     // ~6.0.14.4 - 7.x (Stygian Abyss to High Seas+)
                    if (Pattern.PatternSearch(processHandle, address, currentSection.VirtualSize, pattern) > 0)
                    {                        
                        window.PositionAddress = ((pattern[5] << 24) + (pattern[4] << 16) + (pattern[3] << 8) + pattern[2]) - 4;
                    }                    
                }

                if (window.CharacterAddress == 0)
                {
                    short[] pattern = { 0xE8, -1, -1, -1, -1, 0x68, -1, -1, -1, -1, 0x68, -1, -1, -1, -1, 0x57, 0xE8, -1, -1, -1, -1, 0x83, 0xC4, -1, 0x68, -1, -1, -1, -1 };

                    if (Pattern.PatternSearch(processHandle, address, currentSection.VirtualSize, pattern) > 0)
                    {                        
                        window.ServerAddress = ((pattern[9] << 24) + (pattern[8] << 16) + (pattern[7] << 8) + pattern[6]);
                        window.CharacterAddress = ((pattern[28] << 24) + (pattern[27] << 16) + (pattern[26] << 8) + pattern[25]);
                    }
                }   
    
                // 4.0.11d - ~6.0.9.x (Mondain's Legacy to Kingdom Reborn+)
                if (window.PositionAddress == 0)
                {
                    short[] pattern = { 0x8B, 0x0D, -1, -1, -1, -1, 0x53, 0x55, 0x56, 0x8B, 0x35 };

                     // ~6.0.14.4 - 7.x (Stygian Abyss to High Seas+)
                    if (Pattern.PatternSearch(processHandle, address, currentSection.VirtualSize, pattern) > 0)
                    {                        
                        window.PositionAddress = ((pattern[5] << 24) + (pattern[4] << 16) + (pattern[3] << 8) + pattern[2]) - 4;
                    }                    
                }

                if (window.CharacterAddress == 0)
                {
                    short[] pattern = { 0x83, 0xC4, -1, 0x68, -1, -1, -1, -1, 0x68, -1, -1, -1, -1, 0x57, 0xE8, -1, -1, -1, -1, 0x83, 0xC4, -1, 0x68, -1, -1, -1, -1, 0x68, -1, -1, -1, -1, 0x57, 0xE8, -1, -1, -1, -1, 0x8B, 0x54, 0x24 };

                    if (Pattern.PatternSearch(processHandle, address, currentSection.VirtualSize, pattern) > 0)
                    {                        
                        window.ServerAddress = ((pattern[7] << 24) + (pattern[6] << 16) + (pattern[5] << 8) + pattern[4]);
                        window.CharacterAddress = ((pattern[26] << 24) + (pattern[25] << 16) + (pattern[24] << 8) + pattern[23]);
                    }
                }   
            }
            return 0;
        }

        public static void UpdateClient(GameWindow window)
        {
            var buffer = new byte[32];
            var bytesRead = 0;

            if(!window.IsInitialized)
            {
                return;
            }

            var windowHandle = window.OpenHandle.ToInt32();

            try
            {
                ReadProcessMemory(windowHandle, window.ServerAddress, buffer, buffer.Length, ref bytesRead);
                window.Character.Server = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                ReadProcessMemory(windowHandle, window.CharacterAddress, buffer, buffer.Length, ref bytesRead);
                window.Character.Name = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                var nullIndex = window.Character.Name.IndexOf('\0');
                window.Character.Name = window.Character.Name.Substring(0, nullIndex);

                ReadProcessMemory(windowHandle, window.PositionAddress, buffer, buffer.Length, ref bytesRead);
                window.Character.Z = BitConverter.ToInt32(buffer, 0);
                window.Character.Y = BitConverter.ToInt32(buffer, 4);
                window.Character.X = BitConverter.ToInt32(buffer, 8);
                window.Character.F = BitConverter.ToInt32(buffer, 12);
            }
            catch(Exception ex)
            {
                // The client has closed
                window.ClientClosed = true;

            }

            var foreground = GetForegroundWindow();

            if(foreground.ToInt32() == windowHandle)
            {
                window.IsActiveWindow = true;
            }
            else
            {
                window.IsActiveWindow = false;
            }
        }
    }   
}
