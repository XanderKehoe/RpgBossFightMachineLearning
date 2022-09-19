using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostbolt : CharacterAbility
{
    public Frostbolt()
    {
        this.name = AbilityName.FROSTBOLT;
        this.damage = 75f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 70f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.SelectedTarget, -0.2f, 15),
            new StatusEffect(StatusEffectType.MovementSpeedMultiplier, StatusEffectTarget.SelectedTarget, -0.25f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
