using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace DualSideDoorBreach.Patches
{
    internal sealed class DoorIsBreachAnglePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Door).GetMethod(nameof(Door.IsBreachAngle),
                BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool Prefix(Door __instance, Vector3 yourPosition, ref bool __result)
        {
            if (!Plugin.Enabled.Value)
                return true;

            __result = DoorBreachUtil.EvaluateIsBreachAngle(__instance, yourPosition);
            return false;
        }
    }

    internal sealed class DoorBreachSuccessRollPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Door).GetMethod(nameof(Door.BreachSuccessRoll),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(Vector3) },
                null);
        }

        [PatchPrefix]
        private static bool Prefix(Door __instance, Vector3 yourPosition, ref bool __result)
        {
            if (!Plugin.Enabled.Value)
                return true;

            if (!DoorBreachUtil.IsBreachAllowed(__instance))
            {
                __result = false;
                return false;
            }

            if (DoorBreachUtil.IsBreachOnlyInteractionDoor(__instance))
            {
                __result = DoorBreachUtil.VanillaBreachSuccessRoll(__instance, yourPosition);
                return false;
            }

            var breachPlayer = DoorBreachUtil.ResolveLocalPlayer(__instance.InteractingPlayer);
            __result = DoorKeyUtil.PlayerCanBreachLockedDoor(__instance, breachPlayer);
            return false;
        }
    }

    internal sealed class DoorKickOpenVectorPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Door).GetMethod(nameof(Door.KickOpen),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(Vector3), typeof(bool) },
                null);
        }

        [HarmonyBefore(DoorDashCompat.PluginGuid)]
        [PatchPrefix]
        private static void Prefix(Door __instance, Vector3 yourPosition)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.AdjustOpenDirection.Value)
                BreachKickContext.Begin(__instance, yourPosition);

            if (DoorKeyUtil.IsLockedBreachTarget(__instance))
            {
                var player = DoorBreachUtil.ResolveLocalPlayer(__instance.InteractingPlayer);
                DoorKeyUtil.TryConsumeKeyForBreach(__instance, player);
            }
        }

        [PatchPostfix]
        private static void Postfix()
        {
            BreachKickContext.End();
        }

        [PatchFinalizer]
        private static Exception Finalizer(Exception __exception)
        {
            BreachKickContext.End();
            return __exception;
        }
    }

    internal sealed class DoorKickOpenBoolPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Door).GetMethod(nameof(Door.KickOpen),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(bool) },
                null);
        }

        [PatchPrefix]
        private static bool Prefix(Door __instance, bool confirmed)
        {
            if (!Plugin.Enabled.Value || !Plugin.AdjustOpenDirection.Value)
                return true;

            var player = __instance.InteractingPlayer;
            var position = player != null ? player.Position : __instance.transform.position;
            __instance.KickOpen(position, confirmed);
            return false;
        }
    }

    internal sealed class DoorInitSmoothOpenPatch : ModulePatch
    {
        private static readonly MethodInfo InitSmoothOpenMethod =
            AccessTools.Method(
                typeof(WorldInteractiveObject.InteractionState),
                nameof(WorldInteractiveObject.InteractionState.InitSmoothOpen),
                new[]
                {
                    typeof(EDoorState),
                    typeof(float),
                    typeof(float),
                    typeof(AnimationCurve),
                    typeof(bool)
                });

        protected override MethodBase GetTargetMethod()
        {
            return InitSmoothOpenMethod;
        }

        [PatchPrefix]
        private static void Prefix(EDoorState state, ref float resultAngle)
        {
            if (!Plugin.Enabled.Value || !Plugin.AdjustOpenDirection.Value)
                return;

            if (!BreachKickContext.Active || state != EDoorState.Breaching || !BreachKickContext.ReverseDirection)
                return;

            resultAngle = -resultAngle;
        }
    }
}
