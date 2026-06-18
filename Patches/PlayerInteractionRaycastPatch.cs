using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DualSideDoorBreach.Patches
{
    internal sealed class PlayerInteractionRaycastPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.InteractionRaycast));
        }

        [PatchTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var operatableField = AccessTools.Field(typeof(WorldInteractiveObject), nameof(WorldInteractiveObject.Operatable));
            var helper = AccessTools.Method(typeof(DoorBreachUtil), nameof(DoorBreachUtil.AllowsInteractionRaycast));

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].LoadsField(operatableField))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, helper);
                }
            }

            return codes;
        }
    }
}
