using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorch : CharacterAbility
{
    public Scorch()
    {
        this.name = AbilityName.SCORCH;
        this.damage = 50f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 50f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -5, 5),
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.SelectedTarget, 0.1f, 2)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
