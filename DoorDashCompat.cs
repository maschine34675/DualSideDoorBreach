using BepInEx.Bootstrap;
using System;

namespace DualSideDoorBreach
{
    internal static class DoorDashCompat
    {
        public const string PluginGuid = "com.tarkin.doordash";

        public static bool IsInstalled => Chainloader.PluginInfos.ContainsKey(PluginGuid);

        public static Type RaycastBreacherType
        {
            get
            {
                if (!Chainloader.PluginInfos.TryGetValue(PluginGuid, out var pluginInfo))
                    return null;

                return pluginInfo.Instance?.GetType().Assembly
                    .GetType("tarkin.doordash.RaycastBreacher");
            }
        }
    }
}
