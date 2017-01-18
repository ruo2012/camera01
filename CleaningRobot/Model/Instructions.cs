using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningRobot.Model
{
    public class Instructions
    {
        /// <summary>
        /// 风机启动
        /// </summary>
        public string fjqd = "c001000f0d0a";
        /// <summary>
        /// 风机关闭
        /// </summary>
        public string fjgb = "c002000f0d0a";
        /// <summary>
        /// 风机加速
        /// </summary>
        public string fjjs = "c003000f0d0a";
        /// <summary>
        /// 风机减速
        /// </summary>
        public string fjslow = "c004000f0d0a";
        /// <summary>
        /// 运动启动
        /// </summary>
        public string ydqj = "c441000f0d0a";
        /// <summary>
        /// 运动关闭
        /// </summary>
        public string ydht = "c111000f0d0a";
        /// <summary>
        /// 运动左转
        /// </summary>
        public string ydzz = "c331000f0d0a";
        /// <summary>
        /// 运动右转
        /// </summary>
        public string ydyz = "c221000f0d0a";
        /// <summary>
        /// 运动加速
        /// </summary>
        public string ydjs = "c901000f0d0a";
        /// <summary>
        /// 运动减速
        /// </summary>
        public string ydslow = "c902000f0d0a";
        /// <summary>
        /// 雨刮启动
        /// </summary>
        public string ygqd = "c601000f0d0a";
        /// <summary>
        /// 雨刮关闭
        /// </summary>
        public string yggb = "c602000f0d0a";
        /// <summary>
        /// 雨刮加速
        /// </summary>
        public string ygjs = "c601010f0d0a";
        /// <summary>
        /// 雨刮减速
        /// </summary>
        public string ysslow = "c001020f0d0a";
        /// <summary>
        /// 喷水开启
        /// </summary>
        public string psqd = "c701000f0d0a";
        /// <summary>
        /// 喷水关闭
        /// </summary>
        public string psgb = "c702000f0d0a";
        /// <summary>
        /// 喷水加速
        /// </summary>
        public string psjs = "c701010f0d0a";
        /// <summary>
        /// 喷水减速
        /// </summary>
        public string psslow = "c701020f0d0a";
        /// <summary>
        /// 摄像头左转
        /// </summary>
        public string sxtzz = "c801010f0d0a";
        /// <summary>
        /// 摄像头右转
        /// </summary>
        public string sztyz = "c801020f0d0a";
    }
}
