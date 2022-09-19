using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusedAim : CharacterAbility
{
    public FocusedAim()
    {
        this.name = AbilityName.FOCUSED_AIM;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HitChanceOnTarget, StatusEffectTarget.AllFriendlies, 0.20f, 10)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
