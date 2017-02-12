using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;
using System.Configuration;
using System.Drawing;

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
    // This file defines the various data structures used to communicate
    // between client and server in XML. It also contains a class used to 
    // serialize and deserialize these data structures.

    #region Data Types and Struct Definitions

    public enum XmlWxForecast { PartlyCloudy = 0, Rainy = 1, Cloudy = 2, Sunny = 3, Snowy = 4 };

    // data structure names are always 3 characters with the first two characters
    // always being "Wx". This makes it easy to determine the data type on the receiving
    // end.

    public struct WxB
    {        
        public byte Stat;
        public double QNH;
        public double QFE;
        public XmlWxForecast QfeFcst;
        public XmlWxForecast QnhFcst;
    }

    public struct WxW
    {
        public byte Stat;
        public double Gust;
        public double Avg;
        public double Dir;
    }

    public struct WxP
    {
        public byte Stat;
        public double Rate;
        public double Hour;
        public double Day;
        public double Total;
        public long Since;
    }

    public struct WxR
    {
        public byte Stat;
        public int Ch;
        public double T;
        public double Rh;
        public double Dp;
    }

    public struct WxT
    {
        public byte Stat;
        public int Ch;
        public double T;
    }

    public struct WxU
    {
        public byte Stat;
        public double UV;
    }

    public struct WxM
    {
        public string Msg;
    }

    public struct WxS
    {
        public string[] Msgs;
    }

    //
    // The station (clock) record contains the current clock reading plus
    // status bits on the power and battery condition
    //
    public struct WxC
    {
        public byte Stat;
        public long Clock;
    }

    public struct WxL  // Weather log DATA record
    {
        public string Rec;
    }

    public struct WxF  // raw weather log FILE record
    {
        public string Rec;
    }

    /// <summary>
    /// Used to transfer message counts; source data messages, and upload
    /// counts and timers. All timers are in minutes. These may be sent once
    /// per minute or so -- no need to send them real frequently.
    /// Negative values indicate the counter is not to be displayed.
    /// Optionally, this could be merged with another data struct...???
    /// </summary>
    public struct WxI
    {
        public int UsbCnt;
        public int ArdCnt;
        public int WuCnt;
        public int WuTmr;
        public int CwpCnt;
        public int CwpTmr;
        public int FtpCnt;
        public int FtpTmr;

        // new members starting with release 4.2.1.2
        public int PwsCnt;
        public int PwsTmr;

        [XmlIgnore]public const int Old    = 0x20000000;
        [XmlIgnore]public const int Older  = 0x40000000;
    }

    

    public enum XmlRecordType
    {
        Clock, TemperatureHumidity, Temperature, Barometer, Wind, Rain, UV, 
        Message, Messages, Log, Counters, LogFile, Unknown
    };

    #endregion

    public class WsdlXml
    {
        public const byte StatusValid      = 0x01;
        public const byte StatusPower      = 0x02;
        public const byte StatusClockSync  = 0x04;
        // up to 16 battery levels are supported but right now, 
        // only two states are used.
        public const byte StatusBattery    = 0xF0;
        public const byte StatusBatteryOk  = 0x00;
        public const byte StatusBatteryLow = 0x80;


        private XmlSerializer mBaroXs;
        private XmlSerializer mWindXs;
        private XmlSerializer mRainXs;
        private XmlSerializer mTempXs;
        private XmlSerializer mTempRhXs;
        private XmlSerializer mUvXs;
        private XmlSerializer mClockXs;
        private XmlSerializer mMsgXs;
        private XmlSerializer mMsgsXs;
        private XmlSerializer mLogXs;
        private XmlSerializer mLogFileXs;
        private XmlSerializer mCounterXs;

        public WsdlXml()
        {
            mBaroXs = new XmlSerializer(typeof(WxB));
            mWindXs = new XmlSerializer(typeof(WxW));
            mRainXs = new XmlSerializer(typeof(WxP));
            mTempXs = new XmlSerializer(typeof(WxT));
            mTempRhXs = new XmlSerializer(typeof(WxR));
            mUvXs = new XmlSerializer(typeof(WxU));
            mClockXs = new XmlSerializer(typeof(WxC));
            mMsgsXs = new XmlSerializer(typeof(WxS));
            mMsgXs = new XmlSerializer(typeof(WxM));
            mLogXs = new XmlSerializer(typeof(WxL));
            mLogFileXs = new XmlSerializer(typeof(WxF));
            mCounterXs = new XmlSerializer(typeof(WxI));
        }

        public static string TersifyXml(string s)
        {
            // replace namespace declaration (it is very long)
            s = Regex.Replace(s, " xmlns.*\">", ">");
            // replace the xml declaration which is within a <? ... ?> construct
            s = Regex.Replace(s, "<\\?.*\\?>", "");
            // delete the CR/LF which may be at the beginning 
            s = Regex.Replace(s, "^\r\n", "");
            // delete white space between xml elements
            s = Regex.Replace(s, ">\\s*<", "><");
            // replace any carriage returns or line feeds that remain
            // this used to cause a problem with client settings because the "GraphConfigXML" 
            // string array originally contained line feed separators and this operation screwed that up.
            // however, that all XML uses different separators now, like tab or escape characters.
            s = Regex.Replace(s, "[\r\n]", "");
            return s;
        }

        public byte[] TerseXml(XmlRecordType Kind, object Record)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                switch (Kind)
                {
                    case XmlRecordType.Counters:
                        mCounterXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Barometer:
                        mBaroXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Wind:
                        mWindXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Rain:
                        mRainXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Temperature:
                        mTempXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.TemperatureHumidity:
                        mTempRhXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.UV:
                        mUvXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Clock:
                        mClockXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Message:
                        mMsgXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Messages:
                        mMsgsXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.Log:
                        mLogXs.Serialize(ms, Record);
                        break;
                    case XmlRecordType.LogFile:
                        mLogFileXs.Serialize(ms, Record);
                        break;
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }

            byte[] buf = new byte[ms.Length];
            ms.Seek(0L, SeekOrigin.Begin);
            ms.Read(buf, 0, (int)ms.Length);
            string verbose = Encoding.ASCII.GetString(buf);
            string terse = TersifyXml(verbose);
            //Console.WriteLine(terse);

            buf = Encoding.ASCII.GetBytes(terse);
            return buf;
        }

        public bool DeserializeXml(byte[] Xml, out XmlRecordType Kind, out object Record)
        {
            Kind = XmlRecordType.Unknown;
            Record = null;

            int minLength = 11;

            // minimum possible xml string is "<WxX></WxX>", and that's probably invalid
            if (Xml == null || Xml.Length < minLength) return false;

            if (Xml[0] != (byte)'<' || Xml[1] != (byte)'W' || Xml[2] != (byte)'x' || Xml[4] != (byte)'>')
            {
                return false;
            }

            if (Xml == null) return false;

            MemoryStream ms = new MemoryStream(Xml);
            bool ok = true;

            try
            {
                switch (Xml[3])
                {
                    case (byte)'I':
                        Kind = XmlRecordType.Counters;
                        Record = mCounterXs.Deserialize(ms);
                        break;
                    case (byte)'B':
                        Kind = XmlRecordType.Barometer;
                        Record = mBaroXs.Deserialize(ms);
                        break;
                    case (byte)'W':
                        Kind = XmlRecordType.Wind;
                        Record = mWindXs.Deserialize(ms);
                        break;
                    case (byte)'P':
                        Kind = XmlRecordType.Rain;
                        Record = mRainXs.Deserialize(ms);
                        break;
                    case (byte)'R':
                        Kind = XmlRecordType.TemperatureHumidity;
                        Record = mTempRhXs.Deserialize(ms);
                        break;
                    case (byte)'T':
                        Kind = XmlRecordType.Temperature;
                        Record = mTempXs.Deserialize(ms);
                        break;
                    case (byte)'U':
                        Kind = XmlRecordType.UV;
                        Record = mUvXs.Deserialize(ms);
                        break;
                    case (byte)'C':
                        Kind = XmlRecordType.Clock;
                        Record = mClockXs.Deserialize(ms);
                        break;
                    case (byte)'M':
                        Kind = XmlRecordType.Message;
                        Record = mMsgXs.Deserialize(ms);
                        break;
                    case (byte)'S':
                        Kind = XmlRecordType.Messages;
                        Record = mMsgsXs.Deserialize(ms);
                        break;
                    case (byte)'L':
                        // string s = Encoding.ASCII.GetString(Xml);
                        Kind = XmlRecordType.Log;
                        Record = mLogXs.Deserialize(ms);
                        break;
                    case (byte)'F':
                        Kind = XmlRecordType.LogFile;
                        Record = mLogFileXs.Deserialize(ms);
                        break;
                    default:
                        ok = false;
                        break;
                }
            }
            catch 
            {
                ok = false;
            }

            if (!ok)
            {
                Kind = XmlRecordType.Unknown;
                Record = null;
            }

            return ok;
        }


        private static string SerializedDouble(double X, string fmt, CultureInfo ci)
        {
            if (double.IsNaN(X)) return "";
            string s = X.ToString(fmt, ci);
            return s;
        }

        private static double DeserializeDouble(string s, CultureInfo ci)
        {
            if (s == null || s.Length == 0) return double.NaN;
            double x = double.NaN;
            try
            {
                x = double.Parse(s);
            }
            catch { }
            return x;
        }

        private static int DeserializeInt(string s, CultureInfo ci)
        {
            if (s == null || s.Length == 0) return 0;
            int x = 0;
            try
            {
                x = int.Parse(s);
            }
            catch { }
            return x;
        }

        public static bool DeserializeLogRecord(string s, out WxLogRecord rec)
        {
            rec = new WxLogRecord();
            CultureInfo ic = CultureInfo.InvariantCulture;
            string[] f = s.Split(new char[] { ',' });
            if (f.Length < 14) return false;

            rec.When = new DateTime(long.Parse(f[0]));
            rec.StationBarometer = DeserializeDouble(f[1], ic);
            rec.QnhBarometer = DeserializeDouble(f[2], ic);
            rec.GustSpeed = DeserializeDouble(f[3], ic);
            rec.GustDirection = DeserializeInt(f[4], ic);
            rec.AverageSpeed = DeserializeDouble(f[5], ic);
            rec.AverageDirection = DeserializeInt(f[6], ic);
            rec.RainRate = DeserializeDouble(f[7], ic);
            rec.StationRain = DeserializeDouble(f[8], ic);
            rec.AdjustedRain = DeserializeDouble(f[9], ic);
            rec.BucketRain = DeserializeDouble(f[10], ic);
            rec.RainSince = new DateTime(long.Parse(f[11], ic));
            rec.UV = DeserializeDouble(f[12], ic);
            int n = DeserializeInt(f[13], ic);
            if (n < 0 || n > 10) return false;
            if (f.Length != (14 + 3 * n)) return false;
            rec.Temperatures = new WxTemperatureRecord[n];

            for (int k = 0; k < n; k++)
            {
                rec.Temperatures[k].Temperature = DeserializeDouble(f[3*k + 14], ic);
                rec.Temperatures[k].DewPoint = DeserializeDouble(f[3*k + 15], ic);
                rec.Temperatures[k].RH = DeserializeDouble(f[3*k + 16], ic);
            }

            return true;
        }

        public static string SerializeLogRecord(WxLogRecord s)
        {
            string listSep = ",";
            CultureInfo ic = CultureInfo.InvariantCulture;
            string data = s.When.Ticks.ToString(ic) + listSep;
            data += SerializedDouble(s.StationBarometer, "0.0000", ic) + listSep;
            data += SerializedDouble(s.QnhBarometer, "0.0000", ic) + listSep;
            data += SerializedDouble(s.GustSpeed, "0.000", ic) + listSep;
            data += s.GustDirection.ToString("0", ic) + listSep;
            data += SerializedDouble(s.AverageSpeed, "0.000", ic) + listSep;
            data += s.AverageDirection.ToString("0", ic) + listSep;
            data += SerializedDouble(s.RainRate, "0.000", ic) + listSep;
            data += SerializedDouble(s.StationRain, "0.000", ic) + listSep;
            data += SerializedDouble(s.AdjustedRain, "0.000", ic) + listSep;
            data += SerializedDouble(s.BucketRain, "0.000", ic) + listSep;
            data += s.RainSince.Ticks.ToString("0") + listSep;
            data += SerializedDouble(s.UV, "0", ic) + listSep;

            data += s.Temperatures.Length.ToString("0", ic); // list separator omitted on purpose

            for (int k = 0; k < s.Temperatures.Length; k++)
            {
                WxTemperatureRecord tr = s.Temperatures[k];
                data += listSep + SerializedDouble(tr.Temperature, "0.00", ic);
                data += listSep + SerializedDouble(tr.DewPoint, "0.00", ic);
                data += listSep + SerializedDouble(tr.RH, "0.0", ic);
            }

            return data;
        }

        private static string SettingName(ref string rec)
        {
            if (rec == null || rec[0] != '<') return null;
            int term = rec.IndexOf("/>");
            if (term < 0) return null;
            string name = rec.Substring(1, term - 1);
            rec = rec.Substring(term + 2);
            return name;
        }

        private static string SettingType(string rec)
        {
            if (rec == null || rec[0] != '<') return null;
            int term = rec.IndexOf('>');
            int term2 = rec.IndexOf(' ');

            if (term < 0)
            {
                term = term2;
            }
            else
            {
                if (term2 > 0) term = (term2 < term) ? term2 : term;
            }

            if (term < 0) return null;
            string name = rec.Substring(1, term - 1);
            return name;
        }

        // 
        // Due to the way settings objects are defined, they cannot be conveniently 
        // serialized just by calling the default serializer (as is done in the TerseXml method).
        // Instead, the methods below serialize each member of the settings object individually.
        // These pieces are then concatenated using a separator character (the default is ESCape).
        // The deserializer works in reverse, splitting up the chunks using the separator, then 
        // each piece is de-serialized individually. 
        //
        // These methods are mostly used for transmitting settings data between a WSDL server and 
        // its clients. There is an additional important use however. The user-named stored graph configurations
        // are saved by serializing the WsdlGraphicsSettings object and then saving the string in
        // one element of a string collection. For this purpose, a different separator (the TAB character)
        // is used. This is necessary because these strings (being part of the WsdlClientSettings object)
        // will also be serialized for tranmission from server to client. If the same character is
        // used as a separator for both purposes, things get confused and the client settings object
        // cannot be properly deserialized.
        //

        public static bool DeserializeSettingsObject(string s, ApplicationSettingsBase obj)
        {
            return DeserializeSettingsObject(s, "\u001B", obj);
        }

        public static bool DeserializeSettingsObject(string s, string separator, ApplicationSettingsBase obj)
        {
            XmlSerializer xsInt = new XmlSerializer(typeof(int));
            XmlSerializer xsBool = new XmlSerializer(typeof(bool));
            XmlSerializer xsString = new XmlSerializer(typeof(string));
            XmlSerializer xsStringColl = new XmlSerializer(typeof(System.Collections.Specialized.StringCollection));
            XmlSerializer xsDouble = new XmlSerializer(typeof(double));
            XmlSerializer xsDate = new XmlSerializer(typeof(DateTime));
            XmlSerializer xsPoint = new XmlSerializer(typeof(Point));
            XmlSerializer xsSize = new XmlSerializer(typeof(Size));

            char[] sep = separator.ToCharArray();
            string[] recs = s.Split(sep);
            bool ok = true;
            MemoryStream ms;

            try
            {
                foreach (string rec in recs)
                {
                    string data = rec;
                    string name = SettingName(ref data);
                    if (name == null)
                    {
                        ok = false;
                        break;
                    }
                    string kind = SettingType(data);
                    ms = new MemoryStream(Encoding.ASCII.GetBytes(data));

                    switch (kind)
                    {
                        case "string":
                            obj[name] = xsString.Deserialize(ms);
                            break;
                        case "int":
                            obj[name] = xsInt.Deserialize(ms);
                            break;
                        case "boolean":
                            obj[name] = xsBool.Deserialize(ms);
                            break;
                        case "double":
                            obj[name] = xsDouble.Deserialize(ms);
                            break;
                        case "ArrayOfString":
                            obj[name] = xsStringColl.Deserialize(ms);
                            break;
                        case "dateTime":
                            obj[name] = xsDate.Deserialize(ms);
                            break;
                        case "Point":
                            obj[name] = xsPoint.Deserialize(ms);
                            break;
                        case "Size":
                            obj[name] = xsSize.Deserialize(ms);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                ok = false;
            }
            return ok;
        }

        public static string SerializeSettingsObject(ApplicationSettingsBase obj)
        {
            return SerializeSettingsObject(obj, "\u001B");
        }

        public static string SerializeSettingsObject(ApplicationSettingsBase obj, string separator)
        {
            XmlSerializer xsInt = new XmlSerializer(typeof(int));
            XmlSerializer xsBool = new XmlSerializer(typeof(bool));
            XmlSerializer xsString = new XmlSerializer(typeof(string));
            XmlSerializer xsStringColl = new XmlSerializer(typeof(System.Collections.Specialized.StringCollection));
            XmlSerializer xsDouble = new XmlSerializer(typeof(double));
            XmlSerializer xsDate = new XmlSerializer(typeof(DateTime));
            XmlSerializer xsPoint = new XmlSerializer(typeof(Point));
            XmlSerializer xsSize = new XmlSerializer(typeof(Size));

            IEnumerator e = obj.Properties.GetEnumerator();
            string result = "";
            string sep = "";
            MemoryStream ms = new MemoryStream();

            while (e.MoveNext())
            {
                SettingsProperty sp = (SettingsProperty)e.Current; ;

                object prop = obj[sp.Name];
                Type ptype = prop.GetType();
                
                switch (ptype.Name)
                {
                    case "Boolean":
                        xsBool.Serialize(ms, prop);
                        break;
                    case "Int32":
                        xsInt.Serialize(ms, prop);
                        break;
                    case "Double":
                        xsDouble.Serialize(ms, prop);
                        break;
                    case "DateTime":
                        xsDate.Serialize(ms, prop);
                        break;
                    case "String":
                        xsString.Serialize(ms, prop);
                        break;
                    case "StringCollection":
                        xsStringColl.Serialize(ms, prop);
                        break;
                    case "Point":
                        xsPoint.Serialize(ms, prop);
                        break;
                    case "Size":
                        xsSize.Serialize(ms, prop);
                        break;
                    default:
                        break;
                }
                ms.Flush();
                byte[] buf = ms.GetBuffer(); 
                long len = ms.Length;
                byte[] bites = new byte[len];
                Array.Copy(buf, bites, len);
                string xml = Encoding.ASCII.GetString(bites);
                string terse = TersifyXml(xml);
                result += sep + "<" + sp.Name + "/>" + terse;
                sep = separator;
                ms.SetLength(0L);                
            }

            return result;
        }

    }

 
}
