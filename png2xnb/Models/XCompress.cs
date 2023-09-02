using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace png2xnb.Models
{
    class XCompress
    {
        public enum XMEMCODEC_TYPE
        {
            XMEMCODEC_DEFAULT = 0,
            XMEMCODEC_LZX = 1
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct XMEMCODEC_PARAMETERS_LZX
        {
            [FieldOffset(0)]
            public int Flags;
            [FieldOffset(4)]
            public int WindowSize;
            [FieldOffset(8)]
            public int CompressionPartitionSize;
        }

        [DllImport("xcompress32.dll", EntryPoint = "XMemCompress")]
        public static extern int XMemCompress(int Context,
                                              byte[] pDestination, ref int pDestSize,
                                              byte[] pSource, int pSrcSize);

        [DllImport("xcompress32.dll", EntryPoint = "XMemCreateCompressionContext")]
        public static extern int XMemCreateCompressionContext(
            XMEMCODEC_TYPE CodecType, ref XMEMCODEC_PARAMETERS_LZX pCodecParams,
            int Flags, ref int pContext);

        [DllImport("xcompress32.dll", EntryPoint = "XMemDestroyCompressionContext")]
        public static extern void XMemDestroyCompressionContext(int Context);

        [DllImport("xcompress32.dll", EntryPoint = "XMemDecompress")]
        public static extern int XMemDecompress(int Context,
                                                byte[] pDestination, ref int pDestSize,
                                                byte[] pSource, int pSrcSize);

        [DllImport("xcompress32.dll", EntryPoint = "XMemCreateDecompressionContext")]
        public static extern int XMemCreateDecompressionContext(
            XMEMCODEC_TYPE CodecType,
            ref XMEMCODEC_PARAMETERS_LZX pCodecParams,
            int Flags, ref int pContext);

        [DllImport("xcompress32.dll", EntryPoint = "XMemDestroyDecompressionContext")]
        public static extern void XMemDestroyDecompressionContext(int Context);

        public static byte[] Compress(byte[] decompressedData)
        {
            // Setup our compression context
            int compressionContext = 0;

            XMEMCODEC_PARAMETERS_LZX codecParams;
            codecParams.Flags = 0;
            codecParams.WindowSize = 64 * 1024;
            codecParams.CompressionPartitionSize = 256 * 1024;

            XMemCreateCompressionContext(
                XMEMCODEC_TYPE.XMEMCODEC_LZX,
                ref codecParams, 0, ref compressionContext);

            // Now lets compress
            int compressedLen = decompressedData.Length * 2;
            byte[] compressed = new byte[compressedLen];
            int decompressedLen = decompressedData.Length;
            XMemCompress(compressionContext,
                compressed, ref compressedLen,
                decompressedData, decompressedLen);
            // Go ahead and destory our context
            XMemDestroyCompressionContext(compressionContext);

            // Resize our compressed data
            Array.Resize(ref compressed, compressedLen);

            // Now lets return it
            return compressed;
        }

        public static byte[] Decompress(byte[] compressedData, byte[] decompressedData)
        {
            // Setup our decompression context
            int DecompressionContext = 0;

            XMEMCODEC_PARAMETERS_LZX codecParams;
            codecParams.Flags = 0;
            codecParams.WindowSize = 64 * 1024;
            codecParams.CompressionPartitionSize = 256 * 1024;

            XMemCreateDecompressionContext(
                XMEMCODEC_TYPE.XMEMCODEC_LZX,
                ref codecParams, 0, ref DecompressionContext);

            // Now lets decompress
            int compressedLen = compressedData.Length;
            int decompressedLen = decompressedData.Length;
            try
            {
                XMemDecompress(DecompressionContext,
                    decompressedData, ref decompressedLen,
                    compressedData, compressedLen);
            }
            catch
            {
            }
            // Go ahead and destory our context
            XMemDestroyDecompressionContext(DecompressionContext);
            // Return our decompressed bytes
            return decompressedData;
        }

        public static bool isItAvailable()
        {
            try
            {
                Compress(new byte[1]);
                return true;
            }
            catch (DllNotFoundException e)
            {
                if (e.Message.Contains("xcompress32.dll"))
                {
                    return false;
                }
                throw e;
            }
        }
    }
}
