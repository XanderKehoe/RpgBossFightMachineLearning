using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineShield : CharacterAbility
{
    public DivineShield()
    {
        this.name = AbilityName.DIVINE_SHIELD;
        this.damage = 0f;
        this.range = 0f;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.BlockChance, StatusEffectTarget.Self, 0.50f, 8)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
