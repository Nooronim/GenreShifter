using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.OS;

namespace GenreShifterProt4
{
    public static class PowerStatus
    {
        public static BroadcastReceiver _batteryStatusReciever;
        private static int _batteryLevel;
        private static int _batteryLevelScale = 100;

        static PowerStatus()
        {
            _batteryStatusReciever = new PowerStatusBroadcastReciever();
        }

        private class PowerStatusBroadcastReciever : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                _batteryLevel = intent.GetIntExtra("level", 0);
                _batteryLevelScale = intent.GetIntExtra("scale", 100);
            }
        }

        public static float? BatteryLifePrecent { get { return _batteryLevel * 100 / _batteryLevelScale; } }
    }
}