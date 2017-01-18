using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using WIFIRobotCMDEngineV2;

namespace CommandLib
{
    public class DirectionCtrl
    {
        string CMD_Forward = "",
            CMD_Backward = "",
            CMD_TurnLeft = "",
            CMD_TurnRight = "",
            CMD_Stop = "";

        public void Init()
        {
            CMD_Forward = Utilities.ReadIni("Forward", "forward", "");
            CMD_Backward = Utilities.ReadIni("Backward", "backward", "");
            CMD_TurnLeft = Utilities.ReadIni("Left", "left", "");
            CMD_TurnRight = Utilities.ReadIni("Right", "right", "");
            CMD_Stop = Utilities.ReadIni("Stop", "stop", "");
        }

        /// <summary>
        /// forward 
        /// 往前走
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="RobotEngine2"></param>
        /// <param name="Send_status"></param>
        /// <param name="ctrlType"></param>
        /// <param name="cmd"></param>
        /// <param name="comm"></param>
        public void OnUp(object obj, WifiRobotCMDEngineV2 RobotEngine2, ref bool Send_status, int ctrlType, SerialPort comm)
        {
            MessageBox.Show("Up");
            if (Send_status)
            {
                RobotEngine2.SendCMD(ctrlType, CMD_Forward, comm);
                Send_status = false;
            }
        }

        /// <summary>
        /// Backward 
        /// 往后走
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="RobotEngine2"></param>
        /// <param name="Send_status"></param>
        /// <param name="ctrlType"></param>
        /// <param name="cmd"></param>
        /// <param name="comm"></param>
        public void OnDown(object obj, WifiRobotCMDEngineV2 RobotEngine2, ref bool Send_status, int ctrlType, SerialPort comm)
        {
            MessageBox.Show("Down");

            if (Send_status)
            {
                RobotEngine2.SendCMD(ctrlType, CMD_Backward, comm);
                Send_status = false;
            }
        }

        /// <summary>
        /// 往左走
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="RobotEngine2"></param>
        /// <param name="Send_status"></param>
        /// <param name="ctrlType"></param>
        /// <param name="cmd"></param>
        /// <param name="comm"></param>
        public void OnLeft(object obj, WifiRobotCMDEngineV2 RobotEngine2, ref bool Send_status, int ctrlType, SerialPort comm)
        {
            MessageBox.Show("Left");

            if (Send_status)
            {
                RobotEngine2.SendCMD(ctrlType, CMD_TurnLeft, comm);
                Send_status = false;
            }
        }

        public void OnStop(object obj, WifiRobotCMDEngineV2 RobotEngine2, ref bool Send_status, int ctrlType, SerialPort comm)
        {
            Send_status = true;
            RobotEngine2.SendCMD(ctrlType, CMD_Stop, comm);
        }

        /// <summary>
        /// 往右走
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="RobotEngine2"></param>
        /// <param name="Send_status"></param>
        /// <param name="ctrlType"></param>
        /// <param name="cmd"></param>
        /// <param name="comm"></param>
        public void OnRight(object obj, WifiRobotCMDEngineV2 RobotEngine2, ref bool Send_status, int ctrlType, SerialPort comm)
        {
            MessageBox.Show("Right");

            if (Send_status)
            {
                RobotEngine2.SendCMD(ctrlType, CMD_TurnRight, comm);
                Send_status = false;
            }
        }

        public void OnDecelerate(object obj)
        {
            MessageBox.Show("Decelerate");
        }

        public void OnTurbe(object obj)
        {
            MessageBox.Show("OnTurbe");
        }


    }
}
