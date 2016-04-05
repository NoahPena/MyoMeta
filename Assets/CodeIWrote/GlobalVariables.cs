using UnityEngine;
using System.Collections;

namespace ISuckAtCoding
{
    public static class GlobalVariables
    {
        public static float roll = 0;
        public static float pitch = 0;
        public static float gaz = 0;
        public static float yaw = 0;

        public static bool inAir = false;
        public static bool currentlyFlying = false;
        public static bool isVertical = false;

        public static string navigationData = "";

        public static bool useKeyboard = false;
        public static bool useWASD = true;

        public static float altitude = 0.0f;
        public static float prevAltitude = 0.0f;

        public static bool debug = false;
    }
}