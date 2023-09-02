using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace png2xnb.Models
{
    class BinaryWriterWrapper : IDisposable
    {
        private BinaryWriter bw;

        public BinaryWriterWrapper(BinaryWriter bw)
        {
            this.bw = bw;
        }

        public void WriteByte(byte b)
        {
            bw.Write(b);
        }

        public void WriteInt(int v)
        {
            bw.Write(v);
        }

        public void WriteChars(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            bw.Write(bytes);
        }

        public void Write7BitEncodedInt(int i)
        {
            while (i >= 0x80)
            {
                WriteByte((byte)(i & 0xff));
                i >>= 7;
            }
            WriteByte((byte)i);
        }

        public void WriteString(string s)
        {
            Write7BitEncodedInt(s.Length);
            WriteChars(s);
        }

        public void WriteColor(Color c)
        {
            WriteByte(c.R);
            WriteByte(c.G);
            WriteByte(c.B);
            WriteByte(c.A);
        }

        public void WriteByteArray(byte[] data)
        {
            bw.Write(data);
        }

        public void Close()
        {
            bw.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
