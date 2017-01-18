using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CommandLib
{
    public class WiperCtrl
    {
        public static void OnWiperDecelerateFunc(object obj)
        {
            MessageBox.Show("OnWiperDecelerateFunc");
        }

        public static void OnWiperTurbeFunc(object obj)
        {
            MessageBox.Show("OnWiperTurbeFunc"); ;
        }

        public static void OnWiperFunc(object obj)
        {
            MessageBox.Show("OnWiperFunc");
        }
    }
}
