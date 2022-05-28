using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject simulation;
    public Animator animator;

    public MlAgentsController mlAgentsController;

    public Stats stats;

    protected bool globalCooldownEnabled = false;

    protected List<Ability> abilities = new();
    private List<string> lockedAnimations = new(); // this is for controlling animations to not be rapidly repeated.
    private const float LOCKED_ANIMATION_TIME = 8f;

    [SerializeField] private List<StatusEffect> currentStatusEffects = new();

    public ushort teamNumber;
    protected List<Entity> teammates = new();

    public Entity currentTarget;

    public float meleeRangeRadius = 1f; // how close do hostiles have to be to melee this entity

    static readonly float PASSIVE_HEALTH_REGEN = 5f;
    static readonly float PASSIVE_MANA_REGEN = 10f;

    public bool isDead = false;

    public NavMeshAgent navMeshAgent;

    public Vector3 startPos;
    public Quaternion startRot;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (simulation == null)
        {
            Debug.LogError(name + ": no simulation is set");
            return;
        }

        if (stats.health <= 0)
        {
            Debug.LogError("(" + simulation.name + ") " + name + ": Initial health is less than or equal to 0");
        }

        if (teamNumber == 0)
        {
            Debug.LogError("(" + simulation.name + ") " + name + ": teamNumber is not set");
        }

        if (currentTarget == null)
        {
            Debug.LogError("(" + simulation.name + ") " + name + ": no initial target is set");
        }

        if (gameManager == null)
        {
            Debug.LogError("(" + simulation.name + ") " + name + ": no gameManager is set");
        }

        if (animator == null)
        {
            Debug.LogError("(" + simulation.name + ") " + name + ": no animator is set");
        }

        stats.maxHealth = stats.health;
        stats.maxMana = stats.mana;

        navMeshAgent.speed = stats.baseMoveSpeed;

        InvokeRepeating("UpdatePerSecond", 1, 1);

        if (this is Character)
            teammates = gameManager.GetTeamMembers((Character) this);

        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            void FaceTarget()
            {
                if (currentTarget != null)
                {
                    Vector3 facingDir = currentTarget.transform.position - transform.position;
                    facingDir.y = 0;

                    if (facingDir.x != 0 || facingDir.z != 0) // this is here to get rid of annoying warnings.
                    {
                        Quaternion rotation = Quaternion.LookRotation(facingDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f * Time.deltaTime);
                    }
                }
            }

            if (currentTarget != null)
            {
                if (currentTarget.isDead)
                {
                    if (this is PlainAI)
                    {
                        PlainAI castedThis = (PlainAI) this;
                        if (castedThis.threatMap.ContainsKey(currentTarget))
                            castedThis.threatMap.Remove(currentTarget);
                    }

                    currentTarget = null;
                    return;
                }
            }
            else if (this is PlainAI)
            {
                // switch target logic should go here
                PlainAI castedThis = (PlainAI) this;

                //Debug.Log(name + ": Searching for new target");

                Entity newTarget = null;
                float highestThreat = float.NegativeInfinity;
                foreach (KeyValuePair<Entity, float> entry in castedThis.threatMap)
                {
                    if (entry.Value > highestThreat && !entry.Key.isDead)
                    {
                        newTarget = entry.Key;
                        highestThreat = entry.Value;
                    }
                }

                currentTarget = newTarget;

                if (currentTarget == null)
                {
                    // still no new target (all characters must be dead?)
                    Debug.Log("("+simulation.name+")"+ name + ": no new target found, all characters are dead (RESET SCENE)");
                    gameManager.CheckForEndOfEpisode(true);

                    return;
                }
                //else
                    //Debug.Log(name + ": New target found - ["+currentTarget.name+"]");
            }

            if (!globalCooldownEnabled)
            {
                MakeCombatDecision();

                globalCooldownEnabled = true;

                StartCoroutine(StartGlobalCooldownTimer(GetNextGlobalCooldownTimer()));
            }

            if (!navMeshAgent.isStopped)
            {
                bool stop = false;
                if (this is Character)
                {
                    Character castedThis = (Character) this;
                    if (Vector3.Distance(this.transform.position, currentTarget.transform.position) < castedThis.nextAbility.range)
                        stop = true;
                }

                if (stop)
                    navMeshAgent.isStopped = true;
                else
                    navMeshAgent.SetDestination(currentTarget.transform.position);
            }

            FaceTarget();
        }
    }

    // An update function called once per second.
    void UpdatePerSecond()
    {
        if (!isDead)
        {
            float healthRegenAmount = PASSIVE_HEALTH_REGEN;
            float manaRegenAmount = PASSIVE_MANA_REGEN;
            float animationAttackSpeedModifier = 1f;
            float animationMoveSpeedModifier = 1f;

            List<StatusEffect> toRemoveList = new();

            foreach (StatusEffect statusEffect in currentStatusEffects)
            {
                if (statusEffect.time == 0)
                    toRemoveList.Add(statusEffect);
                else
                {
                    if (statusEffect.type == StatusEffectType.HealthPerSecond)
                        healthRegenAmount += statusEffect.strength;
                    else if (statusEffect.type == StatusEffectType.ManaPerSecond)
                        manaRegenAmount += statusEffect.strength;
                    else if (statusEffect.type == StatusEffectType.AttackSpeed)
                        animationAttackSpeedModifier += statusEffect.strength;
                    else if (statusEffect.type == StatusEffectType.MovementSpeedMultiplier)
                        animationMoveSpeedModifier += statusEffect.strength;

                    statusEffect.time--;
                }
            }

            if (healthRegenAmount >= 0)
                RestoreHealth(healthRegenAmount);
            else
                CalculateAndTakeDamageFromAbility(this, AbilityName.UPDATE_PER_SECOND, healthRegenAmount);
            RestoreMana(manaRegenAmount);

            navMeshAgent.speed = stats.baseMoveSpeed * animationMoveSpeedModifier;

            if (this is Character)
            {
                animator.SetFloat("attackSpeedModifier", Mathf.Pow(animationAttackSpeedModifier, 1.5f));
                animator.SetFloat("movementSpeedModifier", animationMoveSpeedModifier);
            }

            foreach (StatusEffect statusEffect in toRemoveList)
            {
                currentStatusEffects.Remove(statusEffect);
            }
        }
    }

    // returns true if global cooldown should be started
    protected virtual void MakeCombatDecision()
    {
        Debug.LogError("(" + simulation.name + ") " + name + ": MakeCombatDecision - Called from base function, this is supposed to be overwritten!");
    }

    float GetNextGlobalCooldownTimer()
    {
        const float BASE_TIME = 2f;
        const float CAP = 0.1f;

        float ret = BASE_TIME * (1 - GetTotalStatusEffectModifier(StatusEffectType.AttackSpeed));

        if (ret < CAP)
            ret = CAP;

        //Debug.Log(name + ": Next GCT: " + ret);

        return ret;
    }

    IEnumerator StartGlobalCooldownTimer(float time)
    {
        yield return new WaitForSeconds(time);

        globalCooldownEnabled = false;
    }

    IEnumerator StartUnlockAnimationTimer(float time, string animationName)
    {
        yield return new WaitForSeconds(time);

        //Debug.Log(name + ": StartUnlockAnimationTimer - [" + animationName + "] unlocked");
        lockedAnimations.Remove(animationName);
    }

    protected bool IsAbilityInRange(Entity target, float abilityRange)
    {
        return Vector3.Distance(this.transform.position, target.transform.position) <= abilityRange + target.meleeRangeRadius + this.meleeRangeRadius;
    }

    float GetTotalStatusEffectModifier(StatusEffectType type)
    {
        float ret = 0f;
        foreach(StatusEffect statusEffect in currentStatusEffects)
        {
            if (statusEffect.type == type)
            {
                ret += statusEffect.strength;
            }
        }

        return ret;
    }

    public Dictionary<StatusEffectType, float> GetAllTotalStatusEffectModifiers()
    {
        Dictionary<StatusEffectType, float> ret = new();
        foreach (StatusEffectType statusEffect in Enum.GetValues(typeof(StatusEffectType)))
        {
            ret.Add(statusEffect, GetTotalStatusEffectModifier(statusEffect));
        }

        return ret;
    }

    protected void CastAbility(Entity target, Ability ability)
    {
        if (ability is CharacterAbility)
        {
            CharacterAbility castedAbility = (CharacterAbility) ability;
            if (stats.mana < castedAbility.manaCost)
            {
                Debug.LogWarning("(" + simulation.name + ") " + name + ": CastAbility - tried to cast [" + ability.name + "] but didn't have enough mana");
                return; // not enough mana, don't cast ability
            }
            else
                stats.mana -= castedAbility.manaCost;
        }

        Ability clonedAbility = ability.Clone();

        target.CalculateAndTakeDamageFromAbility(this, clonedAbility.name, clonedAbility.damage);

        foreach (StatusEffect statusEffect in clonedAbility.statusEffects)
        {
            statusEffect.afflictingAbilityName = ability.name;

            switch (statusEffect.target)
            {
                case StatusEffectTarget.Self:
                    {
                        this.ApplyNewStatusEffect(statusEffect);
                        break;
                    }

                case StatusEffectTarget.SelectedTarget:
                    {
                        if (target != null)
                            target.ApplyNewStatusEffect(statusEffect);
                        else
                            Debug.LogError("(" + simulation.name + ") " + name + ": CastAbility - target in SelectedTarget case is NULL");
                        break;
                    }

                case StatusEffectTarget.AllFriendlies:
                    {
                        this.ApplyNewStatusEffect(statusEffect);

                        //Debug.Log(name + ": casting [" + statusEffect.type + "] to all teammates");

                        foreach (Entity teammate in teammates)
                        {
                            //Debug.Log(name + ": giving statusEffect of type [" + statusEffect.type + "] to my teammeate - " + teammate.name);
                            teammate.ApplyNewStatusEffect(statusEffect);
                        }

                        break;
                    }

                /*case StatusEffectTarget.AllEnemies:
                    {
                        Debug.LogError(name + ": CastAbility - unhandled StatusEffectTarget of: " + statusEffect.target.ToString());

                        break;
                    }*/

                default:
                    {
                        Debug.LogError("(" + simulation.name + ") " + name + ": CastAbility - unhandled StatusEffectTarget of: " + statusEffect.target.ToString());
                        break;
                    }
            }
        }

        if (this is Character)
        {
            switch (ability.animationType)
            {
                case Ability.AnimationType.MeleeAttack1:
                    animator.CrossFade("MeleeAttack1", 0.1f);
                    break;
                case Ability.AnimationType.MeleeAttack2:
                    animator.CrossFade("MeleeAttack2", 0.1f);
                    break;
                case Ability.AnimationType.CastSpell:
                    animator.CrossFade("CastSpell", 0.1f);
                    break;
                case Ability.AnimationType.CastHeal:
                    animator.CrossFade("CastHeal", 0.1f);
                    break;
                case Ability.AnimationType.ShootArrow:
                    animator.CrossFade("ShootArrow", 0.1f);
                    break;

                default:
                    Debug.LogWarning("(" + simulation.name + ") " + name + ": CastAbility - no matching case for AnimationType [" + ability.animationType.ToString() + "]");
                    break;
            }
        }
        else if (this is Dragon)
        {
            switch (ability.animationType)
            {
                case Ability.AnimationType.BasicAttack:
                    animator.CrossFade("Basic Attack", 0.1f);
                    break;

                default:
                    Debug.LogWarning("(" + simulation.name + ") " + name + ": CastAbility - no matching case for AnimationType [" + ability.animationType.ToString() + "]");
                    break;
            }
        }
    }

    // damageingEntity - the entity that is damaging this entity
    public void CalculateAndTakeDamageFromAbility(Entity damagingEntity, AbilityName abilityName, float amount)
    {
        if (amount == 0)
            return;

        //if (amount < 0)
            //Debug.LogWarning(name + ": TakeDamage - negative value received and will heal target, if you want to heal, you should use HealDamage instead");

        float getHitProbability = damagingEntity.stats.baseHitChance
            + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.HitChanceOnTarget)
            - this.GetTotalStatusEffectModifier(StatusEffectType.HitChanceOnSelf);
        if (UnityEngine.Random.Range((float) 0, (float) 1) >= getHitProbability)
        {
            // this ability missed.
            return;
        }

        // Check if attacking entity has any attack power modifers
        float totalAttackPowerModifier = 1f + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.AttackPower);

        // Handle special cases for each ability
        foreach (StatusEffect statusEffect in currentStatusEffects)
        {
            switch (abilityName) 
            {
                // Hunter
                case AbilityName.CHIMERA_SHOT:
                {
                    foreach (StatusEffect statusEffect2 in currentStatusEffects)
                    {
                        if (statusEffect2.afflictingAbilityName == AbilityName.SERPENT_STING)
                        {
                            statusEffect2.time = Hunter.SERPENT_STING_TIME;
                        }
                    }
                    break;
                }
                case AbilityName.KILL_SHOT:
                {
                    float hpRatio = stats.health / stats.maxHealth;
                    totalAttackPowerModifier += (1 - hpRatio);
                    break;
                }

                // Death Knight
                case AbilityName.SCOURGE_STRIKE:
                case AbilityName.DEATH_STRIKE:
                case AbilityName.OBLITERATE:
                {
                    uint diseaseCount = 0;
                    foreach (StatusEffect statusEffect2 in currentStatusEffects)
                    {
                        if (statusEffect2.afflictingAbilityName == AbilityName.ICY_TOUCH || statusEffect2.afflictingAbilityName == AbilityName.PLAGUE_STRIKE)
                        {
                            diseaseCount++;
                        }
                    }

                    switch (statusEffect.afflictingAbilityName) 
                    {
                        case AbilityName.SCOURGE_STRIKE:
                        {
                            totalAttackPowerModifier += 0.15f * diseaseCount;
                            break;
                        }
                        case AbilityName.DEATH_STRIKE:
                        {
                            damagingEntity.RestoreHealth((0.1f * damagingEntity.stats.maxHealth) + (0.05f + diseaseCount));
                            break;
                        }
                        case AbilityName.OBLITERATE:
                        {
                            totalAttackPowerModifier += 0.45f * diseaseCount;
                            foreach (StatusEffect statusEffect2 in currentStatusEffects)
                            {
                                if (statusEffect2.afflictingAbilityName == AbilityName.ICY_TOUCH || statusEffect2.afflictingAbilityName == AbilityName.PLAGUE_STRIKE)
                                {
                                    statusEffect2.time = 0;
                                }
                            }
                            break;
                        }
                    }

                    break;
                }
            }
        }

        amount *= totalAttackPowerModifier;

        // Check for critical strike
        float doCriticalStrikeProbability = damagingEntity.stats.baseCritChance
            + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.CriticalStrikeChanceOnTarget)
            - this.GetTotalStatusEffectModifier(StatusEffectType.CriticalStrikeChanceOnSelf);
        if (UnityEngine.Random.Range((float) 0, (float) 1) < doCriticalStrikeProbability)
        {
            // critical strike! do 50% more damage after other damage modifiers have been applied
            if (this is Dragon)
            {
                string getHitAnimationName = "Get Hit";
                if (!lockedAnimations.Contains(getHitAnimationName))
                {
                    animator.CrossFade(getHitAnimationName, 0.10f);
                    lockedAnimations.Add(getHitAnimationName);
                    //Debug.Log(name + ": Get Hit Animation Start");
                    StartCoroutine(StartUnlockAnimationTimer(LOCKED_ANIMATION_TIME, getHitAnimationName));
                }
            }
            amount *= 1.5f;
        }

        // Check for block
        float doBlockProbability = this.stats.baseBlockChance + this.GetTotalStatusEffectModifier(StatusEffectType.BlockChance);
        float randChance = UnityEngine.Random.Range((float) 0, (float) 1);
        if (randChance < doBlockProbability)
        {
            //Debug.Log(name + ": Blocking! ["+ randChance+"] < ["+doBlockProbability+"]");
            string blockAnimationName = "Block";
            
            if (this is Dragon && !lockedAnimations.Contains(blockAnimationName))
            {
                animator.CrossFade(blockAnimationName, 0.10f);
                lockedAnimations.Add(blockAnimationName);
                //Debug.Log(name + ": Block Animation Start");
                StartCoroutine(StartUnlockAnimationTimer(LOCKED_ANIMATION_TIME, blockAnimationName));
            }
            else if (!(this is Dragon))
                animator.CrossFade(blockAnimationName, 0.10f);

            // Ability was blocked, reduce damage by 50%
            amount *= 0.5f;
        }

        // Apply reward to character
        if (damagingEntity is Character && this is Dragon)
        {
            Character castedCharacter = (Character) damagingEntity;
            Dragon castedDragon = (Dragon) this;
            castedCharacter.abilityChooser.AddReward((amount / castedDragon.stats.maxHealth) * MlAgentsController.REWARD_DAMAGE);
        }

        // Check for damage absorbtion from status effects
        foreach (StatusEffect statusEffect in currentStatusEffects)
        {
            if (statusEffect.type == StatusEffectType.DamageAbsorption)
            {
                if (statusEffect.strength > amount)
                {
                    // absorbing all the remaining damage and still some left to spare
                    statusEffect.strength -= amount;
                    amount = 0;
                    break;
                }
                else if (statusEffect.strength == amount)
                {
                    // absorbing all the remaining damage and none left to spare
                    statusEffect.strength = 0;
                    statusEffect.time = 0;
                    amount = 0;
                    break;
                }
                else
                {
                    // using all absorbtion from this effect, but not enough
                    // will continue to try other status effects (if any) to absorb remaining damage
                    statusEffect.strength = 0;
                    statusEffect.time = 0;
                    amount -= statusEffect.strength;
                }
            }
        }

        stats.health -= amount;
        if (stats.health < 0)
        {
            // do death
            isDead = true;
            animator.CrossFade("Death", 0.15f);

            // clear status effects
            while (currentStatusEffects.Count > 0)
                currentStatusEffects.RemoveAt(0);

            if (this is Character)
                mlAgentsController.AddReward(MlAgentsController.REWARD_CHARACTER_DEATH);
        }
        else if (this is PlainAI && damagingEntity != this)
        {
            // Plain AI uses threat system to determine target, this block handles how threat is calculated.
            PlainAI castedThis = (PlainAI) this;
            float threatModifier = 1f + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.ThreatGeneration);
            if (castedThis.threatMap.ContainsKey(damagingEntity))
            {
                castedThis.threatMap[damagingEntity] += (amount * threatModifier);
            }
            else
            {
                castedThis.threatMap.Add(damagingEntity, amount * threatModifier);
            }

            if (currentTarget != null)
            {
                if (!castedThis.threatMap.ContainsKey(currentTarget))
                    castedThis.threatMap.Add(currentTarget, 0);

                Entity greatestThreat = currentTarget;
                float greatestThreatAmount = castedThis.threatMap[currentTarget];

                foreach (KeyValuePair<Entity, float> entry in castedThis.threatMap)
                {
                    if (entry.Value > greatestThreatAmount)
                    {
                        greatestThreat = entry.Key;
                        greatestThreatAmount = entry.Value;
                    }
                }

                if (greatestThreat != currentTarget)
                    currentTarget = greatestThreat;
            }
        }
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0)
            return;

        //if (amount < 0)
            //Debug.LogWarning("(" + simulation.name + ") " + name + ": TakeDamage - negative value received and will damage target, if you want to damage, you should use TakeDamage instead");


        stats.health += amount;
        if (stats.health > stats.maxHealth)
            stats.health = stats.maxHealth;
    }

    public void RestoreMana(float amount)
    {
        if (amount == 0)
            return;

        //if (amount < 0)
            //Debug.LogWarning(name + ": TakeDamage - negative value received and will damage target, if you want to damage, you should use TakeDamage instead");

        stats.mana += amount;
        if (stats.mana > stats.maxMana)
            stats.mana = stats.maxMana;
    }

    public void ApplyNewStatusEffect(StatusEffect newStatusEffect)
    {
        bool alreadyExist = false;
        foreach(StatusEffect statusEffect in currentStatusEffects)
        {
            if (statusEffect.afflictingAbilityName == newStatusEffect.afflictingAbilityName &&
                statusEffect.type == newStatusEffect.type)
            {
                statusEffect.time = newStatusEffect.time;
                alreadyExist = true;
                break;
            }
        }

        if (!alreadyExist)
            currentStatusEffects.Add(newStatusEffect.Clone());
    }

    public void ResetStatsAndStatusEffects()
    {
        while (currentStatusEffects.Count > 0)
            currentStatusEffects.RemoveAt(0);

        stats.health = stats.maxHealth;
        stats.mana = stats.maxMana;

        isDead = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRangeRadius);
    }
}
