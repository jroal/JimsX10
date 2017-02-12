using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
// using System.ServiceModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Configuration;

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
    public enum KindOfSettings { Server, FtpGraphics, Client, Graphics };

    public class WsdlClient : ClientServerComms
    {
        Queue mStationDataQueue;
        Queue mWxLogQueue;
        Queue mWxLogFileQueue;
        Queue mMessageQueue;
        Socket mSocket;
        int mUdpPort;
        IPEndPoint mEndPoint;
        WsdlXml mXml;
        Thread mRxThread;
        private bool mAbortAllThreads = false;

        public WsdlClient(int PortNumber,
            ref Queue StationData, ref Queue WxLog, ref Queue WxLogFile, ref Queue Messages)
        {
            mStationDataQueue = StationData;
            mWxLogQueue = WxLog;
            mWxLogFileQueue = WxLogFile;
            mMessageQueue = Messages;
            mXml = new WsdlXml();
            mHasher = MD5.Create();

            mUdpPort = PortNumber;
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            mEndPoint = new IPEndPoint(IPAddress.Any, mUdpPort);
            mSocket.Bind(mEndPoint);

            mRxThread = new Thread(new ThreadStart(Receiver));
            mRxThread.Start();
        }

        /// <summary>
        /// This will find the IP address of the WSDL server if it is out there.
        /// It works by trying to receive one of the UDP packets broadcast by the 
        /// server, and traces back the sender's address. The server should be 
        /// sending packets at least once per minute (log file records), so a
        /// 90-second timeout should be adequate to decide if there's a server around.
        /// After a packet is received, an attempt is made to decode it to verify that
        /// it really came from a WSDL server.
        /// </summary>
        /// <param name="PortNumber"></param>
        /// <returns></returns>
        public static IPAddress FindWsdlServerAddress(int PortNumber)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, PortNumber);
            EndPoint rmtep = (EndPoint)ep;
            
            Socket rx = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            rx.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            rx.ReceiveTimeout = 90000;
            rx.Bind(ep);
            IPAddress serverAddr = null;

            bool gotit = false;
            DateTime started = DateTime.UtcNow;
            WsdlXml xml = new WsdlXml();
            XmlRecordType kind;
            object record;
            TimeSpan timeLimit = new TimeSpan(0,1,30);
            byte[] buf = new byte[4096];
            ClientServerComms csc = new ClientServerComms();
            csc.mHasher = MD5.Create();

            do
            {
                try
                {
                    // will throw exception if a timeout occurs
                    int cnt = rx.ReceiveFrom(buf, ref rmtep);
                    if (cnt <= 0) continue;
                    // receiving a UDP packet on the desired port does not prove it came from
                    // the server. try to de-serialize it as a final verification
                    byte[] msg = new byte[cnt];
                    Array.Copy(buf, msg, cnt);
                    msg = csc.Expand(msg);
                    if (msg == null) continue;
                    msg = csc.VerifyHash(msg);
                    if (msg == null) continue;
                    bool validData = xml.DeserializeXml(msg, out kind, out record);
                    if (validData)
                    {
                        serverAddr = ((IPEndPoint)rmtep).Address;
                        gotit = true;
                    }
                } 
                catch { gotit = false; }
            } while (!gotit && ((DateTime.UtcNow - started) < timeLimit));

            rx.Close();
            return gotit ? serverAddr : null;
        }

        private void Receiver()
        {
            while (!mAbortAllThreads)
            {
                byte[] btmp = new byte[4096];
                int nRead;

                // the delay here is somewhat arbitrary. this will poll for incoming data 
                // roughly about 17 times per second. a good compromise between response
                // time and unnecessary overhead
                //
                while (mSocket.Available <= 0 && !mAbortAllThreads) Thread.Sleep(57);

                if (mAbortAllThreads) break;

                try
                {

                    nRead = mSocket.Receive(btmp);
                    byte[] buf = new byte[nRead];
                    Array.Copy(btmp, buf, nRead);

                    buf = Expand(buf);
                    if (buf == null) continue;
                    buf = VerifyHash(buf);
                    if (buf == null) continue;

                    XmlRecordType kind;
                    object record;

                    if (mAbortAllThreads) break;

                    bool ok = mXml.DeserializeXml(buf, out kind, out record);
                    if (!ok) continue;

                    StationData sd = new StationData();

                    if (mAbortAllThreads) break;

                    switch (kind)
                    {
                        case XmlRecordType.Counters:
                            WxI ctr = (WxI)record;
                            sd.Record.Counters.ArduinoEnabled = (ctr.ArdCnt >= 0);
                            sd.Record.Counters.Usb = (ctr.ArdCnt > ctr.UsbCnt) ? ctr.ArdCnt : ctr.UsbCnt;
                            sd.Record.Counters.Cwop = ctr.CwpCnt;
                            sd.Record.Counters.CwopTimer = ctr.CwpTmr;
                            sd.Record.Counters.Ftp = ctr.FtpCnt;
                            sd.Record.Counters.FtpTimer = ctr.FtpTmr;
                            sd.Record.Counters.Wu = ctr.WuCnt;
                            sd.Record.Counters.WuTimer = ctr.WuTmr;
                            sd.Record.Counters.Pws = ctr.PwsCnt;
                            sd.Record.Counters.PwsTimer = ctr.PwsTmr;
                            sd.RecordType = StationRecordType.Counters;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Barometer:
                            WxB baro = (WxB)record;
                            sd.Record.StationBarometer = baro.QFE;
                            sd.Record.QnhBarometer = baro.QNH;
                            sd.Record.QnhForecast = (WxForecast)baro.QnhFcst;
                            sd.Record.StationForecast = (WxForecast)baro.QfeFcst;
                            sd.RecordType = StationRecordType.Barometer;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Clock:
                            WxC clk = (WxC)record;
                            sd.Record.StationClock = new DateTime(clk.Clock);
                            sd.Record.StationClockOk = (clk.Stat & WsdlXml.StatusClockSync) != 0;
                            sd.Record.StationPower = (clk.Stat & WsdlXml.StatusPower) != 0;
                            sd.Record.StationBattery = (clk.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.Clock;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Rain:
                            WxP rain = (WxP)record;
                            sd.Record.RainRate = rain.Rate;
                            sd.Record.RainSince = new DateTime(rain.Since);
                            sd.Record.RainThisDay = rain.Day;
                            sd.Record.RainThisHour = rain.Hour;
                            sd.Record.RainBattery = (rain.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.Rain;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Temperature:
                            WxT t = (WxT)record;
                            sd.Record.Temperature = t.T;
                            sd.Sensor = t.Ch;
                            sd.Record.TemperatureBattery = (t.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.Temperature;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.TemperatureHumidity:
                            WxR th = (WxR)record;
                            sd.Record.Temperature = th.T;
                            sd.Record.RH = th.Rh;
                            sd.Record.DewPoint = th.Dp;
                            sd.Sensor = th.Ch;
                            sd.Record.TemperatureBattery = (th.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.TemperatureHumidity;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.UV:
                            WxU uv = (WxU)record;
                            sd.Record.Uv = uv.UV;
                            sd.Record.UvBattery = (uv.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.UV;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Wind:
                            WxW wind = (WxW)record;
                            sd.Record.AverageSpeed = wind.Avg;
                            sd.Record.Direction = wind.Dir;
                            sd.Record.GustSpeed = wind.Gust;
                            sd.Record.WindBattery = (wind.Stat & WsdlXml.StatusBattery) == WsdlXml.StatusBatteryLow;
                            sd.RecordType = StationRecordType.Wind;
                            mStationDataQueue.Enqueue(sd);
                            break;
                        case XmlRecordType.Log:
                            WxL l = (WxL)record;
                            mWxLogQueue.Enqueue(l.Rec);
                            break;
                        case XmlRecordType.LogFile:
                            WxF f = (WxF)record;
                            mWxLogFileQueue.Enqueue(f.Rec);
                            break;
                        case XmlRecordType.Messages:
                            WxS msgs = (WxS)record;
                            foreach (string s in msgs.Msgs)
                            {
                                mMessageQueue.Enqueue(s);
                            }
                            break;
                        case XmlRecordType.Message:
                            WxM msg = (WxM)record;
                            mMessageQueue.Enqueue(msg.Msg);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    // no need to do anything, just continue
                }
            }
        }

        public bool Disconnect()
        {
            mAbortAllThreads = true;
            if (mRxThread == null) return true;
            try
            {
                mRxThread.Abort();
                bool killed = mRxThread.Join(1500);
                if (killed) mRxThread = null;
                return killed;
            }
            catch { }
            return false;
        }

        public byte[] GetWeatherLog(IPAddress Address, int Port)
        {
            TcpClient client = null;
            
            try
            {
                client = new TcpClient(Address.ToString(), Port);
                NetworkStream ns = client.GetStream();
                ns.WriteTimeout = 3000;
                ns.WriteByte((byte)'L'); // request a log file dump

                byte[] buf = ReadTcpBytes(ns);
                return buf;
            }
            catch (Exception e) 
            {
                string s = e.ToString();
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }
            return null;
        }

        public bool GetSettings(IPAddress Address, int Port, 
            KindOfSettings Kind, ApplicationSettingsBase Settings)
        {
            TcpClient client = null;
            byte rqst;
            bool ok = false;

            switch (Kind)
            {
                case KindOfSettings.Server:
                    rqst = (byte)'s';
                    break;
                case KindOfSettings.FtpGraphics:
                    rqst = (byte)'f';
                    break;
                case KindOfSettings.Client:
                    rqst = (byte)'c';
                    break;
                case KindOfSettings.Graphics:
                    rqst = (byte)'g';
                    break;
                default:
                    return false;                    
            }

            try
            {
                client = new TcpClient(Address.ToString(), Port);
                NetworkStream ns = client.GetStream();
                ns.WriteTimeout = 5000;
                ns.ReadTimeout  = 8000;
                ns.WriteByte(rqst); // request server options
                string s = ReadTcpString(ns);
                if (s == null)
                {
                    ok = false;
                }
                else
                {
                    ok = WsdlXml.DeserializeSettingsObject(s, Settings);
                }
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }
            return ok;
        }

        public bool PutSettings(IPAddress Address, int Port,
            KindOfSettings Kind, ApplicationSettingsBase Settings)
        {
            TcpClient client = null;
            byte rqst;

            switch (Kind)
            {
                case KindOfSettings.Server:
                    rqst = (byte)'S';
                    break;
                case KindOfSettings.FtpGraphics:
                    rqst = (byte)'F';
                    break;
                default:
                    return false;
            }

            try
            {
                client = new TcpClient(Address.ToString(), Port);
                NetworkStream ns = client.GetStream();
                ns.WriteTimeout = 3000;
                ns.WriteByte(rqst); // request server options
                string s = WsdlXml.SerializeSettingsObject(Settings);
                if (s == null) return false;
                bool ok = SendTcpString(s, ns);
                return ok;
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }
            return false;
        }

    }
}
