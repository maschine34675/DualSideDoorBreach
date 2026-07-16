using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using System.Linq;

namespace DualSideDoorBreach
{
    internal static class DoorKeyUtil
    {
        public static bool IsLockedBreachTarget(Door door)
        {
            return door.DoorState == EDoorState.Locked
                || door.FallbackState == EDoorState.Locked;
        }

        public static bool AppliesLockedKeyRule(Door door)
        {
            return Plugin.RequireKeyForLockedBreach.Value
                && IsLockedBreachTarget(door)
                && !string.IsNullOrEmpty(door.KeyId);
        }

        public static bool PlayerCanBreachLockedDoor(GamePlayerOwner owner, Door door)
        {
            return PlayerCanBreachLockedDoor(door, owner?.Player);
        }

        public static bool PlayerCanBreachLockedDoor(Door door, Player player)
        {
            if (!Plugin.RequireKeyForLockedBreach.Value)
                return true;
            if (Plugin.AllowBreachWithoutKey.Value)
                return true;

            if (door.DoorState != EDoorState.Locked)
                return true;
            if (string.IsNullOrEmpty(door.KeyId))
                return true;

            return HasMatchingKeyAndSkill(door, player);
        }
        public static bool HasMatchingKeyAndSkill(Door door, Player player)
        {
            if (player == null)
                return false;

            if (GetMatchingKey(player, door) == null)
                return false;

            return MeetsSkillRequirement(player, door);
        }

        public static bool TryConsumeKeyForBreach(Door door, Player player)
        {
            if (!Plugin.RequireKeyForLockedBreach.Value
                || !Plugin.ConsumeKeyOnLockedBreach.Value
                || !IsLockedBreachTarget(door))
            {
                return true;
            }

            if (player == null || string.IsNullOrEmpty(door.KeyId))
                return true;

            var key = GetMatchingKey(player, door);
            if (key == null)
                return false;

            if (!MeetsSkillRequirement(player, door))
                return false;

            GStruct154<GClass3408> discardResult = default;
            key.NumberOfUsages++;

            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage
                && key.Template.MaximumNumberOfUsage > 0)
            {
                discardResult = InteractionsHandlerClass.Discard(
                    key.Item,
                    (TraderControllerClass)key.Item.Parent.GetOwner(),
                    false);

                if (discardResult.Failed)
                    return false;
            }

            GClass3408 discardedItem = discardResult.Succeeded ? discardResult.Value : null;
            var interactionResult = new KeyInteractionResultClass(key, discardedItem, true);
            interactionResult.RaiseEvents(player.InventoryController, CommandStatus.Begin);
            interactionResult.RaiseEvents(player.InventoryController, CommandStatus.Succeed);
            return true;
        }

        private static KeyComponent GetMatchingKey(Player player, WorldInteractiveObject door)
        {
            if (player == null || string.IsNullOrEmpty(door.KeyId))
                return null;

            return player.InventoryController.Inventory.Equipment
                .GetItemComponentsInChildren<KeyComponent>(false)
                .FirstOrDefault(key => key.Template.KeyId == door.KeyId);
        }

        private static bool MeetsSkillRequirement(Player player, WorldInteractiveObject door)
        {
            if (!door.HasSkillRequirement)
                return true;

            if (!player.Skills.TryGetSkill(door.SkillRequirement, out var skill))
                return false;

            return skill.Level >= door.SkillMinLevelRequirement;
        }
    }
}
