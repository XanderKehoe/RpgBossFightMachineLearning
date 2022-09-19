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
    RAIN_OF_FIRE,
    FLAME_BREATH,
}

[System.Serializable]
public abstract class Ability
{
    public AbilityName name { get; protected set; } // name of the ability
    public float damage { get; protected set; } // how much base damage does the ability deal
    public float range { get; protected set; } // how much base range does the ability have
    public List<string> animationNameList { get; protected set; } // Which animation(s) correlate to this ability
    public List<StatusEffect> statusEffects { get; protected set; } // The status effects this ability applies.

    public List<StatusEffect> GetStatusEffectListClone()
    {
        List<StatusEffect> clonedStatusEffects = new();

        foreach (StatusEffect statusEffect in statusEffects)
            clonedStatusEffects.Add(statusEffect.Clone());

        return clonedStatusEffects;
    }

    public string GetAnimationName()
    {
        void CheckAnimationName(string str)
        {
            switch (str)
            {
                case "MeleeAttack1":
                case "MeleeAttack2":
                case "CastSpell":
                case "CastHeal":
                case "ShootArrow":
                case "Basic Attack":
                case "Flame Breath":
                    break;

                default:
                    Debug.LogError("CheckAnimationName - [" + str + "] from: "+name.ToString());
                    break;
            }
        }

        if (animationNameList.Count == 0)
            Debug.LogError("GetAnimationName() - No animations for ability: " + name.ToString());

        string randAnimation = animationNameList[Random.Range(0, animationNameList.Count)];

        CheckAnimationName(randAnimation);

        return randAnimation;
    }

    public abstract float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity);
}
