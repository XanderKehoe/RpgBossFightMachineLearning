using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgement : CharacterAbility
{
    public Judgement()
    {
        this.name = AbilityName.JUDGEMENT;
        this.damage = 150f;
        this.range = Character.MELEE_RANGE + 10f;
        this.manaCost = 30f;
        this.animationNameList = new List<string> { "CastSpell" };
        this.statusEffects = new List<StatusEffect> { };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        return 0;
    }
}
