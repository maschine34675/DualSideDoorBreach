using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace DualSideDoorBreach.Patches
{
    internal sealed class DoorLockedBreachInteractPatch : ModulePatch
    {
        private static readonly MethodInfo InteractMethod =
            AccessTools.Method(typeof(Door), nameof(Door.Interact), new[] { typeof(InteractionResult) });

        protected override MethodBase GetTargetMethod()
        {
            return InteractMethod;
        }

        [HarmonyBefore(DoorDashCompat.PluginGuid)]
        [PatchPrefix]
        private static bool Prefix(Door __instance, InteractionResult interactionResult)
        {
            if (!Plugin.Enabled.Value || interactionResult.InteractionType != EInteractionType.Breach)
                return true;

            var player = DoorBreachUtil.ResolveLocalPlayer(__instance.InteractingPlayer);
            if (!DoorKeyUtil.PlayerCanBreachLockedDoor(__instance, player))
                return false;

            if (Plugin.AdjustOpenDirection.Value && player != null)
                BreachKickContext.Begin(__instance, player.Position);

            return true;
        }
    }

    internal sealed class MovementContextLockedBreachPatch : ModulePatch
    {
        private static readonly MethodInfo BreachStartMethod =
            AccessTools.Method(typeof(MovementContext), "method_18");

        private static readonly FieldInfo PlayerField =
            AccessTools.Field(typeof(MovementContext), "_player");

        protected override MethodBase GetTargetMethod()
        {
            return BreachStartMethod;
        }

        [PatchPrefix]
        private static bool Prefix(MovementContext __instance, WorldInteractiveObject interactive)
        {
            if (!Plugin.Enabled.Value)
                return true;

            var door = interactive as Door;
            if (door == null)
                return true;

            var player = (Player)PlayerField.GetValue(__instance);
            if (!DoorKeyUtil.PlayerCanBreachLockedDoor(door, player))
                return false;

            if (Plugin.AdjustOpenDirection.Value && player != null)
                BreachKickContext.Begin(door, player.Position);

            return true;
        }
    }
}
