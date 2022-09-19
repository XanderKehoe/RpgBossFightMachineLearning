using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScourgeStrike : CharacterAbility
{
    public ScourgeStrike()
    {
        this.name = AbilityName.SCOURGE_STRIKE;
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

        float damageModifier = 0.15f * diseaseCount;

        return damageModifier;
    }
}
