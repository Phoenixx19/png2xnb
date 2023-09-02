using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace png2xnb.Models
{
    public class XNBManager
    {
        private static string TEXTURE_2D_TYPE = "Microsoft.Xna.Framework.Content.Texture2DReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553";
        private static int HEADER_SIZE = 3 + 1 + 1 + 1;
        private static int COMPRESSED_FILE_SIZE = 4;
        private static int TYPE_READER_COUNT_SIZE = 1;
        private static int TYPE_SIZE = 2 + TEXTURE_2D_TYPE.Length + 4;
        private static int SHARED_RESOURCE_COUNT_SIZE = 1;
        private static int OBJECT_HEADER_SIZE = 21;

        private static int METADATA_SIZE = HEADER_SIZE + COMPRESSED_FILE_SIZE + TYPE_READER_COUNT_SIZE + TYPE_SIZE + SHARED_RESOURCE_COUNT_SIZE + OBJECT_HEADER_SIZE;

        private static int ImageSize(Bitmap png)
        {
            return 4 * png.Height * png.Width;
        }

        private static int CompressedFileSize(Bitmap png)
        {
            return METADATA_SIZE + ImageSize(png);
        }

        private static void WriteCompressedData(BinaryWriterWrapper bw, Bitmap png, bool premultiply)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] uncompressedData;
                using (BinaryWriterWrapper mw = new BinaryWriterWrapper(new BinaryWriter(stream)))
                {
                    WriteData(png, mw, premultiply);
                    uncompressedData = stream.ToArray();
                }
                byte[] compressedData = XCompress.Compress(uncompressedData);
                bw.WriteInt(6 + 4 + 4 + compressedData.Length); // compressed file size including headers
                bw.WriteInt(uncompressedData.Length); // uncompressed data size (exluding headers! only the data)
                bw.WriteByteArray(compressedData);
            }
        }

        private static void WriteData(Bitmap png, BinaryWriterWrapper bw, bool premultiply)
        {
            bw.Write7BitEncodedInt(1);       // type-reader-count
            bw.WriteString(TEXTURE_2D_TYPE); // type-reader-name
            bw.WriteInt(0);                  // reader version number
            bw.Write7BitEncodedInt(0);       // shared-resource-count
                                             // writing the image pixel data
            bw.WriteByte(1); // type id + 1 (referencing the TEXTURE_2D_TYPE)
            bw.WriteInt(0);  // surface format; 0=color
            bw.WriteInt(png.Width);
            bw.WriteInt(png.Height);
            bw.WriteInt(1); // mip count
            bw.WriteInt(ImageSize(png)); // number of bytes in the image pixel data
            BitmapData bitmapData = png.LockBits(new Rectangle(0, 0, png.Width, png.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                var length = bitmapData.Stride * bitmapData.Height;
                byte[] bytes = new byte[length];
                Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
                for (int i = 0; i < bytes.Length; i += 4)
                {
                    // always swap red and blue channels
                    // premultiply alpha if requested
                    int a = bytes[i + 3];
                    if (!premultiply || a == 255)
                    {
                        // No premultiply necessary
                        byte b = bytes[i];
                        bytes[i] = bytes[i + 2];
                        bytes[i + 2] = b;
                    }
                    else if (a != 0)
                    {
                        byte b = bytes[i];
                        bytes[i] = (byte)(bytes[i + 2] * a / 255);
                        bytes[i + 1] = (byte)(bytes[i + 1] * a / 255);
                        bytes[i + 2] = (byte)(b * a / 255);
                    }
                    else
                    {
                        // alpha is zero, so just zero everything
                        bytes[i] = 0;
                        bytes[i + 1] = 0;
                        bytes[i + 2] = 0;
                    }
                }
                bw.WriteByteArray(bytes);
            }
            finally
            {
                png.UnlockBits(bitmapData);
            }
        }

        public static int Convert(string pngFile, string xnbFile, bool compressed, bool reach, bool premultiply)
        {
            using (Bitmap png = new Bitmap(pngFile))
            {
                using (FileStream outFs = new FileStream(xnbFile, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriterWrapper bw = new BinaryWriterWrapper(new BinaryWriter(outFs)))
                    {
                        bw.WriteChars("XNB"); // format-identifier
                        bw.WriteChars("w");   // target-platform
                        bw.WriteByte((byte)5);  // xnb-format-version
                        byte flagBits = 0;
                        if (!reach)
                        {
                            flagBits |= 0x01;
                        }
                        if (compressed)
                        {
                            flagBits |= 0x80;
                        }
                        bw.WriteByte(flagBits); // flag-bits; 00=reach, 01=hiprofile, 80=compressed, 00=uncompressed
                        if (compressed)
                        {
                            WriteCompressedData(bw, png, premultiply);
                        }
                        else
                        {
                            bw.WriteInt(CompressedFileSize(png)); // compressed file size
                            WriteData(png, bw, premultiply);
                        }
                        return 1;
                    }
                }
            }
        }
    }
}
