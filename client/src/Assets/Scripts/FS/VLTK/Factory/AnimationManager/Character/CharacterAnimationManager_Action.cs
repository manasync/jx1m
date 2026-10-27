using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FS.VLTK.Entities.Enum;

namespace FS.VLTK.Factory.Animation
{
    /// <summary>
    /// Quản lý động tác
    /// </summary>
    public partial class CharacterAnimationManager
    {
        /// <summary>
        /// Động tác
        /// </summary>
        public class PlayerAction
        {
            /// <summary>
            /// Loại động tác
            /// </summary>
            public PlayerActionType Type { get; set; }

            /// <summary>
            /// Tên động tác khi cưỡi
            /// </summary>
            public string Ride { get; set; }

            /// <summary>
            /// Tên động tác khi ở trạng thái thường
            /// </summary>
            public string Normal { get; set; }
        }

        public class ActionByWeapon
        {
            /// <summary>
            /// ID vũ khí
            /// </summary>
            public string WeaponID { get; set; }

            /// <summary>
            /// Danh sách động tác với 2 trạng thái tương ứng
            /// </summary>
            public Dictionary<PlayerActionType, PlayerAction> Actions { get; set; }
        }

        /// <summary>
        /// Danh sách tên động tác theo loại và vũ khí
        /// </summary>
        public Dictionary<string, ActionByWeapon> ActionNames { get; set; } = new Dictionary<string, ActionByWeapon>();

        /// <summary>
        /// Lấy tên động tác dựa theo loại
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="weaponID"></param>
        /// <param name="isRide"></param>
        /// <returns></returns>
        public string GetActionName(PlayerActionType actionType, string weaponID, bool isRide)
        {
            /// Nếu tên động tác theo vũ khí tồn tại
            if (this.ActionNames != null && this.ActionNames.TryGetValue(weaponID, out ActionByWeapon actionInfo))
            {
                if (actionInfo.Actions != null && actionInfo.Actions.TryGetValue(actionType, out PlayerAction actionName))
                {
                    string res = isRide ? actionName.Ride : actionName.Normal;
                    if (!string.IsNullOrEmpty(res))
                    {
                        return res;
                    }
                }
            }

            // Fallback default action mapping
            if (isRide)
            {
                switch (actionType)
                {
                    case PlayerActionType.FightStand:
                    case PlayerActionType.NormalStand:
                    case PlayerActionType.Sit:
                    case PlayerActionType.Jump:
                        return "RideStand";
                    case PlayerActionType.Walk:
                        return "RideWalk";
                    case PlayerActionType.Run:
                    case PlayerActionType.RunAttack:
                        return "RideRun";
                    case PlayerActionType.NormalAttack:
                    case PlayerActionType.CritAttack:
                        return "RideCut";
                    case PlayerActionType.Magic:
                        return "RideMagic";
                    case PlayerActionType.Wound:
                        return "RideWound";
                    case PlayerActionType.Die:
                        return "RideDie";
                    default:
                        return "RideStand";
                }
            }
            else
            {
                switch (actionType)
                {
                    case PlayerActionType.FightStand:
                    case PlayerActionType.NormalStand:
                        return "FreeStand1";
                    case PlayerActionType.Walk:
                        return "FreeWalk";
                    case PlayerActionType.Run:
                    case PlayerActionType.RunAttack:
                        return "FreeRun";
                    case PlayerActionType.NormalAttack:
                    case PlayerActionType.CritAttack:
                        return "FreeAttack";
                    case PlayerActionType.Magic:
                        return "FreeMagic";
                    case PlayerActionType.Wound:
                        return "FreeWound";
                    case PlayerActionType.Die:
                        return "FreeDie";
                    case PlayerActionType.Sit:
                        return "SitDown";
                    case PlayerActionType.Jump:
                        return "JumpFly";
                    default:
                        return "FreeStand1";
                }
            }
        }
    }
}
