using UnityEngine;

namespace Project.Enums
{
    #region Setting
    public enum EVolumeType
    {
    BGM,
    Voice,
    Effect,
    System
    }
    #endregion

    #region Status Effect
    public enum EStatusEffectApplyPolicy
    {
        OnTriggered,
        OnDamageOverTime
    }
    #endregion

    #region Character
    public enum EStatusEffect
    {
        None,
        Bleeding,
        Posioning
    }

    public enum EWeaponState
    {
        Sword,
        Pistol
    }

    public enum TeamID
    {
        Hero,
        Enemy,
        Neutrality
    }

    public enum MovementMode
    {
        Ground,
        Water
    }
    #endregion

    #region Actor
    public enum EProjectileType
    {
        UnityChan_Normal,
        UnityChan_Ability,
        WindWisp,
        WaterWisp,
        EarthWisp
    }

    public enum EBulletDestroyTypes
    {
        Destructible,
        InDestructible
    }

    public enum EBulletTypes
    {
        Normal,
        Penetration
    }

    public enum EHealType
    {
        HPType,
        MPType
    }

    public enum EItemType
    {
        HP_Potion,
        MP_Potion,
        Gold
    }

    public enum EChestType
    {
        Normal,
        Special,
        Mimic
    }

    public enum EFallingObjectType
    {
        Rock
    }

    public enum EEffectType
    {
        JustEvasion,
        PerfectEvasion,
        HP_Recovery,
        HP_Regen,
        MP_Recovery,
        Buff_PowerUp
    }
    #endregion

    #region Reward Item
    public enum ERewardType
    {
        HP_Potion,
        MP_Potion,
        Gold,
        Equipment
    }
    #endregion

    // ----KHT----

    #region FloatingDamage

    public enum DamageType
    {
        Default,
        Critical,
        Heal,
    }

    #endregion

    #region Language

    public enum Language
    {
        EN,
        KO,
    }

    #endregion

    #region DataTable

    public enum CharacterType
    {
        PC = 1,
        NPC,
        Monster
    }

    public enum ItemType
    {
        Consumable = 1,
        StableCore
    }

    public enum PriceType
    {
        Gold = 1,
        Token
    }

    public enum NextDialogueType
    {
        End = 1,
        Dialogue,
        Choice
    }

    public enum AchievementType
    {
        Location = 1,
        Item,
        Battle
    }

    public enum AchievementMethodType
    {
        ReachTargetLocation = 1,
        UseItem,
        ObtainItem,
        EliminateTarget
    }

    #endregion
}

namespace Project.Structs
{
    public struct FSwordHitBox
    {
        public Vector2 Offset; // 히트박스의 위치
        public Vector2 Size;   // 히트박스의 크기
    }

    public struct FCameraShakeOffset
    {
        public float ImpulseDuration;
        public float ShakeDuration;
        public float MaxImpactForce;
        public float Magnitude;
    }
}