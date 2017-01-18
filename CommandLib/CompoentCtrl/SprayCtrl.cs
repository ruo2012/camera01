using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CommandLib
{
    public class SprayCtrl
    {
        public static void OnSprayDecelerateFunc(object obj)
        {
            MessageBox.Show("OnSprayDecelerateFunc");
        }

        public static void OnSprayTurbeFunc(object obj)
        {
            MessageBox.Show("OnSprayTurbeFunc");
        }

        public static void OnSprayFunc(object obj)
        {
            MessageBox.Show("OnSprayFunc");
        }
    }
}
