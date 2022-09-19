using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    HealthPerSecond,
    AttackSpeed,
    DamageAbsorption,
    HitChanceOnSelf, // Entities with this status effect are less(+)/more(-) likely to be hit by enemies attacking them.
    HitChanceOnTarget, // Entities with this status effect are more(+)/less(-) likely to hit their target when attacking.
    AttackPower,
    ManaPerSecond,
    HealingMultiplier,
    ThreatGeneration,
    BlockChance,
    CriticalStrikeChanceOnSelf, // Entities with this status effect are less(+)/more(-) likely to be hit by enemies attacking them.
    CriticalStrikeChanceOnTarget, // Entities with this status effect are more(+)/less(-) likely to land critical strikes.
    MovementSpeedMultiplier
}

public enum StatusEffectTarget
{
    Self,
    SelectedTarget,
    AllFriendlies,
    AllEnemies,
    AreaOfEffect, // applies only to entities within a specific area, to be decided by each ability
}

public enum AreaOfEffectType
{
    Cone,
    Circle
}

[System.Serializable]
public class StatusEffect
{
    public AbilityName afflictingAbilityName;
    public StatusEffectType type;
    public StatusEffectTarget target;
    public float strength;
    public uint time;

    public StatusEffect(StatusEffectType type, StatusEffectTarget target, float strength, uint time)
    {
        this.type = type;
        this.target = target;
        this.strength = strength;
        this.time = time;

        if (strength == 0)
        {
            Debug.LogWarning("New StatusEffect of type '" + type.ToString() + "' created with a strength of 0");
        }

        if (type == StatusEffectType.DamageAbsorption && strength < 0)
        {
            Debug.LogWarning("New StatusEffect of type 'DamageAbsorption' created with strength < 0, unpredictable behavior may occur...");
        }
    }

    public StatusEffect(StatusEffectType type, StatusEffectTarget target, float strength, uint time, AbilityName afflictingAbilityName)
    {
        this.type = type;
        this.target = target;
        this.strength = strength;
        this.time = time;
        this.afflictingAbilityName = afflictingAbilityName;

        if (strength == 0)
        {
            Debug.LogWarning("New StatusEffect of type '" + type.ToString() + "' created with a strength of 0");
        }

        if (type == StatusEffectType.DamageAbsorption && strength < 0)
        {
            Debug.LogWarning("New StatusEffect of type 'DamageAbsorption' created with strength < 0, unpredictable behavior may occur...");
        }
    }

    public StatusEffect Clone()
    {
        return new StatusEffect(type, target, strength, time, afflictingAbilityName);
    }

    /* Example constructions
     * DamageOverTime, -15 health every second for 10 seconds: 
     * new StatusEffect(StatusEffectType.HealthOverTime, -15, 10
     * AttackSpeed, -20% attack speed every second for 15 seconds:
     * new StatusEffect(StatusEffectType.AttackSpeed, -20, 15)
     */
}
