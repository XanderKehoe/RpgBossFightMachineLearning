using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Special 'update' ability to work with updated ability structure
// Provides an 'ability' for the UpdatePerSecond to use passing into CalculateAndTakeDamageFromAbility function
public class UpdateAbility : CharacterAbility
{
    public UpdateAbility(float damage)
    {
        this.name = AbilityName.BASIC_ATTACK;
        this.damage = damage;
        this.range = Character.MELEE_RANGE;
        this.manaCost = 0f;
        this.animationNameList = new List<string> { "N/A" };
        this.statusEffects = new List<StatusEffect> { };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
