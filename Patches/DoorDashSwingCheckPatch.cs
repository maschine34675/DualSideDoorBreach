using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace DualSideDoorBreach.Patches
{
    internal sealed class DoorDashSwingCheckPatch : ModulePatch
    {
        private static MethodBase _targetMethod;
        private static bool _isApplied;

        public static void TryEnable()
        {
            if (_isApplied || !Plugin.DoorDashCompatibility.Value)
                return;

            if (!DoorDashCompat.IsInstalled)
            {
                Plugin.Log.LogInfo("DoorDash compatibility is enabled but DoorDash is not installed.");
                return;
            }

            var breacherType = DoorDashCompat.RaycastBreacherType;
            if (breacherType == null)
            {
                Plugin.Log.LogWarning("DoorDash is installed but RaycastBreacher type was not found.");
                return;
            }

            _targetMethod = AccessTools.Method(breacherType, "WillDoorSwingTowardsPlayer");
            if (_targetMethod == null)
            {
                Plugin.Log.LogWarning("DoorDash compatibility: WillDoorSwingTowardsPlayer was not found.");
                return;
            }

            new DoorDashSwingCheckPatch().Enable();
            _isApplied = true;
            Plugin.Log.LogInfo("DoorDash compatibility patch applied.");
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetMethod;
        }

        [PatchPrefix]
        private static bool Prefix(Door door, Vector3 playerPosition, ref bool __result)
        {
            if (!Plugin.Enabled.Value || !Plugin.DoorDashCompatibility.Value)
                return true;

            if (DoorBreachUtil.IsBreachOnlyInteractionDoor(door))
                return true;

            if (!DoorBreachUtil.EvaluateIsBreachAngle(door, playerPosition))
            {
                __result = true;
                return false;
            }

            __result = false;
            return false;
        }
    }
}
