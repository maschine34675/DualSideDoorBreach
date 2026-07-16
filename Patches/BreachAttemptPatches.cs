using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace DualSideDoorBreach.Patches
{
    internal sealed class MovementContextBreachAttemptPatch : ModulePatch
    {
        private static readonly MethodInfo BreachStartMethod =
            AccessTools.Method(typeof(MovementContext), "method_18");

        private static readonly FieldInfo PlayerField =
            AccessTools.Field(typeof(MovementContext), "_player");

        protected override MethodBase GetTargetMethod()
        {
            return BreachStartMethod;
        }

        [PatchPostfix]
        private static void Postfix(MovementContext __instance, WorldInteractiveObject interactive)
        {
            if (!Plugin.Enabled.Value)
                return;

            var door = interactive as Door;
            if (door == null || !__instance.NextBreachResult)
                return;

            var player = (Player)PlayerField.GetValue(__instance);
            if (!BreachAttemptService.IsHumanBreacher(player))
                return;
            var effectiveState = door.DoorState == EDoorState.Interacting
                ? door.FallbackState
                : door.DoorState;

            if (DoorBreachUtil.IsBreachOnlyInteractionDoor(door, effectiveState))
                return;

            if (BreachAttemptService.RollAttempt(door, player, __instance.InteractionParameters.InteractionPosition))
            {
                if (player.IsYourPlayer
                    && !Plugin.AllowBreachWithoutKey.Value
                    && DoorKeyUtil.AppliesLockedKeyRule(door))
                {
                    PendingKeyConsume.Set(door, player);
                }

                return;
            }

            __instance.NextBreachResult = false;
            __instance.PlayerAnimatorSetKickSucceed(false);
        }
    }
}
