using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.M_Plugins.Helpers.Utils
{
    public static class TypeCodeHelper
    {
        private static string CameraType = "3000201|14|3000610|1000102";

        /// <summary>
        /// 根据TypeCode判断是否是摄像机
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsCamera(string typeCode)
        {
            if (string.IsNullOrEmpty(typeCode)) return false;
            return IsTypeCodeContains(typeCode,CameraType);
        }

        private static string DoorAccess = "其他_单联单控开关_3D|";
        /// <summary>
        /// 根据TypeCode判断是否是门禁
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsDoorAccess(string typeCode)
        {
            if (string.IsNullOrEmpty(typeCode)) return false;
            return IsTypeCodeContains(typeCode,DoorAccess);
        }

        private static string LocationDev = "20180821|";
        /// <summary>
        /// 是否定位设备
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsLocationDev(string typeCode)
        {
            if (string.IsNullOrEmpty(typeCode)) return false;
            return IsTypeCodeContains(typeCode,LocationDev);
        }
        /// <summary>
        /// 静态设备Typecode
        /// </summary>
        private static string StaticDev="20181008|";
        /// <summary>
        /// 静态设备Typecode
        /// </summary>
        public static string StaticDevTypeCode = "20181008";
        /// <summary>
        /// 是否静态设备
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsStaticDev(string typeCode)
        {
            if (string.IsNullOrEmpty(typeCode)) return false;
            return IsTypeCodeContains(typeCode,StaticDevTypeCode);
        }

        /// <summary>
        /// 是否包含TypeCode
        /// </summary>
        /// <param name="typeCode"></param>
        /// <param name="allCode"></param>
        /// <returns></returns>
        private static bool IsTypeCodeContains(string typeCode,string allCode)
        {
            string[] part = allCode.Split('|');
            foreach(var item in part)
            {
                if(item==typeCode)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
