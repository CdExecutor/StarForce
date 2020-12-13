using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarForce
{
    public static class PlatformUtility
    {
        public static string GetPlatformStr()
        {
            if (Application.platform == RuntimePlatform.Android)
                return "Android";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "iOS";
            else if (Application.platform == RuntimePlatform.WindowsEditor)
                return "Windows";
            else if (Application.platform == RuntimePlatform.OSXEditor)
                return "MacOS";
            return "";
        }
    }
}
