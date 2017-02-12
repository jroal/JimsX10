﻿//============================================================================
//Thermostat Control Form  Copyright © 2009, Jim Roal
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Diagnostics;
using WsdlClientInterface;

namespace JimsX10
{
    public struct tempMinMax
    {
        public double tempMin;
        public double tempMax;
        public double DPMin;
        public double DPMax;
        public double RHMin;
        public double RHMax;
        public double ATMin;
        public double ATMax;
        public double HIMax;
        public double WCMin;
    }
    
    public partial class TStat : Form
    {
        CheckBox[] sensorName = new CheckBox[11]; // array of checkboxes with sensor names
        public Label[] sensorTemp = new Label[11]; // array of temperatures
        public Label[] sensorDP = new Label[11]; // array of dew point labels
        public Label[] sensorRH = new Label[11]; // array of relative himidity labels
        public Label[] sensorAT = new Label[11]; // array of apparent temperature labels
        public Label[] sensorHI = new Label[11]; // array of heat index labels
        public Label[] sensorWC = new Label[11]; // array of wind chill labels
        public double[] sensorTempNum = new double[11]; // numeric temp values used for calucations
        public double[] sensorDPNum = new double[11]; // dew points
        public double[] sensorRHNum = new double[11]; // relative himidities
        public double[] sensorATnum = new double[11]; // Apparent temperature values
        public double[] sensorHInum = new double[11]; // Heat index temperature values
        public double[] sensorWCnum = new double[11]; // Wind Chill temperature values
        public tempMinMax[] tMinMax = new tempMinMax[11]; // array of min/max values
        double wind; // store the latest average wind
        public WxUnits mWxDataUnits;
        public WxUnits mWxDisplayUnits;
        public double Rain24; // rain in the last 24 hours.  Used to control sprinkler override.
        public double AveIndoorTempValue; // keep the average of the checked temp boxes
        

        DateTime last_run = new DateTime(); // used for compressor timer
        int BarnSensorNum = 1; // assume the barn uses outside temp for now.  Sensor can be selected later.
        int FireSensorNum = 1; //same as above
        public int errorcount = 0;
        int ERRORMAX = 3;
        WsdlClient mClient;     // receives data from the server
        Queue mDataQ;           // weather data broadcasts are stored in this queue
        Queue mJunkQ;           // used to hold weather log updates and messages (we don't use this data)
        // this timer will call our processing function every 10 seconds.
        //
        System.Windows.Forms.Timer mTimer;
        System.Windows.Forms.Timer checkTimer;
        //
        //
        // information about temperatures used sensorTimeoutLimit make decisions about fan on/off states.
        //        
        DateTime[] lasttemp = new DateTime[10];        // last time an currentInsideTemp temperature reading was received
        TimeSpan sensorTimeoutLimit = new TimeSpan(0, 10, 0);
 
        // label colors used sensorTimeoutLimit indicate status
        //
        Color okColor = Color.LimeGreen;
        Color hotColor = Color.Firebrick;
        Color oldColor = Color.Yellow;
        Color offColor = Color.DarkGreen;
        //
        

        public TStat()
        {
            InitializeComponent();
            //
            // create the WSDL client sensorTimeoutLimit capture UDP broadcast packets. data received will
            // be stored in one of the queues for later examination by the timer tick function.
            // If you have changed the WSDL Server's UDP port then change the first argument sensorTimeoutLimit 
            // the WsdlClient constructor below sensorTimeoutLimit match.
            //
            mDataQ = new Queue();
            mJunkQ = new Queue();

            mClient = new WsdlClient(ClientServerComms.DefaultUdpPort,
                ref mDataQ, ref mJunkQ, ref mJunkQ, ref mJunkQ);
            //
            // start up a timer sensorTimeoutLimit call the Process() function every 10 seconds
            //
            mTimer = new System.Windows.Forms.Timer();
            mTimer.Tick += new EventHandler(Process);
            mTimer.Interval = 10000;  // check temperatures every 10 seconds
            mTimer.Start();

            checkTimer = new System.Windows.Forms.Timer();
            checkTimer.Tick += new EventHandler(processCheck);
            checkTimer.Interval = 30000; //confirm check box state every 30s
            checkTimer.Start();

            last_run = DateTime.Now; // this is used to time the compressor off time to prevent rapid start
            wind = 0;
            mWxDisplayUnits.Temperature = TemperatureUnit.degF;
            mWxDisplayUnits.Wind = SpeedUnit.mi_per_hr;
            for (int j = 0; j < lasttemp.Length; j++)
            {
                lasttemp[j] = DateTime.MinValue;
            }
        }

        private void CoolingDesiredMax_changed(object sender, EventArgs e)
        {
            // make sure the heating and cooling don't fight each other
            if (HeatDesiredMin.Value >= CoolingDesiredMax.Value) HeatDesiredMin.Value = CoolingDesiredMax.Value - 1;
            UpdateStatus();
        }

        private void HeatDesiredMin_Changed(object sender, EventArgs e)
        {
            // make sure the heating and cooling don't fight each other
            if (CoolingDesiredMax.Value <= HeatDesiredMin.Value) CoolingDesiredMax.Value = HeatDesiredMin.Value + 1;
            UpdateStatus();
        }

        public void TStat_Load(object sender, EventArgs e)
        {


            // create the controls for each of the defined temperature/humidity sensors
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; i++)
            {
                sensorName[i] = new CheckBox();
                this.ActCondGroup.Controls.Add(sensorName[i]);
                sensorName[i].Location = new System.Drawing.Point(35, 40 + 26 * i);
                sensorName[i].CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
                sensorName[i].TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                sensorName[i].Text = "Sensor " + i; //Properties.Settings.Default.SensorNames[i];
                // make an attempt to find indoor sensors.  OK, so it's a cheesy attempt.  Nice to have something better.
                //if (!Properties.Settings.Default.SensorNames[i].Contains("Out")) sensorName[i].Checked = true;
                sensorName[i].CheckedChanged += new EventHandler(TStat_TextChanged);
                
                sensorTemp[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorTemp[i]);
                sensorTemp[i].Text = "N/A";
                sensorTemp[i].Width = 50;
                sensorTemp[i].Location = new System.Drawing.Point(150, 45 + 26 * i);
                sensorTemp[i].TextChanged += new EventHandler(TStat_TextChanged);
                

                sensorDP[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorDP[i]);
                sensorDP[i].Text = "N/A";
                sensorDP[i].Width = 50;
                sensorDP[i].Location = new System.Drawing.Point(230, 45 + 26 * i);

                sensorRH[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorRH[i]);
                sensorRH[i].Location = new System.Drawing.Point(290, 45 + 26 * i);
                sensorRH[i].Text = "N/A";
                sensorRH[i].Width = 50;
                sensorRH[i].TextChanged += new EventHandler(TStat_TextChanged);

                sensorAT[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorAT[i]);
                sensorAT[i].Location = new System.Drawing.Point(360, 45 + 26 * i);
                sensorAT[i].Text = "N/A";
                sensorAT[i].Width = 50;

                sensorHI[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorHI[i]);
                sensorHI[i].Location = new System.Drawing.Point(430, 45 + 26 * i);
                sensorHI[i].Text = "N/A";
                sensorHI[i].Width = 50;

                sensorWC[i] = new Label();
                this.ActCondGroup.Controls.Add(sensorWC[i]);
                sensorWC[i].Location = new System.Drawing.Point(500, 45 + 26 * i);
                sensorWC[i].Text = "N/A";
                sensorWC[i].Width = 50;

                BarnSensor.Items.Add(sensorName[i].Text); // add this sensor to the barn control selection
                FireSensor.Items.Add(sensorName[i].Text); // add this sensor to the fireplace contorl
                
            }
                load_settings();
                Rain24 = 0;
                resetMiinMaxToolStripMenuItem_Click(null, null);
        }

        private void load_settings()
        {
            // load all defaults
            CoolAddress.Text = Properties.Settings.Default.TStatCoolAdd;
            HeatAddress.Text = Properties.Settings.Default.TStatHeatAdd;
            FanAddress.Text = Properties.Settings.Default.TStatMixAdd;
            SprinklerAddress.Text = Properties.Settings.Default.TStatRainAdd;
            HumidifierAddress.Text = Properties.Settings.Default.TStatHumidAdd;
            CoolingDesiredMax.Value = Properties.Settings.Default.TStatMaxTemp;
            HeatDesiredMin.Value = Properties.Settings.Default.TStatMinTemp;
            MaxSplit.Value = Properties.Settings.Default.TStatMaxSplit;
            RainThresh.Text = Properties.Settings.Default.TStatRainMin;
            DesHumidity.Value = Properties.Settings.Default.TStatHumidity;
            UseAppTemp.Checked = Properties.Settings.Default.TStatUsaApp;
            BarnCoolAdd.Text = Properties.Settings.Default.TStatBarnCoolAdd;
            BarnDesMax.Value = Properties.Settings.Default.TStatBarnDesMax;
            BarnDesMin.Value = Properties.Settings.Default.TStatBarnDesMin;
            BarnHeatAdd.Text = Properties.Settings.Default.TStatBarnHeatAdd;
            BarnUseAppTemp.Checked = Properties.Settings.Default.TStatBarnUseApp;
            BarnCoolAdd.Text = Properties.Settings.Default.TStatBarnCoolAdd;
            BarnHeatAdd.Text = Properties.Settings.Default.TStatBarnHeatAdd;
            BarnSensorNum = Properties.Settings.Default.TStatBarnSensor;
            BarnSensor.SelectedIndex = BarnSensorNum;
            FreshAdd.Text = Properties.Settings.Default.TStatFreshAdd;
            MaxInDP.Value = Properties.Settings.Default.TStatMaxDP;
            AutoHumidity.Checked = Properties.Settings.Default.AutoDesHumidity;
            FireSensorNum = Properties.Settings.Default.FireSensor;
            FireSensor.SelectedIndex = FireSensorNum;
            FireCommandOnce.Checked = Properties.Settings.Default.FireSendCommandOnce;
            FireplaceAddress.Text = Properties.Settings.Default.FireplaceAdd;
            FireplaceFanAddress.Text = Properties.Settings.Default.FireplaceFanAdd;
            FireOutdoorMin.Value = Properties.Settings.Default.FireOutdoorMin;
            FireDesiredMin.Value = Properties.Settings.Default.FireDesiredMin;
            FireHysteresis.Value = Properties.Settings.Default.FireHysteresis;
            FireplaceFanCheckbox.Checked = Properties.Settings.Default.UseFireFan;
            for(int i = 0;i<10 ; i++)
            {
                bool chk = Convert.ToBoolean( Properties.Settings.Default.CheckBoxes[i]);
                sensorName[i].Checked = chk ;
                String test = Properties.Settings.Default.CheckBoxes[i];
            }
            fehrenheitToolStripMenuItem.Checked = Properties.Settings.Default.TUnitsdegF;
            // make sure the current check mark matches the actual state
            CoolCompressor.Checked = X10check(CoolCompressor.Checked, CoolAddress.Text);
            MixFan.Checked = X10check(MixFan.Checked, FanAddress.Text);
            Heat.Checked = X10check(Heat.Checked, HeatAddress.Text);
            HumidifierCheck.Checked = X10check(HumidifierCheck.Checked, HumidifierAddress.Text);
            SprinklersBox.Checked = X10check(SprinklersBox.Checked, SprinklerAddress.Text);
            BarnHeat.Checked = X10check(BarnHeat.Checked, BarnHeatAdd.Text);
            BarnCool.Checked = X10check(BarnCool.Checked, BarnCoolAdd.Text);
            FreshAirFan.Checked = X10check(FreshAirFan.Checked, FreshAdd.Text);
            
            

        }

        private void SaveSet_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TStatCoolAdd = CoolAddress.Text;
            Properties.Settings.Default.TStatHeatAdd = HeatAddress.Text;
            Properties.Settings.Default.TStatMixAdd = FanAddress.Text;
            Properties.Settings.Default.TStatRainAdd = SprinklerAddress.Text;
            Properties.Settings.Default.TStatHumidAdd = HumidifierAddress.Text;
            Properties.Settings.Default.TStatMaxTemp = CoolingDesiredMax.Value;
            Properties.Settings.Default.TStatMinTemp = HeatDesiredMin.Value;
            Properties.Settings.Default.TStatMaxSplit = MaxSplit.Value;
            Properties.Settings.Default.TStatRainMin = RainThresh.Text;
            Properties.Settings.Default.TStatHumidity = DesHumidity.Value;
            Properties.Settings.Default.TStatUsaApp = UseAppTemp.Checked;
            Properties.Settings.Default.TStatBarnCoolAdd = BarnCoolAdd.Text;
            Properties.Settings.Default.TStatBarnDesMax = BarnDesMax.Value;
            Properties.Settings.Default.TStatBarnDesMin = BarnDesMin.Value;
            Properties.Settings.Default.TStatBarnHeatAdd = BarnHeatAdd.Text;
            Properties.Settings.Default.TStatBarnUseApp = BarnUseAppTemp.Checked;
            Properties.Settings.Default.TStatBarnCoolAdd = BarnCoolAdd.Text;
            Properties.Settings.Default.TStatBarnHeatAdd = BarnHeatAdd.Text;
            Properties.Settings.Default.TStatBarnSensor = BarnSensorNum;
            Properties.Settings.Default.FireSensor = FireSensorNum;
            Properties.Settings.Default.TStatFreshAdd = FreshAdd.Text;
            Properties.Settings.Default.TStatMaxDP = MaxInDP.Value;
            Properties.Settings.Default.AutoDesHumidity = AutoHumidity.Checked;
            Properties.Settings.Default.FireSendCommandOnce = FireCommandOnce.Checked;
            Properties.Settings.Default.FireplaceAdd = FireplaceAddress.Text;
            Properties.Settings.Default.FireplaceFanAdd = FireplaceFanAddress.Text;
            Properties.Settings.Default.FireOutdoorMin = FireDesiredMin.Value;
            Properties.Settings.Default.FireDesiredMin = FireDesiredMin.Value;
            Properties.Settings.Default.FireHysteresis = FireHysteresis.Value;
            Properties.Settings.Default.UseFireFan = FireplaceFanCheckbox.Checked;
            for (int i = 0; i < 10; i++)
            {
                Properties.Settings.Default.CheckBoxes[i] = Convert.ToString(sensorName[i].Checked);
            }
            Properties.Settings.Default.TUnitsdegF = fehrenheitToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }
        void TStat_TextChanged(object sender, EventArgs e)
        {
            //UpdateStatus();
            //this.Refresh();
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            //this.Visible = false;  // hide this control
            this.Close();
            mClient.Disconnect();
            this.Dispose();
        }
        private void HideButton_Click(object sender, FormClosingEventArgs e)
        {
            //this.Visible = false;  // hide this control
            mClient.Disconnect();
            this.Dispose();
        }

        public void UpdateStatus()
        {
            double min = 999, max = -999, temp;
            double MaxDes = Convert.ToDouble( CoolingDesiredMax.Value);
            double MinDes = Convert.ToDouble( HeatDesiredMin.Value);
            double diff;
            bool ok = false;
            string u = WxTemperatureUnit.ToString(mWxDisplayUnits.Temperature);

            int areok = 0;
            int cnt = 0;
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; i++)
            {
                if (sensorName[i].Checked)
                {
                    ++cnt;
                    if (UseAppTemp.Checked) // if this is checked, control to apparent temp not actual
                        temp = WxTemperatureUnit.Convert(sensorATnum[i], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
                    else
                        temp = WxTemperatureUnit.Convert(sensorTempNum[i], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
                    if (temp > max) max = temp;
                    if (temp < min) min = temp;
                    if (sensorRHNum[i] > 0) ++areok;
                } 
            }

            if (cnt == areok) ok = true;
            else ok = false;

 
            // calculate the apparent temps, wind chill, and heat index and update the display
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; ++i)
            {
                sensorAT[i].Text = WxTemperatureUnit.DspString(sensorATnum[i], TemperatureUnit.degC, mWxDisplayUnits.Temperature) + u;
                sensorHI[i].Text = WxTemperatureUnit.DspString(sensorHInum[i], TemperatureUnit.degF, mWxDisplayUnits.Temperature) + u;
                sensorWC[i].Text = WxTemperatureUnit.DspString(sensorWCnum[i], TemperatureUnit.degF, mWxDisplayUnits.Temperature) + u;               
            }
            AveIndoorTempValue = AveIndoorTemp(); // just update the ave temp.  May dump this later if I don't need it.
            
            // now update the outputs
            // Note:  The reason these are all if..else if... is for hysteresis to prevent excessive toggling
            if (max > MaxDes && ok)  // if it's too hot, cool it off
            {               
                if((DateTime.Now - last_run).TotalSeconds > 30.0) // make sure we don't cycle the compressor too fast.
                    CoolCompressor.Checked = true;
                else
                    CoolCompressor.Checked = false;
            }
            else if(max+1.0 < MaxDes)
            {
                 CoolCompressor.Checked = false;
                 last_run = DateTime.Now; // reset compressor off timer
            }

            if (min < MinDes && !CoolCompressor.Checked && ok) // if it's too cold, and the a/c is not ON, heat it up
                 Heat.Checked = true;
            else if(min-1 > MinDes)
                 Heat.Checked = false;

            // if the temp split is too high then mix the air between the floors/rooms.
            if (ok)
                diff = max - min;
            else
                diff = 0; 
            
            if (diff > Convert.ToDouble(MaxSplit.Value) && ok)
                 MixFan.Checked = true;
            else if(diff+1.0 < Convert.ToDouble(MaxSplit.Value))
                 MixFan.Checked = false; 


            
            // reset the max desired humdity based on outdoor temp
            if(AutoHumidity.Checked) DesHumidity.Value = Convert.ToDecimal(MaxDesHum());
            if (DesHumidity.Value > Convert.ToDecimal(MaxDesHum()))
                DesHumudityLabel.BackColor = Color.Red;
            else
                DesHumudityLabel.BackColor = Color.LightGreen;

            
            //check to see if we should enable the humidifier
            if (AveIndoorHum() < Convert.ToDouble(DesHumidity.Value) && ok)
                HumidifierCheck.Checked = true;
            else if (AveIndoorHum()+1.0 > Convert.ToDouble(DesHumidity.Value) && ok)
                HumidifierCheck.Checked = false;

            // should we bring in fresh air?  Always use apparent temp for this
            // this assumes sensor 1 is the outside sensor
            double outside = WxTemperatureUnit.Convert(sensorTempNum[1], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
            double OutDP = WxTemperatureUnit.Convert(sensorDPNum[1], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
            double DesMaxDP = Convert.ToDouble(MaxInDP.Value);
            if (((max >= MaxDes - 1 && outside < max)||(min <= MinDes + 1 && outside > min)) && OutDP < DesMaxDP && ok) 
                FreshAirFan.Checked = true;
            else
                FreshAirFan.Checked = false;

            // compare the 24 hour rain to the threshold
            try
            {
                
                if (Rain24 < Convert.ToDouble(RainThresh.Text))
                {
                    SprinklersBox.Checked = true;
                }
                else
                {
                    SprinklersBox.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Check rain threshold: " + ex);
            }
            Rain24Label.Text = "24hr Actual Rain: " + Rain24.ToString("#0.00in");
           
            UpdateLabel.Text = "Updated: " + DateTime.Now.ToLongTimeString();

            // take care of the barn now
            if (sensorRHNum[BarnSensorNum] > 0)
            {
                double BarnTemp;
                if (BarnUseAppTemp.Checked)
                    BarnTemp = WxTemperatureUnit.Convert(sensorATnum[BarnSensorNum], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
                else
                    BarnTemp = WxTemperatureUnit.Convert(sensorTempNum[BarnSensorNum], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);

                if (BarnTemp > Convert.ToDouble(BarnDesMax.Value))
                    BarnCool.Checked = true;
                else if (BarnTemp + 2.0 < Convert.ToDouble(BarnDesMax.Value))
                    BarnCool.Checked = false;

                if (BarnTemp < Convert.ToDouble(BarnDesMin.Value))
                    BarnHeat.Checked = true;
                else if (BarnTemp - 1.0 > Convert.ToDouble(BarnDesMin.Value))
                    BarnHeat.Checked = false;
            }

            //Take care of the fireplace
            if (sensorRHNum[FireSensorNum] > 0)
            {
                double outsideTemp = WxTemperatureUnit.Convert(sensorTempNum[FireSensorNum], mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
                if (outsideTemp < Convert.ToDouble(FireOutdoorMin.Value))
                {
                    if (AveIndoorTempValue < Convert.ToDouble(FireDesiredMin.Value))
                        Fireplace.Checked = true;
                }
            }

        }

        private double AveIndoorTemp()
        {
            double sum = 0, count = 0, ave = 0;
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; i++)
            {
                if (sensorName[i].Checked)
                {
                    ++count;
                    if(UseAppTemp.Checked) // if this is checked, control to apparent temp not actual
                        sum += sensorATnum[i];
                    else
                        sum += sensorTempNum[i];

                }
            }
            if (count > 0.9) // make sure no devide by 0
                ave = sum / count;
            else
                ave = -100;

            if (ave > -100)
                AveIndTempLabel.Text = "Ave Indoor Temp: " + WxTemperatureUnit.DspString(ave, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature)
                    + " F"; // update status label
            else
                AveIndTempLabel.Text = "Not yet calculated";
            return WxTemperatureUnit.Convert(ave, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature);
        }

        private double AveIndoorHum()
        {
            double sum = 0, count = 0, ave = 0;
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; i++)
            {
                if (sensorName[i].Checked)
                {
                    ++count;
                    sum += sensorRHNum[i];
                }
            }
            if (count > 0.9) // make sure no devide by 0
                ave = sum / count;
            else
                ave = -100;

            if (ave > -100)
            {
                AveHumLabel.Text = "Average Indoor Humidity: " + ave.ToString("#0.0") + "%"; // update status label
                return ave;
            }
            else
            {
                AveHumLabel.Text = "Not yet calculated";
                return 40; // use a default
            }
        }

        private void CoolCompressor_CheckedChanged(object sender, EventArgs e)
        {
            if (CoolCompressor.Checked)  // if it's too hot, cool it off
            {
                if (X10ON(true, CoolAddress.Text))
                {
                    CoolCompressor.Checked = true;
                    AppendToFile("Cooling Compressor ON, " + CoolAddress.Text + ",1");
                }
                else
                {
                    CoolAddress.Text = "";
                    AppendToFile("Cooling Compressor ON Failed");
                }
            }
            else
            {
                if (X10ON(false, CoolAddress.Text))
                {
                    CoolCompressor.Checked = false;
                    AppendToFile("Cooling Compressor OFF, " + CoolAddress.Text + ",0");
                }
                else
                {
                    CoolAddress.Text = "";
                    AppendToFile("Cooling Compressor OFF Failed");
                }
            }
        }

        private void MixFan_CheckedChanged(object sender, EventArgs e)
        {
            // The mix fan tries to balance the indoor temps
            if (MixFan.Checked )
            {
                if (X10ON(true, FanAddress.Text))
                {
                    MixFan.Checked = true;
                    AppendToFile("Mix Fan ON, " + FanAddress.Text + ",1");
                }
                else
                {
                    FanAddress.Text = "";
                    AppendToFile("Mix Fan ON Failed");
                }
            }
            else
            {
                if (X10ON(false, FanAddress.Text))
                {
                    MixFan.Checked = false;
                    AppendToFile("Mix Fan OFF, " + FanAddress.Text + ",0");
                }
                else
                {
                    FanAddress.Text = "";
                    AppendToFile("Mix Fan OFF Failed" );
                }
            }

        }

        private void Heat_CheckedChanged(object sender, EventArgs e)
        {
            if (Heat.Checked && !CoolCompressor.Checked) // if it's too cold, and the a/c is not ON, heat it up
            {
                if (X10ON(true, HeatAddress.Text))
                {
                    Heat.Checked = true;
                    AppendToFile("Heat ON, " + HeatAddress.Text + ",1");
                }
                else
                {
                    HeatAddress.Text = "";
                    AppendToFile("Heat ON Failed");
                }
            }
            else
            {
                if (X10ON(false, HeatAddress.Text))
                {
                    Heat.Checked = false;
                    AppendToFile("Heat OFF, " + HeatAddress.Text + ",0");
                }
                else
                {
                    HeatAddress.Text = "";
                    AppendToFile("Heat OFF Failed ");
                }
            }
        }

        private void HumidifierCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (HumidifierCheck.Checked) // if it's too dry enable the humidifier
            {
                if (X10ON(true, HumidifierAddress.Text))
                {
                    HumidifierCheck.Checked = true;
                    AppendToFile("Humidifier ON, " + HumidifierAddress.Text + ",1");
                }
                else
                {
                    HumidifierAddress.Text = "";
                    AppendToFile("Humidifier ON Failed" );
                }
            }
            else
            {
                if (X10ON(false, HumidifierAddress.Text))
                {
                    HumidifierCheck.Checked = false;
                    AppendToFile("Humidifier OFF, " + HumidifierAddress.Text + ",0");
                }
                else
                {
                    HumidifierAddress.Text = "";
                    AppendToFile("Humidifier OFF Failed");
                }
            }
        }

 

 
        private void SprinklersBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SprinklersBox.Checked)
            {
                if (X10ON(true, SprinklerAddress.Text))
                {
                    SprinklersBox.Checked = true;
                    AppendToFile("Sprinklers ON, " + SprinklerAddress.Text + ",1");
                }
                else
                {
                    SprinklerAddress.Text = "";
                    AppendToFile("Sprinklers ON Failed" );
                }
            }
            else
            {
                if (X10ON(false, SprinklerAddress.Text))
                {
                    SprinklersBox.Checked = false;
                    AppendToFile("Sprinklers OFF, " + SprinklerAddress.Text + ",0");
                }
                else
                {
                    SprinklerAddress.Text = "";
                    AppendToFile("Sprinklers OFF Failed " );
                }
            }

        }



        private double MaxDesHum()
        {
            //calculates the max desired humidity to prevent mold indoors.  Simplified to a liner equation.
            double max;
            for (int i = 0; i < Properties.Settings.Default.TempSensorCount; i++)
            {
                if (!sensorName[i].Checked) // find the outside temp sensor
                {
                    max = WxTemperatureUnit.Convert(sensorTempNum[i],mWxDataUnits.Temperature, TemperatureUnit.degF) * 0.6 + 26.0;
                    // set to lowest of configured desired or max
                    if (max < Convert.ToDouble(Properties.Settings.Default.TStatHumidity))
                        return max;
                    else
                        return Convert.ToDouble(Properties.Settings.Default.TStatHumidity);
                }
            }
                return 40.0;
        }
        private bool AppendToFile(string data)
        {
            StreamWriter sw = null;
            try
            {
                sw = System.IO.File.AppendText(Properties.Settings.Default.LogFilePath + "HomeControlLog.csv");
            }
            catch { }
            if (sw == null) return false;

            bool ok = true;
            try { sw.WriteLine(DateTime.Now.ToString("u") + ",  " + data); }
            catch { ok = false; }
            finally { sw.Close(); }
            return ok;
        }

        private void exportLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String file = Properties.Settings.Default.LogFilePath + "HomeControlLog.csv";
            if(File.Exists(file))
            {
                saveFileDialog1.Filter = "CSV File|*.csv";
                saveFileDialog1.Title = "Save log File";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    if (File.Exists(saveFileDialog1.FileName)) File.Delete(saveFileDialog1.FileName);
                    File.Copy(file, saveFileDialog1.FileName);
                }
            }
        }

        private void deleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String file = Properties.Settings.Default.LogFilePath + "HomeControlLog.csv";
            if (File.Exists(file)) File.Delete(file);
        }

        private void Reload_Click(object sender, EventArgs e)
        {
            load_settings();
            errorcount = 0;
        }

        private void BarnCool_CheckedChanged(object sender, EventArgs e)
        {
            if (BarnCool.Checked) // Cool the barn
            {
                if (X10ON(true, BarnCoolAdd.Text))
                {
                    BarnCool.Checked = true;
                    AppendToFile("Barn Cool ON, " + BarnCoolAdd.Text + ",1");
                }
                else
                {
                    BarnCoolAdd.Text = "";
                    AppendToFile("Barn Cool ON Failed");
                }
            }
            else
            {
                if (X10ON(false, BarnCoolAdd.Text))
                {
                    BarnCool.Checked = false;
                    AppendToFile("Barn Cool OFF, " + BarnCoolAdd.Text + ",0");
                }
                else
                {
                    BarnCoolAdd.Text = "";
                    AppendToFile("Barn Cool OFF Failed");
                }
            }
        }

        private void BarnHeat_CheckedChanged(object sender, EventArgs e)
        {
            if (BarnHeat.Checked) // Heat the barn
            {
                if (X10ON(true, BarnHeatAdd.Text))
                {
                    BarnHeat.Checked = true;
                    AppendToFile("Barn Heat ON, " + BarnHeatAdd.Text + ",1");
                }
                else
                {
                    BarnHeatAdd.Text = "";
                    AppendToFile("Barn Heat ON Failed");
                }
            }
            else
            {
                if (X10ON(false, BarnHeatAdd.Text))
                {
                    BarnHeat.Checked = false;
                    AppendToFile("Barn Heat OFF, " + BarnHeatAdd.Text + ",0");
                }
                else
                {
                    BarnHeatAdd.Text = "";
                    AppendToFile("Barn Heat OFF Failed");
                }
            }
        }

        private void BarnSensor_SelectedIndexChanged(object sender, EventArgs e)
        {      
            for (int i = 0;i < Properties.Settings.Default.TempSensorCount; i++)
            {
                if (BarnSensor.Text == sensorName[i].Text) BarnSensorNum = i;
            }
        }

        private void BarnDesMax_ValueChanged(object sender, EventArgs e)
        {
            if (BarnDesMin.Value > BarnDesMax.Value) BarnDesMin.Value = BarnDesMax.Value - 1;
        }

        private void BarnDesMin_ValueChanged(object sender, EventArgs e)
        {
            if (BarnDesMin.Value > BarnDesMax.Value) BarnDesMax.Value = BarnDesMin.Value + 1;           
        }

        private void FreshAirFan_CheckedChanged(object sender, EventArgs e)
        {
            if (FreshAirFan.Checked) // turn ON fresh air
            {
                if (X10ON(true, FreshAdd.Text))
                {
                    FreshAirFan.Checked = true;
                    AppendToFile("Fresh Air Fan ON, " + FreshAdd.Text + ",1");
                }
                else
                {
                    FreshAdd.Text = "";
                    AppendToFile("Fresh Air Fan ON Failed");
                }
            }
            else
            {
                if (X10ON(false, FreshAdd.Text))
                {
                    FreshAirFan.Checked = false;
                    AppendToFile("Fresh Air Fan OFF, " + FreshAdd.Text + ",0");
                }
                else
                {
                    FreshAdd.Text = "";
                    AppendToFile("Fresh Air Fan OFF Failed");
                }
            }
        }

        private void Fireplace_checkedchanged(object sender, EventArgs e) // used to run the fireplace
        {
            if (Fireplace.Checked) // Control the fireplace and fan.
            {
                if (X10ON(true, FireplaceAddress.Text))
                {
                    Fireplace.Checked = true;
                    AppendToFile("Fireplace ON, " + FireplaceAddress.Text + ",1");
                    if (FireplaceFanCheckbox.Checked && X10Address(FireplaceFanAddress.Text) )
                    {
                        if(X10ON(true, FireplaceFanAddress.Text))
                        {
                            FireplaceFan.Checked = true;
                        }
                    }
                }
                else
                {
                    FireplaceAddress.Text = "";
                    AppendToFile("Fireplace ON Failed");
                }
            }
            else
            {
                if (X10ON(false, FireplaceAddress.Text))
                {
                    Fireplace.Checked = false;
                    AppendToFile("Fireplace OFF, " + FireplaceAddress.Text + ",0");
                    if (FireplaceFanCheckbox.Checked && X10Address(FireplaceFanAddress.Text))
                    {
                        if (X10ON(false, FireplaceFanAddress.Text)) // should put a timer here to run fan for some time after fire
                        {
                            FireplaceFan.Checked = false;
                        }
                    }
                }
                else
                {
                    FireplaceAddress.Text = "";
                    AppendToFile("Fireplace OFF Failed");
                }
            }
        }


        private bool X10ON(bool ON, string address)
        {
            ActiveHomeScriptLib.ActiveHomeClass act = new ActiveHomeScriptLib.ActiveHomeClass();
            // access the X10 device to enable change


            if (X10Address(address))
            {
                try
                {
                    object act2 = act.SendAction("queryplc", address + " on", null, null);
                    bool isON = act2.Equals(1);

                    if(!ON && isON) // if not supposed to be ON but is
                      act.SendAction("sendplc", address + " off", null, null);
                    else if(ON && !isON) // if is ON but should not be
                      act.SendAction("sendplc", address + " on", null, null);

                    errorcount = 0;
                    return true;
                }
                catch (Exception ex)
                {
                    AppendToFile("Error accessing X10 device: " + address);
                    ++errorcount;
                    if (errorcount > ERRORMAX)
                    {
                        //   MessageBox.Show("Error accessing X10 device: " + address + "    " + ex);
                    }
                    return false;
                }
            }
            else
                return true;
        }
        private bool X10check(bool state, string address)
        {
            // check the current state of an X10 device
            
            ActiveHomeScriptLib.ActiveHomeClass act = new ActiveHomeScriptLib.ActiveHomeClass();
            bool rtn;
            if (X10Address(address))
            {
                try
                {
                    if (act.SendAction("queryplc", address + " on", null, null).Equals(1))
                       rtn = true;
                    else
                        rtn = false;
                    errorcount = 0;
                    return rtn;
                }
                catch (Exception ex)
                {
                    AppendToFile("Error accessing X10 device: " + address);
                    ++errorcount;
                    if (errorcount > ERRORMAX)
                    {
                        MessageBox.Show("Error accessing X10 device: " + address + "    " + ex);
                    }
                    return state;
                }
                
            }
            else
                return state;
        }

        private bool X10Address(string add)
        {
            // make sure addresses are valid
            try
            {
                char[] address = add.ToCharArray(0, add.Length);

                if (add.Length < 2 || add.Length > 3) return false;
                if ((address[0] < 'a' || address[0] > 'z') && (address[0] < 'A' || address[0] > 'Z')) return false;
                int num = Convert.ToInt16(add.Substring(1, add.Length - 1));
                if (num < 0 || num > 16) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void processCheck(object sender, EventArgs e)
        {
            // make sure the current check mark matches the actual state
            CoolCompressor.Checked = X10check(CoolCompressor.Checked, CoolAddress.Text);
            MixFan.Checked = X10check(MixFan.Checked, FanAddress.Text);
            Heat.Checked = X10check(Heat.Checked, HeatAddress.Text);
            HumidifierCheck.Checked = X10check(HumidifierCheck.Checked, HumidifierAddress.Text);
            SprinklersBox.Checked = X10check(SprinklersBox.Checked, SprinklerAddress.Text);
            BarnHeat.Checked = X10check(BarnHeat.Checked, BarnHeatAdd.Text);
            BarnCool.Checked = X10check(BarnCool.Checked, BarnCoolAdd.Text);
            FreshAirFan.Checked = X10check(FreshAirFan.Checked, FreshAdd.Text);
        }
 
                /// <summary>
        /// This function runs periodically (called by the timer tick event) and
        /// is responsible for analyzing temperature readings and turning fans 
        /// on and off as necessary. 
        /// 
        /// This is where you can add whatever custom behaviors are desired. 
        /// Although this function looks for temperature readings, you can 
        /// grab any weather data of interest...barometer, wind, rain...whatever.
        /// 
        /// This function uses X10 switches to control fans but you can delete that
        /// code and do anything else you want to such as play sounds, dial phone
        /// numbers, send e-mails, start programs, etc. The only problem is -- you 
        /// have to figure out how! Good Luck, and please feel free to post your 
        /// examples on the WSDL SourceForge forums for others to see.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="evargs"></param>
        public void Process(object source, EventArgs evargs)
        {
            //
            // A timout value of 10 minutes is used to determine if contact
            // has been lost with the wireless sensors.
            //
            TimeSpan sensorTimeoutLimit = new TimeSpan(0, 10, 0);
            //
            // just throw out any data in the junk queue since we don't need it.
            // this data will be weather log updates and messages.
            //
            while (mJunkQ.Count > 0) mJunkQ.Dequeue();
            //
            // entries in the data queue are what we are really interested in
            //
            string u = WxTemperatureUnit.ToString(mWxDisplayUnits.Temperature);

            while (mDataQ.Count > 0)
            {
                StationData sd = (StationData)mDataQ.Dequeue(); // get the next record in queue
                //
                // we only care about temperature/humidity data from the indoor and outdoor channels
                //
                double wnd = wind;
                if (sd.RecordType == StationRecordType.TemperatureHumidity)
                {
                    sensorTemp[sd.Sensor].Text = WxTemperatureUnit.DspString(
                            sd.Record.Temperature, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                    sensorDP[sd.Sensor].Text = WxTemperatureUnit.DspString(
                            sd.Record.DewPoint, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                    sensorRH[sd.Sensor].Text = sd.Record.RH.ToString("##0.0") + "%";

                    sensorTempNum[sd.Sensor] = sd.Record.Temperature;
                    sensorRHNum[sd.Sensor] = sd.Record.RH;
                    sensorDPNum[sd.Sensor] = sd.Record.DewPoint;
                    if (sensorName[sd.Sensor].Checked) // no wind inside
                        wnd = 0;
                    else
                        wnd = wind;
                    sensorATnum[sd.Sensor] = Moisture.AppTemp(WxTemperatureUnit.Convert(sd.Record.Temperature,
                                mWxDataUnits.Temperature, TemperatureUnit.degC),sd.Record.RH, wnd);
                    sensorHInum[sd.Sensor] = Moisture.HeatIndex(WxTemperatureUnit.Convert(sd.Record.Temperature,
                                mWxDataUnits.Temperature, TemperatureUnit.degF), sd.Record.RH,
                                WxTemperatureUnit.Convert(sd.Record.DewPoint,
                                mWxDataUnits.Temperature, TemperatureUnit.degF));
                    sensorWCnum[sd.Sensor] = Moisture.WindChill(WxTemperatureUnit.Convert(sd.Record.Temperature,
                                mWxDataUnits.Temperature, TemperatureUnit.degF), WxSpeedUnit.Convert(wnd, SpeedUnit.m_per_sec, SpeedUnit.mi_per_hr));

                    DateTime now = DateTime.UtcNow;
                    lasttemp[sd.Sensor] = now;
                    for (int k = 0; k < sensorName.Length - 1; k++)
                    {
                        TimeSpan sinceTemp = now - lasttemp[k];
                        sensorName[k].BackColor = (sinceTemp > sensorTimeoutLimit) ? oldColor : okColor;
                    }
 
                    // store all the min/max data
                    if (sensorATnum[sd.Sensor] > tMinMax[sd.Sensor].ATMax) tMinMax[sd.Sensor].ATMax = sensorATnum[sd.Sensor];
                    if (sensorATnum[sd.Sensor] < tMinMax[sd.Sensor].ATMin) tMinMax[sd.Sensor].ATMin = sensorATnum[sd.Sensor];
                    if (sensorTempNum[sd.Sensor] > tMinMax[sd.Sensor].tempMax) tMinMax[sd.Sensor].tempMax = sensorTempNum[sd.Sensor];
                    if (sensorTempNum[sd.Sensor] < tMinMax[sd.Sensor].tempMin) tMinMax[sd.Sensor].tempMin = sensorTempNum[sd.Sensor];
                    if (sensorDPNum[sd.Sensor] > tMinMax[sd.Sensor].DPMax) tMinMax[sd.Sensor].DPMax = sensorDPNum[sd.Sensor];
                    if (sensorDPNum[sd.Sensor] < tMinMax[sd.Sensor].DPMin) tMinMax[sd.Sensor].DPMin = sensorDPNum[sd.Sensor];
                    if (sensorRHNum[sd.Sensor] > tMinMax[sd.Sensor].RHMax) tMinMax[sd.Sensor].RHMax = sensorRHNum[sd.Sensor];
                    if (sensorRHNum[sd.Sensor] < tMinMax[sd.Sensor].RHMin) tMinMax[sd.Sensor].RHMin = sensorRHNum[sd.Sensor];
                    if (sensorHInum[sd.Sensor] > tMinMax[sd.Sensor].HIMax) tMinMax[sd.Sensor].HIMax = sensorHInum[sd.Sensor];
                    if (sensorWCnum[sd.Sensor] < tMinMax[sd.Sensor].WCMin) tMinMax[sd.Sensor].WCMin = sensorWCnum[sd.Sensor];
                    continue;
                }

                if (sd.RecordType == StationRecordType.Wind)
                {
                    wind = sd.Record.AverageSpeed;
                    WindLabel.Text = "Wind: " + WxSpeedUnit.WindDspString(WxSpeedUnit.Convert(sd.Record.AverageSpeed,mWxDataUnits.Wind,mWxDisplayUnits.Wind)
                        ,mWxDisplayUnits.Wind) + 
                        WxSpeedUnit.ToString(mWxDisplayUnits.Wind);
                    continue;
                }

                if (sd.RecordType == StationRecordType.Rain)
                {
                    Rain24 = sd.Record.RainThisDay + sd.Record.RainThisHour;
                    
                    continue;
                }
              
            
            }
            //
            // how long has it been since we have received data from each sensor?
            // keep the longest time interval of them all. 
            //  

            //TimeSpan oldestSensor = (sinceInsideTemp > sinceOutsideTemp) ? sinceInsideTemp : sinceOutsideTemp;
            //oldestSensor = (oldestSensor > sinceAcTemp) ? oldestSensor : sinceAcTemp;
            UpdateStatus(); // run the main loop and update the form
        }

        private void DesHumidity_ValueChanged(object sender, EventArgs e)
        {
            if (DesHumidity.Value > Convert.ToDecimal(MaxDesHum()))
                DesHumudityLabel.BackColor = Color.Red;
            else
                DesHumudityLabel.BackColor = Color.LightGreen;
        }

        private void Temp_Hover(object sender, EventArgs e)
        {
            
            
        }

 

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSet_Click(null, null);
        }

        private void minMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinMaxForm mx = new MinMaxForm();
            string u = WxTemperatureUnit.ToString(mWxDisplayUnits.Temperature);
            mx.dataGridView1.Rows.Add(tMinMax.Length - 1);

            for (int count = 0; count < tMinMax.Length - 1; count++)
            {
                mx.dataGridView1.Rows[count].Cells[0].Value = count.ToString();
                if (tMinMax[count].tempMin < 999)
                    mx.dataGridView1.Rows[count].Cells[1].Value = WxTemperatureUnit.DspString(tMinMax[count].tempMin, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[1].Value = "-";

                if (tMinMax[count].tempMax > -99)
                    mx.dataGridView1.Rows[count].Cells[2].Value = WxTemperatureUnit.DspString(tMinMax[count].tempMax, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[2].Value = "-";

                if (tMinMax[count].ATMin < 999)
                    mx.dataGridView1.Rows[count].Cells[3].Value = WxTemperatureUnit.DspString(tMinMax[count].ATMin, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[3].Value = "-";

                if (tMinMax[count].ATMax > -99)
                    mx.dataGridView1.Rows[count].Cells[4].Value = WxTemperatureUnit.DspString(tMinMax[count].ATMax, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[4].Value = "-";

                if (tMinMax[count].DPMin < 99)
                    mx.dataGridView1.Rows[count].Cells[5].Value = WxTemperatureUnit.DspString(tMinMax[count].DPMin, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[5].Value = "-";

                if (tMinMax[count].DPMax > -99)
                    mx.dataGridView1.Rows[count].Cells[6].Value = WxTemperatureUnit.DspString(tMinMax[count].DPMax, mWxDataUnits.Temperature, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[6].Value = "-";

                if (tMinMax[count].RHMin < 100)
                    mx.dataGridView1.Rows[count].Cells[7].Value = tMinMax[count].RHMin.ToString("F1") + "%";
                else
                    mx.dataGridView1.Rows[count].Cells[7].Value = "-";

                if (tMinMax[count].RHMax > 0)
                    mx.dataGridView1.Rows[count].Cells[8].Value = tMinMax[count].RHMax.ToString("F1") + "%";
                else
                    mx.dataGridView1.Rows[count].Cells[8].Value = "-";

                if (tMinMax[count].WCMin < 999)
                    mx.dataGridView1.Rows[count].Cells[9].Value = WxTemperatureUnit.DspString(tMinMax[count].WCMin, TemperatureUnit.degF, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[9].Value = "-";

                if (tMinMax[count].HIMax > -99)
                    mx.dataGridView1.Rows[count].Cells[10].Value = WxTemperatureUnit.DspString(tMinMax[count].HIMax, TemperatureUnit.degF, mWxDisplayUnits.Temperature) + u;
                else
                    mx.dataGridView1.Rows[count].Cells[10].Value = "-";
            }

            mx.ShowDialog();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void resetMiinMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = 0; i<10;i++)
            {
                tMinMax[i].ATMax = -99;
                tMinMax[i].ATMin = 999;
                tMinMax[i].DPMax = -99;
                tMinMax[i].DPMin = 99;
                tMinMax[i].HIMax = -99;
                tMinMax[i].RHMax = 0;
                tMinMax[i].RHMin = 100;
                tMinMax[i].tempMax = -99;
                tMinMax[i].tempMin = 999;
                tMinMax[i].WCMin = 999;
            }
        }

        private void celciusToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (celciusToolStripMenuItem.Checked)
            {
                mWxDisplayUnits.Temperature = TemperatureUnit.degC;
                fehrenheitToolStripMenuItem.Checked = false;
            }
            else
            {
                mWxDisplayUnits.Temperature = TemperatureUnit.degF;
                fehrenheitToolStripMenuItem.Checked = true;
            }
        }

        private void fehrenheitToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (fehrenheitToolStripMenuItem.Checked)
            {
                mWxDisplayUnits.Temperature = TemperatureUnit.degF;
                celciusToolStripMenuItem.Checked = false;
            }
            else
            {
                mWxDisplayUnits.Temperature = TemperatureUnit.degC;
                celciusToolStripMenuItem.Checked = true;
            }
        }

 
        
    }
}
