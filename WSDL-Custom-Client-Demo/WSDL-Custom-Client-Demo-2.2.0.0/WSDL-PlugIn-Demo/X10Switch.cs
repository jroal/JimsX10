using System;
using System.Collections.Generic;
using System.Text;

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
    /// <summary>
    /// A simple wrapper class to control X10 switches from .NET
    /// You must install the X10 drivers for this to work. In addition
    /// you may need to delete and add back the project reference to
    /// "ActiveHomeScriptLib" before this project will build successfully.
    /// To add the reference back, right-click on the project's references and
    /// select Add Reference... The library can be found under the "COM" tab
    /// of the add reference dialog and is called "ActiveHomeScript 1.0 Type Library".
    /// 
    /// Each instance of this class will control one X10 switch. If you need to
    /// control several switches, then create one instance for each switch.
    /// This class does not guard against creating more than one instance to control
    /// the same X10 switch. If you do this, all bets are off.
    /// 
    /// </summary>
    public class X10Switch
    {
        ActiveHomeScriptLib.ActiveHomeClass mX10; // COM object from the X10 folks.
        //
        // house and unit codes for the switch this class will be
        // controlling.
        //
        char mHouseCode = 'a';
        int mUnitCode = 1;
        //
        // Since X10 communications are one-way, there is no way to be 
        // certain that a switch on/off command really worked. To make this a 
        // bit more reliable, on and off commands will be repeated "mStutter" 
        // times to increase the liklihood that the switch is really in the 
        // desired state. However, it is possible that the X10 COM library 
        // detects repeated on commands and throws out the repeats, so this
        // could be a wasted effort.
        //
        // the design here is that mState starts out equal to "stutter" -- in 
        // this example "5" but it can be any other positive integer.
        // to use this class, the client should setup a "run loop" that 
        // periodically executes. Every pass through the run loop, "SetState()" 
        // should be called with the desired switch state -- even if that 
        // state has not changed recently. 
        //
        // Set state will send on or off commands to the switch and either
        // increment or decrement mState each time. Once mState reaches
        // 2*mStutter or zero (depending on the desired state, on or off).
        // At that point it will stop sending on or off commands to the switch.
        //
        // The first time a state is set, the on or off command will be sent
        // mStutter times. Thereafter when changing state, the command will
        // be sent 2*mStutter times.
        //
        const int mStutter = 5;
        int mState = mStutter;
        bool mDesiredState = false;
        //
        // strings to hold the on and off commands for this switch
        //
        string mOnArgs;
        string mOffArgs;

        public X10Switch(string Id)
        {            
            char HouseCode;
            int UnitCode;
            bool ok = X10IdToCodes(Id, out HouseCode, out UnitCode);
            
            if (!ok)
            {
                throw new ArgumentException("Invalid ID argument to X10Switch constructor: " + Id);
            }

            mX10 = new ActiveHomeScriptLib.ActiveHomeClass();
            mHouseCode = HouseCode;
            mUnitCode = UnitCode;
            mOnArgs = mHouseCode + mUnitCode.ToString("0") + " on";
            mOffArgs = mHouseCode + mUnitCode.ToString("0") + " off";
            //
            // if you want to detect RF events from keypads or motion sensors, etc,
            // then add this event handler:
            //
            // mX10.RecvAction += GotEvent; 
        }

        public X10Switch(char HouseCode, int UnitCode)
        {
            mX10 = new ActiveHomeScriptLib.ActiveHomeClass();
            mHouseCode = HouseCode;
            mUnitCode = UnitCode;
            mOnArgs = mHouseCode + mUnitCode.ToString("0") + " on";
            mOffArgs = mHouseCode + mUnitCode.ToString("0") + " off";
            //
            // if you want to detect RF events from keypads or motion sensors, etc,
            // then add this event handler:
            //
            // mX10.RecvAction += GotEvent; 
        }

        public static bool X10IdToCodes(string Id, out char HouseCode, out int UnitCode)
        {
            HouseCode = 'a';
            UnitCode = -1;

            string[] fields = Id.Split(new char[] { ',' });
            bool ok = (fields != null) && (fields.Length == 2) && (fields[0].Length == 1) && char.IsLetter(fields[0][0]);

            if (!ok) return false;
            
            HouseCode = char.ToLowerInvariant(fields[0][0]);

            ok = (HouseCode >= 'a') && (HouseCode <= 'p');
            if (!ok) return false;

            ok = int.TryParse(fields[1], out UnitCode);
            ok = ok && (UnitCode >= 1 && UnitCode <= 16);

            return ok;
        }

        public static bool X10CodesToId(char HouseCode, int UnitCode, out string Id)
        {
            Id = null;

            char c = char.ToLowerInvariant(HouseCode);
            bool ok = (HouseCode >= 'a') && (HouseCode <= 'p');
            if (!ok) return false;

            ok = (UnitCode >= 1) && (UnitCode <= 16);
            if (!ok) return false;

            Id = string.Format("{0},{1}", HouseCode, UnitCode.ToString(System.Globalization.CultureInfo.InvariantCulture));

            return true;
        }

        public bool DesiredState { get { return mDesiredState; } }

        public void SetState(bool State)
        {
            mDesiredState = State;  // record the requested state.
            //
            // repeat the on or off commands several times. For this to work, 
            // the caller must call this function on every pass through it's 
            // run loop. Otherwise, this function will not get the chance to
            // repeat the on or off commands.
            //
            if (State && (mState >= (2 * mStutter))) return;
            if (!State && (mState <= 0)) return;
            mX10.SendAction("sendplc", State ? mOnArgs : mOffArgs, null, null);
            mState += State ? 1 : -1;
        }

        private const string mMotionChannel = "a1";
        private const string mRfMessage = "recvrf";
        private const string mOnMessage = "on";
        private const string mOffMessage = "off";

        private void GotEvent(object action, object p1, object p2, object p3, object p4, object p5, object p6)            
        {
            // the try/catch construct will save us if any of the object types are not
            // what is expected. easier than examining each object type for validity.
            try
            {
                //
                // the RF message that occurs when the motion detector goes off will look like this:
                // "recvrf", "a1", "On", 0
                // the 2nd value is the house/unit code programmed into the motion sensor, which defaults to "a1"
                // the 3rd value is either "On" or "Off" but we don't care about the off events
                // the 4th value is an integer, 0 for key down and 1 for key up.
                // the motion sensor sends the command three times in rapid succession so you may want
                // to filter the extra ones out...
                //
                // events will also occur when plc commands are sent by "SetState()" above but the action
                // string will be "recvplc" in this case. These events are ignored.
                //
                if (string.Compare(mRfMessage, (string)action, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (string.Compare(mMotionChannel, (string)p1, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        bool keyDown = ((int)p3 == 0);
                        bool onMsg = string.Compare(mOnMessage, (string)p2, StringComparison.InvariantCultureIgnoreCase) == 0;
                        bool offMsg = string.Compare(mOffMessage, (string)p2, StringComparison.InvariantCultureIgnoreCase) == 0;
                        
                        if (keyDown)
                        {                            
                            if (onMsg) Console.WriteLine("YIKES!!! Something just went bump in the dark...");
                            if (offMsg) Console.WriteLine("...never mind, I guess it was nothing");
                        }
                    }
                }
            }
            catch { }
        }



    }

}
