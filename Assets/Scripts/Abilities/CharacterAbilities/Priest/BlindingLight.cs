using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindingLight : CharacterAbility
{
    public BlindingLight()
    {
        this.name = AbilityName.BLINDING_LIGHT;
        this.damage = 25f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 125f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.SelectedTarget, -0.25f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
