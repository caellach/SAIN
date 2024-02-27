using Comfort.Common;
using EFT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SAIN.Helpers
{
    public static class BotMoverHelper
    {

        static FieldInfo BotOwnerField = AccessTools.Field(typeof(BotMover), "botOwner_0");

        /**
         * Copied from the original code in 26535
         */
        public static Vector3 GetDirDestination(this BotMover botMover)
        {
            var botOwner = (BotOwner)BotOwnerField.GetValue(botMover);
            var dirDestination = botOwner.Destination - botOwner.Transform.position ?? Vector3.zero;

            return dirDestination;
        }
    }

    public static class ShootDataHelper
    {
        static FieldInfo BotOwnerField = AccessTools.Field(typeof(ShootData), "_owner");
        static FieldInfo PlayerField = AccessTools.Field(typeof(ShootData), "_player");
        static MethodInfo method_3 = AccessTools.Method(typeof(ShootData), "method_3");

        /**
         * Copied from the original code in 26535
         */
        public static bool ChecFriendlyFire(this ShootData shootData, Vector3 from, Vector3 to)
        {
            Player player = (Player)method_3.Invoke(shootData, new object[] { from, to });
            BotOwner _owner = (BotOwner)BotOwnerField.GetValue(shootData);
            Player _player = (Player)PlayerField.GetValue(shootData);
            return player != null && player.Id != _owner.Id && (_owner.Memory.GoalEnemy == null || !(player == Singleton<GameWorld>.Instance.GetAlivePlayerByProfileID(_owner.Memory.GoalEnemy.Person.ProfileId))) && player.Profile.Info.Side == _player.Side;
        }
    }

}
