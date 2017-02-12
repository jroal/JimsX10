using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using WsdlClientInterface;

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

namespace WsdlPlugInDemo
{


    public partial class MainForm : Form
    {
        #region Member Variables

        WsdlClient mClient;     // receives data from the server
        Queue mDataQ;           // weather data broadcasts are stored in this queue
        Queue mLogQ;           // used to hold weather log updates and messages (we don't use this data)
        Queue mLogFileQ;
        Queue mMsgQ;

        //
        // this setup assumes two fans and one small room air conditioner are connected to X10 switches. 
        // The window fan is placed in a window and blows hot air out of the window, 
        // sucking cool air in through other "inlet" windows in the house.
        // The mixing fan is located near an open inlet window and helps to mix the incoming cool
        // air with warmer air currentInsideTemp the house.
        //
        // One X10Switch class is instantiated to control each item to be controlled.
        //
        FanControl WindowFan, MixingFan, Ac, AcMixFan;

        //X10Switch WindowFanSwitch;
        //X10Switch MixingFanSwitch;
        //X10Switch AcSwitch;
        //X10Switch AcMixSwitch;
        //
        // this timer will call our processing function every 10 seconds.
        //
        System.Windows.Forms.Timer mTimer;
        //
        // information about temperatures used to make decisions about fan on/off states.
        //        
        DateTime lastInside;        // last time an currentInsideTemp temperature reading was received
        DateTime lastOutside;       // same for currentOutsideTemp temperature
        DateTime lastAc;            // and for the A/C temperature

        double currentInsideTemp = 0.0;        // current indoor temperature
        double currentOutsideTemp = 100.0;     // current outdoor temperature
        double currentAcTemp = 0.0;            // current A/C regulated temperature

        double tgtIndoorTemp = 80.0;      // desired indoor temperature
        double tgtAcTemp =   80.0;      // targeted AcSwitch temperature
        
        double minDeltaTemp = 2.0;  // minimum (indoor-outdoor) difference for cooling to work
        double hysterisis = 0.5;    // used to prevent rapid on-off cycling of fans

        bool fanDisablesAc;

        bool coolingState = false;      // is it cool enough outside for the fan?
        bool fanState = false;          // should the fan be on (not considering cooling availability)
        bool acState = false;           // is the AC on?
        bool acSwitchState = false;
        bool fanSwitchState = false;


        //
        // label colors used to indicate status
        //
        Color okColor = Color.LimeGreen;
        Color hotColor = Color.Firebrick;
        Color warmColor = Color.Salmon;
        Color oldColor = Color.Yellow;
        Color offColor = Color.DarkGreen;
        
        //
        // WSDL channel numbers for indoor/outdoor temperatures.
        // set these to whatever channels are being used in your system.
        //
        int indoorChannel;
        int outdoorChannel;
        int AcChannel;

        ComboBox[] mX10Combos;

        bool mGuiUpdating = false;

        #endregion


        public MainForm()
        {
            InitializeComponent();
            //
            //
            //
            mX10Combos = new ComboBox[] {
                cbCoolingFanHouseCode, cbCoolingFanUnitCode, cbMixingFanHouseCode, cbMixingFanUnitCode,
                cbAcHouseCode, cbAcUnitCode, cbAcMixingHouseCode, cbAcMixingUnitCode
            };

            foreach (ComboBox cb in mX10Combos) cb.SelectedIndexChanged += X10ComboBox_SelectedIndexChanged;

            mGuiUpdating = true;
            //
            // create fan control structures
            //
            WindowFan = new FanControl(cbCoolingFanHouseCode, cbCoolingFanUnitCode);
            MixingFan = new FanControl(cbMixingFanHouseCode, cbMixingFanUnitCode);
            Ac = new FanControl(cbAcHouseCode, cbAcUnitCode);
            AcMixFan = new FanControl(cbAcMixingHouseCode, cbAcMixingUnitCode);
            //
            // initialize settings from saved properties
            //
            AcChannel = Properties.Settings.Default.AcChannel;
            outdoorChannel = Properties.Settings.Default.OutsideChannel;
            indoorChannel = Properties.Settings.Default.InsideChannel;
            hysterisis = Properties.Settings.Default.Hysterisis;
            tgtIndoorTemp = Properties.Settings.Default.InsideSetpoint;
            minDeltaTemp = Properties.Settings.Default.CoolingThreshold;
            fanDisablesAc = Properties.Settings.Default.FanDisablesAc;
            tgtAcTemp = Properties.Settings.Default.AcSetpoint;
            //
            // set last data acquistion time to long ago 
            //
            lastInside = lastOutside = lastAc = new DateTime(0L); 
            //
            // copy initial values to GUI
            //
            nudAcChannel.Value = (decimal)AcChannel;
            nudOutsideChannel.Value = (decimal)outdoorChannel;
            nudInsideChannel.Value = (decimal)indoorChannel;
            nudHysterisis.Value = (decimal)hysterisis;
            nudSetPoint.Value = (decimal)tgtIndoorTemp;
            nudAcSetPoint.Value = (decimal)tgtAcTemp;
            nudThreshold.Value = (decimal)minDeltaTemp;
            cbFanDisalbesAc.Checked = fanDisablesAc;
            //
            // create X10 switches and initialize the combo boxes for displaying and changing 
            // X10 switch addresses
            //
            UpdateX10Config();

            mGuiUpdating = false;
            //
            // create the WSDL client to capture UDP broadcast packets. data received will
            // be stored in one of the queues for later examination by the timer tick function.
            // If you have changed the WSDL Server's UDP port then change the first argument to 
            // the WsdlClient constructor below to match.
            //
            mDataQ = new Queue();
            mLogQ = new Queue();
            mLogFileQ = new Queue();
            mMsgQ = new Queue();

            mClient = new WsdlClient(ClientServerComms.DefaultUdpPort, 
                ref mDataQ, ref mLogQ, ref mLogFileQ, ref mMsgQ);
            //
            // start up a timer to call the Process() function every 10 seconds
            //
            mTimer = new System.Windows.Forms.Timer();
            mTimer.Tick += new EventHandler(Process);
            mTimer.Interval = 10000;  // 10 seconds in units of milliseconds
            mTimer.Start();
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
            // weather log updates need to be further de-serialized
            //
            while (mLogQ.Count > 0)
            {
                object o = mLogQ.Dequeue();
                string s = (string)o;
                WxLogRecord logRec;
                bool okay = WsdlXml.DeserializeLogRecord(s, out logRec);
                if (okay)
                {
                    // do something with the logRec
                }
            }
            //
            // raw log file records are also received
            //
            while (mLogFileQ.Count > 0)
            {
                object o = mLogFileQ.Dequeue();
                string s = (string)o;
                // do something with "s" now if desired...
            }
            //
            // message log handling
            //
            while (mMsgQ.Count > 0)
            {
                object o = mMsgQ.Dequeue();
                Type oType = o.GetType();

                if (oType == typeof(string))
                {
                    // this is a one-line message
                    string oneLineMsg = (string)o;
                }
                else if (oType == typeof(string[]))
                {
                    // this is a multiple-line message
                    string[] multiLineMsg = (string[])o;
                }
                else
                {
                    // something is wrong. there should only be strings
                    // and string arrays in mMsgQ.
                }
            }
            //
            // entries in the data queue are what we are really interested in
            //
            while (mDataQ.Count > 0)
            {
                StationData sd = (StationData)mDataQ.Dequeue(); // get the next record in queue
                //
                // we only care about temperature/humidity data from the indoor and outdoor channels
                //
                if (sd.RecordType != StationRecordType.TemperatureHumidity) continue;

                if (sd.Sensor == indoorChannel)
                {
                    currentInsideTemp = sd.Record.Temperature * 1.8 + 32.0;
                    lastInside = DateTime.UtcNow;
                    lblInsideTemp.Text = currentInsideTemp.ToString("0.0");
                    continue;
                }

                if (sd.Sensor == outdoorChannel)
                {
                    currentOutsideTemp = sd.Record.Temperature * 1.8 + 32.0;
                    lastOutside = DateTime.UtcNow;
                    lblOutsideTemp.Text = currentOutsideTemp.ToString("0.0");
                    continue;
                }

                if (sd.Sensor == AcChannel)
                {
                    currentAcTemp = sd.Record.Temperature * 1.8 + 32.0;
                    lastAc = DateTime.UtcNow;
                    lblAcTemp.Text = currentAcTemp.ToString("0.0");
                    continue;
                }

            }
            //
            // how long has it been since we have received data from each sensor?
            // keep the longest time interval of them all. 
            //
            DateTime now = DateTime.UtcNow;
            TimeSpan sinceInsideTemp = now - lastInside;
            TimeSpan sinceOutsideTemp = now - lastOutside;
            TimeSpan sinceAcTemp = now - lastAc;

            TimeSpan oldestSensor = (sinceInsideTemp > sinceOutsideTemp) ? sinceInsideTemp : sinceOutsideTemp;
            oldestSensor = (oldestSensor > sinceAcTemp) ? oldestSensor : sinceAcTemp;

            if (oldestSensor > sensorTimeoutLimit)
            {
                //
                // If we have lost contact with any sensor,
                // turn everything off as a safety measure.
                //
                fanState = false;
                acState = false;
                acSwitchState = false;
                fanSwitchState = false;

                lblTempOk.BackColor = oldColor; // this color means the sensors have timed out
                lblCooler.BackColor = oldColor;
                lblAcTempOk.BackColor = oldColor;
            }
            else
            {
                //
                // create three booleans that include hysterisis in their behavior:
                // 1. cooling available based on difference between inside/outside temps
                // 2. desired fan state based on inside temp vs inside setpoint
                // 3. desired ac state based on ac temp vs ac setpoint
                //
                // the actual fan switch state is the logical AND of the cooling available state and the fan state
                //
                bool coolingAboveThreshold = (currentInsideTemp - currentOutsideTemp) >= minDeltaTemp;
                bool coolingAboveHysterisis = (currentInsideTemp - currentOutsideTemp) >= (minDeltaTemp + hysterisis);
                coolingState = coolingState ? coolingAboveThreshold : coolingAboveHysterisis;

                // is the currentInsideTemp temperature below the targeted value?
                bool fanUseful = currentInsideTemp > tgtIndoorTemp;

                // does the currentInsideTemp temperature exceeds the desired value by more than
                // the hysterisis amount?
                bool fanRequired = currentInsideTemp > (tgtIndoorTemp + hysterisis);

                fanState = fanState ? fanUseful : fanRequired;
                fanSwitchState = fanState && coolingState;

                bool acUseful = currentAcTemp > tgtAcTemp;
                bool acRequired = currentAcTemp > (tgtAcTemp + hysterisis);

                acState = acState ? acUseful : acRequired;
                acSwitchState = fanDisablesAc ? (!fanSwitchState & acState) : acState;

                //
                // update label colors to indicate current conditions
                //
                lblTempOk.BackColor = fanUseful ? (fanRequired ? hotColor : warmColor) : okColor;
                lblCooler.BackColor = coolingAboveThreshold ? (coolingAboveHysterisis ? okColor : warmColor) : hotColor;
                lblAcTempOk.BackColor = acUseful ? (acRequired ? hotColor : warmColor) : okColor;

                
            }

            //
            // Possible enhancement left for the reader:
            // It can take a long time to cool a house after sunset because there is
            // a lot of heat energy stored in the house walls and objects such as furniture.
            // To cope with this more efficiently, try an experiment to
            // delay turning off the mixing fan by some length of time (say 5, 10 or 15 minutes).
            // This will help in removing heat from walls and furniture. This could
            // result in getting the house stabilized at the desired temperature a little quicker.
            //

            //
            // set the fans and A/C to desired state and update the GUI label to match.
            //
            WindowFan.Switch.SetState(fanSwitchState);
            MixingFan.Switch.SetState(fanSwitchState);
            lblFanState.BackColor = fanSwitchState ? okColor : offColor;

            Ac.Switch.SetState(acSwitchState);
            AcMixFan.Switch.SetState(acSwitchState);
            lblAcState.BackColor = acSwitchState ? okColor : offColor;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            mClient.Disconnect();
            this.Close();
        }

        private void nudSetPoint_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            tgtIndoorTemp = (double)nudSetPoint.Value;
        }

        private void nudHysterisis_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            hysterisis = (double)nudHysterisis.Value;
        }

        private void nudThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            minDeltaTemp = (double)nudThreshold.Value;
        }

        private void nudAcSetPoint_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            tgtAcTemp = (double)nudAcSetPoint.Value;
        }

        private void nudOutsideChannel_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            outdoorChannel = (int)nudOutsideChannel.Value;
            lastOutside = new DateTime(0L);
        }

        private void nudInsideChannel_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            indoorChannel = (int)nudInsideChannel.Value;
            lastInside = new DateTime(0L);
        }

        private void nudAcChannel_ValueChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            AcChannel = (int)nudAcChannel.Value;
            lastAc = new DateTime(0L);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AcChannel = AcChannel;
            Properties.Settings.Default.AcSetpoint = tgtAcTemp;
            Properties.Settings.Default.CoolingThreshold = minDeltaTemp;
            Properties.Settings.Default.Hysterisis = hysterisis;
            Properties.Settings.Default.InsideChannel = indoorChannel;
            Properties.Settings.Default.InsideSetpoint = tgtIndoorTemp;
            Properties.Settings.Default.OutsideChannel = outdoorChannel;
            Properties.Settings.Default.FanDisablesAc = fanDisablesAc;

            string code;
            
            X10Switch.X10CodesToId(
                (char)(cbCoolingFanHouseCode.SelectedIndex + 'a'),
                cbCoolingFanUnitCode.SelectedIndex + 1, out code);
            Properties.Settings.Default.X10WindowFan = code;

            X10Switch.X10CodesToId(
                (char)(cbMixingFanHouseCode.SelectedIndex+'a'),
                cbMixingFanUnitCode.SelectedIndex + 1, out code);
            Properties.Settings.Default.X10MixingFan = code;

            X10Switch.X10CodesToId(
                (char)(cbAcMixingHouseCode.SelectedIndex + 'a'),
                cbAcMixingUnitCode.SelectedIndex + 1, out code);
            Properties.Settings.Default.X10AcMixingFan = code;

            X10Switch.X10CodesToId(
                (char)(cbAcHouseCode.SelectedIndex + 'a'),
                cbAcUnitCode.SelectedIndex + 1, out code);
            Properties.Settings.Default.X10Ac = code;

            Properties.Settings.Default.Save();
            
            MessageBox.Show("Settings Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateX10Config();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mClient.Disconnect();
        }

        private void cbFanDisalbesAc_CheckedChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;
            fanDisablesAc = cbFanDisalbesAc.Checked;
        }

        private void UpdateX10Config()
        {
            lbSaveNeeded.Visible = false;

            if (!X10ConfigChanged) return;
            //
            // create all X10 switches using addresses stored in settings
            //
            WindowFan.Connect(Properties.Settings.Default.X10WindowFan, false);
            MixingFan.Connect(Properties.Settings.Default.X10MixingFan, false);
            Ac.Connect(Properties.Settings.Default.X10Ac, false);
            AcMixFan.Connect(Properties.Settings.Default.X10AcMixingFan, false);
            //
            // set sensor updated times to long ago to force data re-acquisition
            //
            lastAc = new DateTime(0L);
            lastInside = new DateTime(0L);
            lastOutside = new DateTime(0L);

        }

        private bool X10ConfigChanged
        {
            get
            {
                return WindowFan.ConfigChanged ||
                       MixingFan.ConfigChanged ||
                       Ac.ConfigChanged ||
                       AcMixFan.ConfigChanged;
            }
        }

        private void X10ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mGuiUpdating) return;

            ComboBox cb = (ComboBox)sender;
            int originalIndex = (int)cb.Tag;
    
            if (cb.SelectedIndex != originalIndex)
            {
                cb.BackColor = Color.DarkGoldenrod;
                cb.Parent.Select();
    }
            else
            {
                cb.BackColor = cb.Parent.BackColor;
            }

            lbSaveNeeded.Visible = X10ConfigChanged;
            //
            // if we leave the combobox selected, the text will be an ugly blue color,
            // so force the selection somwehwere else (the combobox's parent will do).
            //
            cb.Parent.Select();
        }
 
    }

 
}