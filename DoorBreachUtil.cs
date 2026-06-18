using EFT;
using EFT.Interactive;
using UnityEngine;

namespace DualSideDoorBreach
{
    internal static class DoorBreachUtil
    {
        public static bool IsBreachOnlyInteractionDoor(Door door)
        {
            if (door == null)
                return false;

            if (door.CanInteractWithBreach
                && door.DoorState == EDoorState.Locked
                && string.IsNullOrEmpty(door.KeyId))
            {
                return true;
            }

            if (door.Operatable)
                return false;

            return door.CanInteractWithBreach || door.CanBeBreached;
        }

        public static bool AllowsInteractionRaycast(WorldInteractiveObject worldObject)
        {
            if (worldObject == null)
                return false;

            if (!Plugin.Enabled.Value)
                return worldObject.Operatable;

            if (worldObject.Operatable)
                return true;

            return IsBreachOnlyInteractionDoor(worldObject as Door);
        }

        public static bool IsBreachAllowed(Door door)
        {
            if (Plugin.AllowNonBreachableDoors.Value)
                return door.CanInteractWithBreach || door.Operatable;

            if (IsBreachOnlyInteractionDoor(door))
                return true;

            if (!door.Operatable)
                return door.CanInteractWithBreach || door.CanBeBreached;

            return door.CanInteractWithBreach;
        }

        public static bool IsBreachActionAvailable(GamePlayerOwner owner, Door door)
        {
            if (!IsBreachAllowed(door))
                return false;

            if (!EvaluateIsBreachAngle(door, owner.Player.Position))
                return false;

            if (IsProneBreachBlocked(owner.Player.MovementContext.CurrentState.Name))
                return false;

            return DoorKeyUtil.PlayerCanBreachLockedDoor(owner, door);
        }

        public static bool EvaluateIsBreachAngle(Door door, Vector3 yourPosition)
        {
            if (!IsBreachAllowed(door))
                return false;

            if (IsBreachOnlyInteractionDoor(door))
                return VanillaIsBreachAngle(door, yourPosition);

            return true;
        }

        public static bool VanillaIsBreachAngle(Door door, Vector3 yourPosition)
        {
            var toPlayer = door.transform.TransformPoint(door.viewTarget1) - yourPosition;
            var shutForward = door.GetDoorRotation(door.GetAngle(EDoorState.Shut))
                * WorldInteractiveObject.GetRotationAxis(door.DoorForward, door.transform);

            var toPlayerXZ = new Vector2(toPlayer.x, toPlayer.z).normalized;
            var shutXZ = new Vector2(shutForward.x, shutForward.z).normalized;

            return Mathf.Abs(Vector2.Dot(toPlayerXZ, shutXZ))
                < EFTHardSettings.Instance.DOOR_BREACH_THRESHOLD;
        }

        public static bool ShouldReverseBreachDirection(Door door, Vector3 yourPosition)
        {
            if (IsBreachOnlyInteractionDoor(door))
                return false;

            return !VanillaBreachWouldSucceed(door, yourPosition);
        }

        public static bool VanillaBreachSuccessRoll(Door door, Vector3 yourPosition)
        {
            var toPlayer = door.transform.TransformPoint(door.viewTarget1) - yourPosition;
            var shutForward = door.GetDoorRotation(door.GetAngle(EDoorState.Shut))
                * WorldInteractiveObject.GetRotationAxis(door.DoorForward, door.transform);
            var openForward = door.GetDoorRotation(door.GetAngle(EDoorState.Open))
                * WorldInteractiveObject.GetRotationAxis(door.DoorForward, door.transform);
            var swingHint = shutForward + openForward;

            var toPlayerXZ = new Vector2(toPlayer.x, toPlayer.z).normalized;
            var swingHintXZ = new Vector2(swingHint.x, swingHint.z).normalized;

            return Vector2.Dot(toPlayerXZ, swingHintXZ) > 0f;
        }

        public static bool VanillaBreachWouldSucceed(Door door, Vector3 yourPosition)
        {
            if (IsBreachOnlyInteractionDoor(door))
                return VanillaBreachSuccessRoll(door, yourPosition);

            if (!door.Operatable)
                return false;

            return VanillaBreachSuccessRoll(door, yourPosition);
        }

        public static bool IsProneBreachBlocked(EPlayerState playerState)
        {
            return playerState == EPlayerState.ProneIdle
                || playerState == EPlayerState.ProneMove
                || playerState == EPlayerState.Prone2Stand;
        }

        public static Player ResolveLocalPlayer(IPlayer interactingPlayer)
        {
            return (interactingPlayer as Player) ?? GamePlayerOwner.MyPlayer;
        }
    }
}
