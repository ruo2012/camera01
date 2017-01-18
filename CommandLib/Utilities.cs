using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CommandLib
{
    public class Utilities
    {
        static string RootPath = System.Windows.Forms.Application.StartupPath;
        static string FileName = RootPath + "\\Config.ini";
        static string ImagePath = Application.StartupPath + "\\Snapshot\\";
        static string VideoPath = Application.StartupPath + "\\Video\\";

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        public static string ReadIni(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
            string s = Encoding.GetEncoding(0).GetString(Buffer);
            s = s.Substring(0, bufLen);
            return s.Trim();
        }


    }
}
