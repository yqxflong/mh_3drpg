using System.Collections.Generic;

namespace Hotfix_LT.Player
{
    public struct CharacterConstants
    {
        public struct Atribute
        {
            public const string HUO = "Ty_Huo";
            public const string SHUI = "Ty_Shui";
            public const string JIN = "Ty_Jin";
            public const string MU = "Ty_Mu";
            public const string TU = "Ty_Tu";
        }

        public enum eRaceAttribute
        {
            None = 0,
            Jin = 1,
            Mu = 2,
            Shui = 3,
            Huo = 4,
            Tu = 5,
            Max
        }

        public static readonly string[] RaceHeroNames = new string[]
        {
        "none",
        "yuanhao",
        "muniuma",
        "linglong",
        "lieyan",
        "huanggang"
        };

        public enum eCombatAttribute
        {
            MaxHp,
            MaxMp,
            Speed,
            Miss,
            Critical,
            CriticalFactor,
            PhysicAttack,
            MagicAttack,
            PhysicDefend,
            MagicDefend,
            Penetration,
            SpellPenetration,
            DamageReduction,
            Stun,
            HpRecover,
        }

        // public static readonly Dictionary<eCombatAttribute, string> AllAttributesName = new Dictionary<eCombatAttribute, string>()
        // {
        //     {eCombatAttribute.MaxHp,"ID_MaxHP"},
        //     //{"maxmp", eCombatAttribute.MaxMp },
        //     {eCombatAttribute.Speed,"ID_COMBAT_TIPS_ATTR_SPEED"},
        //     {eCombatAttribute.Miss,"ID_COMBAT_TIPS_ATTR_MISS"},
        //     {eCombatAttribute.Critical,"ID_COMBAT_TIPS_ATTR_CRITICAL"},
        //     {eCombatAttribute.CriticalFactor,"ID_COMBAT_TIPS_ATTR_CRITICAL_FACTOR"},
        //     {eCombatAttribute.PhysicAttack,"ID_COMBAT_TIPS_ATTR_PHYSIC_ATTACK"},
        //     {eCombatAttribute.MagicAttack,"ID_COMBAT_TIPS_ATTR_MAGIC_ATTACK"},
        //     {eCombatAttribute.PhysicDefend,"ID_COMBAT_TIPS_ATTR_PHYSIC_DEFEND"},
        //     {eCombatAttribute.MagicDefend,"ID_COMBAT_TIPS_ATTR_MAGIC_DEFEND"},
        //     {eCombatAttribute.Penetration,"ID_COMBAT_TIPS_ATTR_PENETRATION"},
        //     {eCombatAttribute.SpellPenetration,"ID_COMBAT_TIPS_ATTR_SPELL_PENETRATION"},
        //     {eCombatAttribute.DamageReduction,"ID_COMBAT_TIPS_ATTR_DAMAGE_REDUCTION"},
        //     {eCombatAttribute.Stun,"ID_COMBAT_TIPS_ATTR_STUN"},
        //     {eCombatAttribute.HpRecover,"ID_COMBAT_TIPS_ATTR_HP_RECOVER"},
        // };

        // public static readonly Dictionary<string, eCombatAttribute> CombatAttributeNames = new Dictionary<string, eCombatAttribute>()
        // {
        //     {"maxhp", eCombatAttribute.MaxHp },
        //     {"maxmp", eCombatAttribute.MaxMp },
        //     {"speed", eCombatAttribute.Speed },
        //     {"dodgerating", eCombatAttribute.Miss },
        //     {"critical", eCombatAttribute.Critical },
        //     {"criticalfactor", eCombatAttribute.CriticalFactor },
        //     {"patk", eCombatAttribute.PhysicAttack },
        //     {"matk", eCombatAttribute.MagicAttack },
        //     {"pdef", eCombatAttribute.PhysicDefend },
        //     {"mdef", eCombatAttribute.MagicDefend },
        //     {"penetration", eCombatAttribute.Penetration },
        //     {"spellpenetration", eCombatAttribute.SpellPenetration },
        //     {"damagereduction", eCombatAttribute.DamageReduction },
        //     {"stun", eCombatAttribute.Stun },
        //     {"hprecover", eCombatAttribute.HpRecover },
        // };

        // public enum eRole
        // {
        //     Hero = 0,
        //     Partner = 1,
        //     Enemy = 2,
        //     NPC = 3,
        //     Summon = 4,
        // }

        // public enum eGender
        // {
        //     Male = 1,
        //     Female = 2,
        // }
    }

    public class CombatAttributes
    {
        private Dictionary<CharacterConstants.eCombatAttribute, int> mAttributes = new Dictionary<CharacterConstants.eCombatAttribute, int>();

        public int Get(CharacterConstants.eCombatAttribute attribute)
        {
            if (mAttributes.ContainsKey(attribute))
            {
                return mAttributes[attribute];
            }

            return 0;
        }

        public void Set(CharacterConstants.eCombatAttribute attribute, int value)
        {
            mAttributes[attribute] = value;
        }
    }
}