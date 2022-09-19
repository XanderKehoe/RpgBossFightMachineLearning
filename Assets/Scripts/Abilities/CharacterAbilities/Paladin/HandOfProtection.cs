using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandOfProtection : CharacterAbility
{
    // Start is called before the first frame update
    public HandOfProtection()
    {
        this.name = AbilityName.HAND_OF_PROTECTION;
        this.damage = 0f;
        this.range = 0f;
        this.manaCost = 150f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.DamageAbsorption, StatusEffectTarget.Self, Paladin.HAND_OF_PROTECTION_DAMAGE_ABSORPTION, 12)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
