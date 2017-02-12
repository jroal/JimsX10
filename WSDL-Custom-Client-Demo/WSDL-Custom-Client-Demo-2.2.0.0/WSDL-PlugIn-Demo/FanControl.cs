using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WsdlPlugInDemo
{
    /// <summary>
    /// This makes it a little easier to manage the interaction between the
    /// ComboBox's used to set X10 switch addresses and the X10 swtiches.
    /// 
    /// We keep a record of ComboBox initial selected index in the ComboBox.Tag member.
    /// This allows us to compare later to see if the current selected index is
    /// different than the original value -- in otherwords, has the value changed
    /// from its initial state?
    /// 
    /// For this to work properly, the client MUST use the parameterized constructor.
    /// </summary>
    internal struct FanControl
    {
        private ComboBox HouseCode;
        private ComboBox UnitCode;
        private X10Switch mSwitch;

        internal FanControl(ComboBox HouseCode, ComboBox UnitCode)
        {
            this.HouseCode = HouseCode;
            this.UnitCode = UnitCode;
            mSwitch = null;
        }

        /// <summary>
        /// Access to the private X10 switch object created and maintained by this struct
        /// </summary>
        internal X10Switch Switch { get { return mSwitch; } }

        /// <summary>
        /// Use this to create the initial X10 switch or re-connect to a different 
        /// X10 address.
        /// 
        /// If an X10 switch exists it will be turned off prior to creating the new switch.
        /// </summary>
        /// <param name="Id"></param>
        internal void Connect(string Id, bool ForceReconnect)
        {
            char house;
            int unit;

            if (!ForceReconnect && (mSwitch != null) && !ConfigChanged) return;

            bool ok = X10Switch.X10IdToCodes(Id, out house, out unit);

            if (!ok) throw new ArgumentException("Invalid ID to FanControl.Connect");

            if (mSwitch != null)
            {
                mSwitch.SetState(false);
                mSwitch = null;
            }

            mSwitch = new X10Switch(Id);
            mSwitch.SetState(false);

            if (HouseCode != null)
            {
                HouseCode.SelectedIndex = (int)(house - 'a');
                HouseCode.BackColor = HouseCode.Parent.BackColor;
                HouseCode.Tag = (object)HouseCode.SelectedIndex;
            }

            if (UnitCode != null)
            {
                UnitCode.SelectedIndex = unit - 1;
                UnitCode.BackColor = UnitCode.Parent.BackColor;
                UnitCode.Tag = (object)UnitCode.SelectedIndex;
            }
        }

        /// <summary>
        /// Has the address specified by the ComboBox's changed from the initial values?
        /// </summary>
        internal bool ConfigChanged
        {
            get
            {
                return (mSwitch == null) || (HouseCode.SelectedIndex != (int)HouseCode.Tag) || (UnitCode.SelectedIndex != (int)UnitCode.Tag);
            }
        }
    }

}
