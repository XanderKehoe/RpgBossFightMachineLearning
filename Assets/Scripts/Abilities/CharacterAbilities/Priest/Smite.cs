using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : CharacterAbility
{
    public Smite()
    {
        this.name = AbilityName.SMITE;
        this.damage = 75f;
        this.range = Character.DEFAULT_RANGED_ATTACK_RANGE;
        this.manaCost = 75f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnSelf, StatusEffectTarget.SelectedTarget, -0.1f, 15)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
