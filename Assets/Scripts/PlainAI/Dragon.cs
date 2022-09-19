using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : PlainAI
{
    public ParticleSystem breathPs;
    public static float FLAME_BREATH_ANGLE = 60f; // treat as constant, but not actually set as one to adjust in editor.
    public const float DEFAULT_MELEE_RANGE = 5f;

    public override void Start()
    {
        base.Start();

        abilities.Add(new PlainAiBasicAttack(75f, DEFAULT_MELEE_RANGE, new List<string> { "Basic Attack" }));
        /*abilities.Add(new PlainAiAbility(AbilityName.RAIN_OF_FIRE, 50f, 15f, 30f, Ability.AnimationType.BASIC_ATTACK, new List<StatusEffect>
        {
            new StatusEffect(StatusEffectType.HealthPerSecond, StatusEffectTarget.SelectedTarget, -10f, 5)
        }));*/
        abilities.Add(new FlameBreath());

        // set boss to initially have all cooldowns on max
        foreach (PlainAiAbility ability in abilities)
        {
            if (ability.name != AbilityName.BASIC_ATTACK)
                SetAbilityCooldown(ability.name, ability.cooldownTime);
        }

        if (breathPs == null)
        {
            Debug.LogError(name + ": BreathPs is NULL!");
        }
    }

    public override void Update()
    {
        base.Update();

        // toggle flame particles when flame breath animation is playing
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Flame Breath") && !breathPs.isPlaying)
        {
            breathPs.Play();
        }
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flame Breath") && breathPs.isPlaying)
        {
            breathPs.Stop();
        }
    }
}
