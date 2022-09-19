using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Character
{
    public static float HAND_OF_PROTECTION_DAMAGE_ABSORPTION = 1000f;
    public static float HAMMER_OF_THE_RIGHTEOUS_THREAT_GEN = 6f;
    public static float HAND_OF_RECKONING_THREAT_GEN = 10f;

    public override void Start()
    {
        base.Start();

        abilities.Add(new CharacterBasicAttack(35, 0, new List<string> { "MeleeAttack1", "MeleeAttack2" }));
        abilities.Add(new AvengersShield());
        abilities.Add(new DivineShield());
        abilities.Add(new HammerOfTheRighteous());
        abilities.Add(new HandOfProtection());
        abilities.Add(new HandOfReckoning());
        abilities.Add(new Judgement());
    }
}
