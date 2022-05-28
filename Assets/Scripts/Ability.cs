using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityName
{
    BASIC_ATTACK,
    UPDATE_PER_SECOND,

    // Paladin
    AVENGERS_SHIELD,
    DIVINE_SHIELD,
    HAMMER_OF_THE_RIGHTEOUS,
    HAND_OF_PROTECTION,
    HAND_OF_RECKONING,
    JUDGEMENT,

    // Death Knight
    ICY_TOUCH,
    PLAGUE_STRIKE,
    SCOURGE_STRIKE,
    HORN_OF_WINTER,
    OBLITERATE,
    DEATH_STRIKE,

    // Mage
    PYROBLAST,
    FROSTBOLT,
    SCORCH,
    EVOCATION,
    TRANSFER_MANA,
    TIME_WARP,

    // Hunter
    SERPENT_STING,
    CHIMERA_SHOT,
    KILL_SHOT,
    FOCUSED_AIM,
    RAPID_FIRE,
    TRANQUILIZING_DART,

    // Priest
    HOLY_LIGHT,
    RENEW,
    HOLY_SHIELD,
    PRAYER_OF_MENDING,
    SMITE,
    BLINDING_LIGHT,
    INNER_FIRE,

    // Dragon
    RAIN_OF_FIRE
}

[System.Serializable]
public class Ability
{
    public enum AnimationType
    {
        // Characters
        MeleeAttack1,
        MeleeAttack2,
        CastSpell,
        CastHeal,
        ShootArrow,

        // Dragon TODO
        BasicAttack
    }

    public AbilityName name { get; protected set; }
    public float damage { get; protected set; }
    public float range { get; protected set; }
    public AnimationType animationType { get; protected set; }
    public List<StatusEffect> statusEffects;

    public virtual Ability Clone()
    {
        Debug.LogError("Ability - Base clone method called, should be overwritten");
        return null;
    }
}

[System.Serializable]
public class CharacterAbility : Ability
{
    public float manaCost;

    public CharacterAbility(AbilityName name, float damage, float range, float manaCost, AnimationType animationType, List<StatusEffect> statusEffects)
    {
        this.name = name;
        this.damage = damage;
        this.range = range + Character.MELEE_RANGE;
        this.manaCost = manaCost;
        this.animationType = animationType;
        this.statusEffects = statusEffects;
    }

    override public Ability Clone()
    {
        List<StatusEffect> clonedStatusEffects = new();

        foreach (StatusEffect statusEffect in statusEffects)
            clonedStatusEffects.Add(statusEffect.Clone());

        return new CharacterAbility(name, damage, range, manaCost, animationType, clonedStatusEffects);
    }
}

[System.Serializable]
public class PlainAiAbility : Ability
{
    public float cooldownTime;

    public PlainAiAbility(AbilityName name, float damage, float range, float cooldownTime, AnimationType animationType, List<StatusEffect> statusEffects)
    {
        this.name = name;
        this.damage = damage;
        this.range = range + Character.MELEE_RANGE;
        this.cooldownTime = cooldownTime;
        this.animationType = animationType;
        this.statusEffects = statusEffects;
    }

    override public Ability Clone()
    {
        List<StatusEffect> clonedStatusEffects = new();

        foreach (StatusEffect statusEffect in statusEffects)
            clonedStatusEffects.Add(statusEffect.Clone());

        return new PlainAiAbility(name, damage, range, cooldownTime, animationType, clonedStatusEffects);
    }
}
