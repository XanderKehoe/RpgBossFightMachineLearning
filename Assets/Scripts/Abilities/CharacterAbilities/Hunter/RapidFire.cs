using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : CharacterAbility
{
    public RapidFire()
    {
        this.name = AbilityName.RAPID_FIRE;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.Self, 0.75f, 8)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
