using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcyTouch : CharacterAbility
{
    public IcyTouch()
    {
        this.name = AbilityName.ICY_TOUCH;
        this.damage = 75;
        this.range = Character.MELEE_RANGE + 5f;
        this.manaCost = 75f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.AttackSpeed, StatusEffectTarget.SelectedTarget, -0.15f, 10)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0; // no special effects or damage modifiers
    }
}
