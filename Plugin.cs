using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DualSideDoorBreach.Patches;

namespace DualSideDoorBreach
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(DoorDashCompat.PluginGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.maschine.DualSideDoorBreach";
        public const string PluginName = "maschine-DualSideDoorBreach";
        public const string PluginVersion = "1.0.0";

        public static ManualLogSource Log;
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> AdjustOpenDirection;
        public static ConfigEntry<bool> AllowNonBreachableDoors;
        public static ConfigEntry<bool> RequireKeyForLockedBreach;
        public static ConfigEntry<bool> ConsumeKeyOnLockedBreach;
        public static ConfigEntry<bool> DoorDashCompatibility;

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "Allow breaching doors from both sides.");
            AdjustOpenDirection = Config.Bind("General", "AdjustOpenDirection", true,
                "Swing the door away from the breaching player instead of always using the default direction.");
            AllowNonBreachableDoors = Config.Bind("General", "AllowNonBreachableDoors", true,
                "Also allow breaching doors marked as non-breachable in the map data (CanInteractWithBreach / CanBeBreached).");
            RequireKeyForLockedBreach = Config.Bind("Locked Doors", "RequireMatchingKey", true,
                "Locked doors can only be breached when the matching key is in your inventory.");
            ConsumeKeyOnLockedBreach = Config.Bind("Locked Doors", "ConsumeKeyOnBreach", true,
                "Use up one charge of the matching key when breaching a locked door.");
            DoorDashCompatibility = Config.Bind("Compatibility", "DoorDash", true,
                "Allow DoorDash sprint-ram breaching from both sides (requires the DoorDash mod).");

            if (Enabled.Value)
            {
                new DoorIsBreachAnglePatch().Enable();
                new DoorBreachSuccessRollPatch().Enable();
                new DoorKickOpenVectorPatch().Enable();
                new DoorKickOpenBoolPatch().Enable();
                new DoorInitSmoothOpenPatch().Enable();
                new GetDoorBreachActionPatch().Enable();
                new DoorLockedBreachInteractPatch().Enable();
                new MovementContextLockedBreachPatch().Enable();
                new PlayerInteractionRaycastPatch().Enable();
                DoorDashCompatPatcher.TryEnableAll();
            }

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }
    }
}
