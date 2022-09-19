using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obliterate : CharacterAbility
{
    public Obliterate()
    {
        this.name = AbilityName.OBLITERATE;
        this.damage = 100f;
        this.range = Character.MELEE_RANGE;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "MeleeAttack1", "MeleeAttack2" };
        this.statusEffects = new List<StatusEffect> { };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        uint diseaseCount = 0;
        foreach (StatusEffect statusEffect in receivingEntity.currentStatusEffects)
        {
            if (statusEffect.afflictingAbilityName == AbilityName.ICY_TOUCH || statusEffect.afflictingAbilityName == AbilityName.PLAGUE_STRIKE)
            {
                diseaseCount++;
            }
        }
        
        // clear out current 'diseases' on receiving entity.
        foreach (StatusEffect statusEffect in receivingEntity.currentStatusEffects)
        {
            if (statusEffect.afflictingAbilityName == AbilityName.ICY_TOUCH || statusEffect.afflictingAbilityName == AbilityName.PLAGUE_STRIKE)
            {
                statusEffect.time = 0;
            }
        }

        float damageModifier = 0.45f * diseaseCount;

        return damageModifier;
    }
}
