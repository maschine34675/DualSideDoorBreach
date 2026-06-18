using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace DualSideDoorBreach.Patches
{
    internal sealed class GetDoorBreachActionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), "smethod_14");
        }

        [PatchPostfix]
        private static void Postfix(GamePlayerOwner owner, Door door, ActionsReturnClass __result)
        {
            if (!Plugin.Enabled.Value || __result?.Actions == null)
                return;

            if (door.DoorState == EDoorState.Open)
                return;

            if (door.DoorState != EDoorState.Shut && door.DoorState != EDoorState.Locked)
                return;

            foreach (var action in __result.Actions)
            {
                if (action.Name != "Breach")
                    continue;

                if (DoorBreachUtil.IsBreachOnlyInteractionDoor(door))
                {
                    if (DoorKeyUtil.AppliesLockedKeyRule(door)
                        && !DoorKeyUtil.PlayerCanBreachLockedDoor(owner, door))
                    {
                        action.Disabled = true;
                    }

                    break;
                }

                action.Disabled = !DoorBreachUtil.IsBreachActionAvailable(owner, door);
                break;
            }
        }
    }
}
