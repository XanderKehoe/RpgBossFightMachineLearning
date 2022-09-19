using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStrike : CharacterAbility
{
    public DeathStrike()
    {
        this.name = AbilityName.DEATH_STRIKE;
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

        castingEntity.RestoreHealth((0.1f * castingEntity.stats.maxHealth) + (0.05f + diseaseCount));

        return 0;
    }
}
