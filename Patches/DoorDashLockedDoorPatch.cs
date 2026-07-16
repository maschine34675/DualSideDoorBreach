using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace DualSideDoorBreach.Patches
{
    internal sealed class DoorDashLockedDoorPatch : ModulePatch
    {
        private static MethodBase _targetMethod;
        private static FieldInfo _playerField;
        private static bool _isApplied;

        public static void TryEnable()
        {
            if (_isApplied || !Plugin.DoorDashCompatibility.Value || !Plugin.RequireKeyForLockedBreach.Value)
                return;

            if (!DoorDashCompat.IsInstalled)
                return;

            var breacherType = DoorDashCompat.RaycastBreacherType;
            if (breacherType == null)
                return;

            _targetMethod = AccessTools.Method(breacherType, "GetBreachableDoorInFrontOfPlayer",
                new[] { typeof(Vector3) });
            _playerField = AccessTools.Field(breacherType, "player");

            if (_targetMethod == null || _playerField == null)
            {
                Plugin.Log.LogWarning("DoorDash compatibility: GetBreachableDoorInFrontOfPlayer was not found.");
                return;
            }

            new DoorDashLockedDoorPatch().Enable();
            _isApplied = true;
            Plugin.Log.LogInfo("DoorDash locked-door key compatibility patch applied.");
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetMethod;
        }

        [PatchPostfix]
        private static void Postfix(object __instance, ref Door __result)
        {
            if (!Plugin.Enabled.Value || !Plugin.RequireKeyForLockedBreach.Value || __result == null)
                return;

            if (__result.DoorState != EDoorState.Locked)
                return;
            if (string.IsNullOrEmpty(__result.KeyId))
                return;
            var player = (Player)_playerField.GetValue(__instance);
            if (!DoorKeyUtil.HasMatchingKeyAndSkill(__result, player))
            {
                __result = null;
                return;
            }

            if (!Plugin.AllowBreachWithoutKey.Value)
                PendingKeyConsume.Set(__result, player);
        }
    }
}
