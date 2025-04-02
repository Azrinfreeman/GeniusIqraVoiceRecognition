using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Information
{
    public class BatteryCallback : AndroidJavaProxy
    {
        public Action<float> onBatteryLifeChange;

        public BatteryCallback() : base("com.gigadrillgames.androidplugin.battery.IBattery")
        {
        }

        void BatteryLifeChange(float val)
        {
            onBatteryLifeChange(val);
        }
    }
}