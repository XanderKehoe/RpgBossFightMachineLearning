using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvengersShield : CharacterAbility
{
    public AvengersShield()
    {
        this.name = AbilityName.AVENGERS_SHIELD;
        this.damage = 50f;
        this.range = Character.MELEE_RANGE + 15f;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnSelf, StatusEffectTarget.SelectedTarget, 0.05f, 5)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
