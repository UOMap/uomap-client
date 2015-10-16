using System.Runtime.InteropServices;

namespace uomap_client
{
    public class Pattern
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, 
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static int PatternSearch(int processHandle, uint address, uint length, short[] pattern)
        {
            var current = (int)address;
            var patternLength = pattern.Length / 2;            

            var buffer = new byte[102400];

             while (current < address + length)
            {
                var bytesRead = 0;
                var patternCurrent = 0;

                if (!ReadProcessMemory(processHandle, current, buffer, buffer.Length, ref bytesRead))
                    return 0;

                for (var i = 0; i < bytesRead; i++)
                {
                    // if the current byte does not equal the current pattern value and the current pattern value is not -1
                    // back up the byte array index to
                    // then reset the pattern match to the beginning
                    if (buffer[i] != (byte)pattern[patternCurrent] && pattern[patternCurrent] >= 0) {
                        i -= patternCurrent;
                        patternCurrent = 0;
                        continue;
                    }

                    patternCurrent++;

                    if (patternCurrent == pattern.Length) {
                        for (int j = 0; j <  pattern.Length; j++)
                            pattern[j] = buffer[i -  pattern.Length + j + 1];
                        return current + (i - patternLength) + 1;
                    }
                }

                current -= pattern.Length;
                current += bytesRead;
            }

            return 0;
        }
    }
}