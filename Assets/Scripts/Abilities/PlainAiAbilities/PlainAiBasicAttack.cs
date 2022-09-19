using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainAiBasicAttack : PlainAiAbility
{
    // All character's have a basic attack ability which can have different damage, range, and animations.
    public PlainAiBasicAttack(float damage, float range, List<string> animationNameList)
    {
        this.name = AbilityName.BASIC_ATTACK;
        this.damage = damage;
        this.range = range;
        this.cooldownTime = 0f;
        this.animationNameList = animationNameList;
        this.statusEffects = new List<StatusEffect> { };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0; // no special effects or damage modifiers
    }
}
