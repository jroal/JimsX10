using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;

//============================================================================
//Weather Station Data Logger  Copyright © 2010 Weber Anderson
// 
//
//This application is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 3 of the License, or (at your option) any later version.
//
//This application is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, see <http://www.gnu.org/licenses/>
//
//=============================================================================

namespace WsdlClientInterface
{
    public class ClientServerComms
    {
        internal MD5 mHasher = null;
        internal int mCompressThreshold = 1500;

        internal bool SendTcpBytes(byte[] Buffer, NetworkStream ns)
        {
            if (Buffer == null || Buffer.Length == 0) return true;
            byte[] buf = AddHashLayer(Buffer, false); // not necessary since compression adds CRC
            buf = Compress(buf);
            string slen = buf.Length.ToString("0000000000");
            byte[] blen = Encoding.ASCII.GetBytes(slen);
            try
            {
                ns.WriteTimeout = 20000;
                ns.Write(blen, 0, blen.Length);
                ns.Write(buf, 0, buf.Length);
                return true;
            }
            catch { }
            return false;
        }

        internal bool SendTcpString(string s, NetworkStream ns)
        {
            return SendTcpBytes(Encoding.ASCII.GetBytes(s), ns);
        }

        internal byte[] Expand(byte[] Buffer)
        {
            // minimum message from this perspective is a 1-byte flag followed by a 1-byte message
            if (Buffer == null || Buffer.Length < 2) return null;

            byte[] buf;
            //
            // all UDP packets start with a 1-byte header which is non-zero if
            // the packet contents were compressed with gzip.
            //
            if (Buffer[0] == 0)
            {
                buf = new byte[Buffer.Length - 1];
                Array.Copy(Buffer, 1, buf, 0, buf.Length);
                return buf;
            }

            buf = new byte[2 * Buffer.Length];
            byte[] xbuf = new byte[Buffer.Length - 1];
            Array.Copy(Buffer, 1, xbuf, 0, xbuf.Length);
            MemoryStream ms = new MemoryStream(xbuf);
            

            GZipStream gz = new GZipStream(ms, CompressionMode.Decompress);
            int dest = 0;
            int read;
            int rqst;
            //
            // if the buffer is not a valid compressed stream,
            // gzip will throw an exception
            //
            try
            {
                while (true)
                {
                    rqst = buf.Length - dest;
                    read = gz.Read(buf, dest, rqst);
                    dest += read;
                    if (dest >= buf.Length)
                    {
                        byte[] btmp = new byte[buf.Length + Buffer.Length];
                        Array.Copy(buf, btmp, buf.Length);
                        buf = btmp;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {
                buf = null;
            }
            gz.Close();
            ms.Close();

            if (buf == null) return null;
            byte[] answer = new byte[dest];
            Array.Copy(buf, answer, dest);
            return answer;
        }

        internal byte[] VerifyHash(byte[] Buf)
        {
            byte[] msg;
            byte[] buf;

            if (Buf == null || Buf.Length < 2) return null;

            if (Buf[0] == 0)
            {
                buf = new byte[Buf.Length - 1];
                Array.Copy(Buf, 1, buf, 0, buf.Length);
                return buf;
            }
            //
            // minimum msg length now is the hash length plus one byte
            //
            int hashLength = mHasher.HashSize >> 3;
            if (Buf.Length < (hashLength + 1)) return null;

            byte[] cksum = new byte[hashLength]; // size of the MD5 hash
            Array.Copy(Buf, Buf.Length - hashLength, cksum, 0, hashLength);
            msg = new byte[Buf.Length - hashLength - 1];
            Array.Copy(Buf, 1, msg, 0, msg.Length);
            byte[] hash = mHasher.ComputeHash(msg);
            if (!CompareHash(cksum, hash))
            {
                return null;
            }
            return msg;
        }

        internal bool CompareHash(byte[] x, byte[] y)
        {
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            bool ok = true;
            for (int k = 0; k < x.Length; k++)
            {
                ok = ok & (x[k] == y[k]);
            }
            return ok;
        }

        internal byte[] ReadTcpBytes(NetworkStream ns)
        {
            int nread;
            try
            {
                //
                // sometimes part of the response does not occur until
                // the server has done some work, like serializing and
                // compressing the weather log for example. this timeout
                // is sized for this situation.
                //
                ns.ReadTimeout = 30000;
                byte[] blen = new byte[10];
                nread = ns.Read(blen, 0, 10);
                string slen = Encoding.ASCII.GetString(blen);
                int len = int.Parse(slen);
                ns.ReadTimeout = 15000;

                byte[] buf = new byte[len];
                int k = 0;
                do
                {
                    nread = ns.Read(buf, k, (int)buf.Length - k);
                    k += nread;
                } while (nread > 0 && k < len);

                if (k < len) return null;

                buf = Expand(buf);
                buf = VerifyHash(buf);
                return buf;
            }
            catch { }
            return null;
        }

        internal string ReadTcpString(NetworkStream ns)
        {
            byte[] buf = ReadTcpBytes(ns);
            if (buf == null) return null;
            string s = null;
            try
            {
                s = Encoding.ASCII.GetString(buf);
            }
            catch
            {
                s = null;
            }
            return s;
        }

        internal byte[] Compress(byte[] Buffer)
        {
            byte[] buf;
            byte compressed = 0;

            if (Buffer.Length > mCompressThreshold)
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    GZipStream gz = new GZipStream(ms, CompressionMode.Compress);
                    gz.Write(Buffer, 0, Buffer.Length);
                    gz.Close();
                    buf = ms.ToArray();
                    ms.Close();
                    compressed = 0xFF;
                }
                catch
                {
                    compressed = 0;
                    buf = Buffer;
                }
            }
            else
            {
                buf = Buffer;
            }
            byte[] answer = new byte[buf.Length + 1];
            answer[0] = compressed;
            Array.Copy(buf, 0, answer, 1, buf.Length);
            return answer;
        }

        internal byte[] AddHashLayer(byte[] Buf, bool UseHash)
        {
            if (Buf == null || Buf.Length == 0) return null;
            
            byte[] cat;
            byte[] hash = null;

            if (UseHash)
            {
                hash = mHasher.ComputeHash(Buf);
                cat = new byte[Buf.Length + hash.Length + 1];
                cat[0] = 0xFF;
            }
            else
            {
                cat = new byte[Buf.Length + 1];
                cat[0] = 0;
            }

            Array.Copy(Buf, 0, cat, 1, Buf.Length);

            if (UseHash)
            {
                Array.Copy(hash, 0, cat, Buf.Length + 1, hash.Length);
            }

            return cat;
        }

        public static int DefaultUdpPort { get { return 9981; } }

        public static int DefaultTcpPort { get { return 9982; } }

    }
}
