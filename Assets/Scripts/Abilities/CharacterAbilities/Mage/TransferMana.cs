using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferMana : CharacterAbility
{
    public TransferMana(int numberOfTeamMates)
    {
        this.name = AbilityName.TRANSFER_MANA;
        this.damage = 0f;
        this.range = float.PositiveInfinity;
        this.manaCost = 100f;
        this.animationNameList = new List<string> { "CastHeal" };
        this.statusEffects = new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.ManaPerSecond, StatusEffectTarget.AllFriendlies, 66 / (numberOfTeamMates - 1), 3)
        };
    }

    public override float HandleSpecialAbilitiesAndGetDamageModifier(Entity castingEntity, Entity receivingEntity)
    {
        throw new System.NotImplementedException();
    }
}
