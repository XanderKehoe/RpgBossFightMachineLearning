using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueStrike : CharacterAbility
{
    public PlagueStrike()
    {
        this.name = AbilityName.PLAGUE_STRIKE;
        this.damage = 75;
        this.range = Character.MELEE_RANGE;
        this.manaCost = 75f;
        this.animationNameList = new List<string> { "MeleeAttack1", "MeleeAttack2" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackPower, StatusEffectTarget.SelectedTarget, -0.20f, 5)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0; // no special effects or damage modifiers
    }
}
