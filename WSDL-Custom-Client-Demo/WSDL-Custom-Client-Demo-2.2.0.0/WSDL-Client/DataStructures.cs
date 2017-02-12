using System;
using System.Collections.Generic;
using System.Text;
//using System.ServiceModel;
using System.Runtime.Serialization;

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
    // This file contains the various data structures used to communicate
    // data between the main run loop and the server


    public struct WxTemperatureRecord
    {
         public double Temperature;
         public double DewPoint;
         public double RH;
    }

    public struct WxLogRecord
    {
        public DateTime When;
        public WxTemperatureRecord[] Temperatures;
        public double GustSpeed;
        public double GustDirection;
        public double AverageSpeed;
        public double AverageDirection;
        public double QnhBarometer;
        public double StationBarometer;
        public double RainRate;
        // for graphing purposes, rain data is either the raw total reported by the
        // station, or one of two adjusted totals: either (1) the raw station total adjusted
        // for reset events plus an initial offset, or (2) a total computed from an initial offset
        // by counting actual tips of the rain gage.
        public double StationRain;
        public double AdjustedRain;
        public double BucketRain; // total rain from bucket tip counting.
        public DateTime RainSince;
        public double UV;
        // fields for storing computed rain-rates. these are only used with the 
        // in-memory copy of the weather log.
        public double StationRainRate;
    }
    
    public enum WxForecast { PartlyCloudy = 0, Rainy = 1, Cloudy = 2, Sunny = 3, Snowy = 4 };

    public struct StationCounters
    {
        public bool ArduinoEnabled;
        public int Usb;
        public int Wu;
        public int Pws;
        public int Cwop;
        public int Ftp;
        public int WuTimer;
        public int PwsTimer;
        public int CwopTimer;
        public int FtpTimer;
    }

    public struct StationDataRecord
    {
        public bool Invalid;
        // this struct will never contain data for more than one temp sensor
        // so we don't need an array of temperature structures.
        public double Temperature;
        public double DewPoint;
        public double RH;
        public double QnhBarometer;
        public double StationBarometer;
        public WxForecast StationForecast;
        public WxForecast QnhForecast;
        public double GustSpeed;
        public double AverageSpeed;
        public double Direction;  // assume this is the current, not avg direction.
        public double RainRate;
        public double RainThisHour;
        public double RainThisDay;
        public double TotalRain;
        public DateTime RainSince;
        public double Uv;
        public DateTime StationClock;
        public bool StationPower;
        public bool StationClockOk;
        public bool StationBattery;
        public bool WindBattery;
        public bool RainBattery;
        public bool TemperatureBattery;
        public bool UvBattery;
        public StationCounters Counters;
    }    
    
    public enum StationRecordType
    {
        Clock, TemperatureHumidity, Temperature, Barometer, Wind, Rain, UV, Counters,
        WindRain, TempRhWind 
    };


    public struct StationData
    {
        public StationRecordType RecordType;
        public int Sensor;
        public StationDataRecord Record;
    }

  
}
