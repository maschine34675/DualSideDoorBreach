using EFT;
using EFT.Interactive;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DualSideDoorBreach
{
    internal static class BreachAttemptService
    {
        private sealed class AttemptState
        {
            public int Count;
        }

        private static readonly ConditionalWeakTable<Door, AttemptState> States =
            new ConditionalWeakTable<Door, AttemptState>();
        public static bool IsHumanBreacher(Player player)
        {
            if (player == null)
                return false;

            if (player.IsYourPlayer)
                return true;

            return player.Profile?.Info?.GroupId == "Fika";
        }

        public static bool RollAttempt(Door door, Player player, Vector3 interactionPosition)
        {
            bool wrongSideGate = Plugin.WrongSideKicksNeeded.Value > 1
                && DoorBreachUtil.ShouldReverseBreachDirection(door, interactionPosition);

            bool noKeyGate = Plugin.AllowBreachWithoutKey.Value
                && Plugin.RequireKeyForLockedBreach.Value
                && DoorKeyUtil.IsLockedBreachTarget(door)
                && !string.IsNullOrEmpty(door.KeyId);

            if (!wrongSideGate && !noKeyGate)
                return true;

            var state = States.GetOrCreateValue(door);
            state.Count++;

            if (wrongSideGate && state.Count < Plugin.WrongSideKicksNeeded.Value)
                return false;

            if (noKeyGate)
            {
                float chance = Mathf.Min(1f,
                    Plugin.NoKeyBaseChance.Value + Plugin.NoKeyChanceIncrease.Value * (state.Count - 1));

                if (Hash01(door.Id, player.ProfileId, state.Count) >= chance)
                    return false;
            }

            States.Remove(door);
            return true;
        }
        private static float Hash01(string doorId, string profileId, int attempt)
        {
            unchecked
            {
                uint hash = 2166136261u;
                hash = MixString(hash, doorId);
                hash = MixString(hash, profileId);
                hash = (hash ^ (uint)attempt) * 16777619u;
                return (hash & 0xFFFFFFu) / (float)0x1000000;
            }
        }

        private static uint MixString(uint hash, string value)
        {
            unchecked
            {
                if (value != null)
                {
                    foreach (char c in value)
                        hash = (hash ^ c) * 16777619u;
                }

                return (hash ^ 0x7Cu) * 16777619u;
            }
        }
    }
    internal static class PendingKeyConsume
    {
        private const float ValidSeconds = 10f;

        private static Door _door;
        private static Player _player;
        private static float _setTime;

        public static void Set(Door door, Player player)
        {
            _door = door;
            _player = player;
            _setTime = Time.realtimeSinceStartup;
        }

        public static bool TryTake(Door door, out Player player)
        {
            player = null;
            if (_door == null || !ReferenceEquals(_door, door))
                return false;

            if (Time.realtimeSinceStartup - _setTime <= ValidSeconds)
                player = _player;

            _door = null;
            _player = null;
            return player != null;
        }
    }
}
