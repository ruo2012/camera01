using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CommandLib
{
    public class FanCtrl
    {
        public static void OnFanDecelerateFunc(object obj)
        {
            MessageBox.Show("OnFanDecelerateFunc");
        }

        public static void OnFanTurboFunc(object obj)
        {
            MessageBox.Show("OnFanTurboFunc");
        }

        public static void OnFanFunc(object obj)
        {
            MessageBox.Show("OnFanFunc");
        }
    }
}
