namespace DualSideDoorBreach.Patches
{
    internal static class DoorDashCompatPatcher
    {
        public static void TryEnableAll()
        {
            DoorDashSwingCheckPatch.TryEnable();
            DoorDashLockedDoorPatch.TryEnable();
        }
    }
}
