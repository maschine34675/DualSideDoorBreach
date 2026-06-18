using EFT.Interactive;

namespace DualSideDoorBreach
{
    internal static class BreachKickContext
    {
        public static bool Active { get; private set; }
        public static bool ReverseDirection { get; private set; }

        public static void Begin(Door door, UnityEngine.Vector3 yourPosition)
        {
            Active = true;
            ReverseDirection = DoorBreachUtil.ShouldReverseBreachDirection(door, yourPosition);
        }

        public static void End()
        {
            Active = false;
            ReverseDirection = false;
        }
    }
}
