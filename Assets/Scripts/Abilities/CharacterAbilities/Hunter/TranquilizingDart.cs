using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranquilizingDart : CharacterAbility
{
    public TranquilizingDart()
    {
        this.name = AbilityName.TRANQUILIZING_DART;
        this.damage = 0f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "ShootArrow" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.SelectedTarget, -0.25f, 15),
            new StatusEffect(StatusEffectType.MovementSpeedMultiplier, StatusEffectTarget.SelectedTarget, -0.25f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
