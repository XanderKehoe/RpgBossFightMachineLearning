using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Entity : MonoBehaviour
{
    public SimulationManager simulationManager;
    public GameObject simulation;
    public Animator animator;

    public MlAgentsController mlAgentsController;

    public Stats stats;

    public List<Ability> abilities { get; protected set; } = new();
    private List<string> lockedAnimations = new(); // this is for controlling animations to not be rapidly repeated.
    private const float LOCKED_ANIMATION_TIME = 8f;

    [SerializeField] public List<StatusEffect> currentStatusEffects = new(); // treat as public get private set, need to keep public to view in editor.

    public ushort teamNumber;
    protected List<Entity> teammates = new();

    public Entity currentTarget;

    public float meleeRangeRadius = 1f; // how close do hostiles have to be to melee this entity

    static readonly float PASSIVE_HEALTH_REGEN = 1.25f;
    static readonly float PASSIVE_MANA_REGEN = 10f;

    public bool isDead = false;

    public NavMeshAgent navMeshAgent;

    public Vector3 startPos;
    public Quaternion startRot;

    protected bool globalCooldownEnabled = false;
    private Coroutine globalCooldownCoroutine = null;

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

        if (simulationManager == null)
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
            teammates = simulationManager.GetTeamMembers((Character) this);

        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    public virtual void Update()
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
                        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 4f * Time.deltaTime);
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
                else if (this is PlainAI)
                {
                    if (Vector3.Distance(currentTarget.transform.position, navMeshAgent.destination) > 1f) // update destination if significant difference between current destination and target's position
                    {
                        //Debug.Log(name + ": setting new navmeshagent destination.");
                        navMeshAgent.SetDestination(currentTarget.transform.position
                            + (Vector3.Normalize(transform.position - currentTarget.transform.position) * currentTarget.meleeRangeRadius));
                    }
                }
            }
            else if (this is PlainAI)
            {
                // threat generation logic starts here
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
                    simulationManager.CheckForEndOfEpisode();

                    return;
                }
                //else
                    //Debug.Log(name + ": New target found - ["+currentTarget.name+"]");
            }

            if (!globalCooldownEnabled)
            {
                MakeCombatDecision();

                globalCooldownCoroutine = StartCoroutine(StartGlobalCooldownTimer(GetNextGlobalCooldownTimer()));
            }

            /*if (!navMeshAgent.isStopped)
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
            } */

            FaceTarget();
        }
    }

    // An update function called once per second.
    void UpdatePerSecond()
    {
        if (!isDead)
        {
            // handle stat regen and status effects
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
                CalculateAndTakeDamageFromAbility(this, new UpdateAbility(healthRegenAmount * -1), true);

            RestoreMana(manaRegenAmount);

            navMeshAgent.speed = stats.baseMoveSpeed * animationMoveSpeedModifier;

            // handle animation speed modification & other character specific updates.
            if (this is Character)
            {
                animator.SetFloat("attackSpeedModifier", Mathf.Pow(animationAttackSpeedModifier, 1.5f));
                animator.SetFloat("movementSpeedModifier", animationMoveSpeedModifier);

                Vector3 dragonForwardVector = mlAgentsController.dragon.transform.forward;
                Vector3 vectorToThis = Vector3.Normalize(transform.position - mlAgentsController.dragon.transform.position);

                /*
                // All characters but the paladin should not be in the dragon's flame breath's angle, this should help guide them to learn that.
                if (!(this is Paladin)) 
                {
                    if (!(MathHelper.IsAngleBetweenTwoVectorsLessThan(dragonForwardVector, vectorToThis, Dragon.FLAME_BREATH_ANGLE)))
                    {
                        mlAgentsController.AddReward(MlAgentsController.REWARD_NOT_IN_FLAME_BREATH_ANGLE);
                    }
                    else
                    {
                        mlAgentsController.AddReward(MlAgentsController.REWARD_NOT_IN_FLAME_BREATH_ANGLE * -1);
                    }
                }
                */
            }

            foreach (StatusEffect statusEffect in toRemoveList)
            {
                currentStatusEffects.Remove(statusEffect);
            }
        }
    }

    // returns true if global cooldown should be started
    protected abstract void MakeCombatDecision();

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

    public void ResetGlobalCooldownTimer()
    {
        if (globalCooldownCoroutine != null)
        {
            StopCoroutine(globalCooldownCoroutine);
        }
        globalCooldownEnabled = false;
    }

    IEnumerator StartGlobalCooldownTimer(float time)
    {
        globalCooldownEnabled = true;
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
        List<Entity> GetAreaOfEffectTargets(List<Entity> potentialTargets, AbilityName abilityName, float range = float.PositiveInfinity)
        {
            List<Entity> retList = new();

            foreach (Entity target in potentialTargets)
            {
                if (abilityName == AbilityName.FLAME_BREATH && Vector3.Distance(transform.position, target.transform.position) < range)
                {
                    Vector3 forwardVector = transform.forward;
                    Vector3 vectorToTarget = Vector3.Normalize(target.transform.position - transform.position);

                    if (MathHelper.IsAngleBetweenTwoVectorsLessThan(forwardVector, vectorToTarget, Dragon.FLAME_BREATH_ANGLE)) // less than x degree angle
                    {
                        retList.Add(target);
                    }
                }
            }

            return retList;
        }

        void ApplyStatusEffects(Ability ability, List<StatusEffect> clonedStatusEffects)
        {
            foreach (StatusEffect statusEffect in clonedStatusEffects)
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
                            if (ability.name != AbilityName.TRANSFER_MANA)
                                this.ApplyNewStatusEffect(statusEffect);

                            //Debug.Log(name + ": casting [" + statusEffect.type + "] to all teammates");

                            foreach (Entity teammate in teammates)
                            {
                                //Debug.Log(name + ": giving statusEffect of type [" + statusEffect.type + "] to my teammeate - " + teammate.name);
                                teammate.ApplyNewStatusEffect(statusEffect);
                            }

                            break;
                        }

                    case StatusEffectTarget.AllEnemies:
                        {
                            if (this is PlainAI)
                            {
                                PlainAI castedThis = (PlainAI)this;

                                foreach (KeyValuePair<Entity, float> entry in castedThis.threatMap)
                                {
                                    entry.Key.ApplyNewStatusEffect(statusEffect);
                                }
                            }
                            else
                            {
                                Debug.LogWarning(name + ": Casted [" + ability.name + "] which has an 'AllEnemies' target type, but this is unhandled for non-PlainAI types");
                            }

                            break;
                        }

                    case StatusEffectTarget.AreaOfEffect:
                        {
                            if (this is PlainAI)
                            {
                                PlainAI castedThis = (PlainAI)this;

                                List<Entity> potentialTargets = new();
                                foreach (KeyValuePair<Entity, float> entry in castedThis.threatMap)
                                {
                                    potentialTargets.Add(entry.Key);
                                }

                                List<Entity> targets = GetAreaOfEffectTargets(potentialTargets, ability.name, ability.range);
                                foreach (Entity AoeTarget in targets)
                                {
                                    AoeTarget.ApplyNewStatusEffect(statusEffect);
                                }
                            }

                            break;
                        }

                    default:
                        {
                            Debug.LogError("(" + simulation.name + ") " + name + ": CastAbility - unhandled StatusEffectTarget of: " + statusEffect.target.ToString());
                            break;
                        }
                }
            }
        }

        simulationManager.abilityUseTracker.AddAbilityUse(this, ability.name);

        // check mana cost and update it.
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

        target.CalculateAndTakeDamageFromAbility(this, ability);

        // apply status effects to proper targets
        List<StatusEffect> clonedStatusEffects = ability.GetStatusEffectListClone();
        ApplyStatusEffects(ability, clonedStatusEffects);

        // perform a corresponding animation
        string getAnimationName = ability.GetAnimationName();
        animator.CrossFade(getAnimationName, 0.1f);
    }

    // damagingEntity - the entity that is damaging this entity
    public void CalculateAndTakeDamageFromAbility(Entity damagingEntity, Ability ability, bool isUpdatePerSecond = false)
    {
        void UpdateThreatMap(float damageAmount)
        {
            // Plain AI uses threat system to determine target, this block handles how threat is calculated.
            PlainAI castedThis = (PlainAI)this;
            float threatModifier = 1f + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.ThreatGeneration);
            if (castedThis.threatMap.ContainsKey(damagingEntity))
            {
                castedThis.threatMap[damagingEntity] += (damageAmount * threatModifier);
            }
            else
            {
                castedThis.threatMap.Add(damagingEntity, damageAmount * threatModifier);
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

        void Die()
        {
            isDead = true;
            animator.CrossFade("Death", 0.15f);

            stats.health = 0;
            stats.mana = 0;

            // clear status effects
            while (currentStatusEffects.Count > 0)
                currentStatusEffects.RemoveAt(0);

            // apply reward (negative) to mlAgentsController
            //if (this is Character)
                //mlAgentsController.GiveRewardWithTracking(MlAgentsController.REWARD_CHARACTER_DEATH, "REWARD_CHARACTER_DEATH");
        }

        float DoCriticalStrikeCalculations(float passedAmount)
        {
            float retVal = passedAmount;

            float doCriticalStrikeProbability = damagingEntity.stats.baseCritChance
                + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.CriticalStrikeChanceOnTarget)
                - this.GetTotalStatusEffectModifier(StatusEffectType.CriticalStrikeChanceOnSelf);

            if (UnityEngine.Random.Range((float)0, (float)1) < doCriticalStrikeProbability)
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
                retVal *= 1.5f;
            }

            return retVal;
        }

        float DoBlockCalculations(float passedAmount)
        {
            float retVal = passedAmount;

            float doBlockProbability = this.stats.baseBlockChance + this.GetTotalStatusEffectModifier(StatusEffectType.BlockChance);
            float randChance = UnityEngine.Random.Range((float)0, (float)1);
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
                retVal *= 0.5f;
            }

            return retVal;
        }

        float GetDamageAfterAbsorption(float beforeAbsorptionDamage)
        {
            float retVal = beforeAbsorptionDamage;

            foreach (StatusEffect statusEffect in currentStatusEffects)
            {
                if (statusEffect.type == StatusEffectType.DamageAbsorption)
                {
                    if (statusEffect.strength > retVal)
                    {
                        // absorbing all the remaining damage and still some left to spare
                        statusEffect.strength -= retVal;
                        retVal = 0;
                        break;
                    }
                    else if (statusEffect.strength == retVal)
                    {
                        // absorbing all the remaining damage and none left to spare
                        statusEffect.strength = 0;
                        statusEffect.time = 0;
                        retVal = 0;
                        break;
                    }
                    else
                    {
                        // using all absorbtion from this effect, but not enough
                        // will continue to try other status effects (if any) to absorb remaining damage
                        statusEffect.strength = 0;
                        statusEffect.time = 0;
                        retVal -= statusEffect.strength;
                    }
                }
            }

            return retVal;
        }

        if (ability.damage == 0)
            return;

        if (ability.damage < 0)
            Debug.LogWarning(name + ": TakeDamage ["+ ability.name.ToString()+"] ["+isUpdatePerSecond+"] - negative value received and will heal target, if you want to heal, you should use HealDamage instead");

        // Perform hit calculations
        float getHitProbability = damagingEntity.stats.baseHitChance
            + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.HitChanceOnTarget)
            - this.GetTotalStatusEffectModifier(StatusEffectType.HitChanceOnSelf);

        if (UnityEngine.Random.Range((float) 0, (float) 1) >= getHitProbability && !isUpdatePerSecond)
        {
            return; // this ability missed.
        }

        if (!isUpdatePerSecond)
        {
            // Get (if any) attacking entity's attack power modifers
            float totalAttackPowerModifier = 1f + damagingEntity.GetTotalStatusEffectModifier(StatusEffectType.AttackPower);
            float modifiedDamageAmount = ability.damage * totalAttackPowerModifier;

            // Handle special cases for each ability
            modifiedDamageAmount += ability.HandleSpecialAbilitiesAndGetDamageModifier(damagingEntity, this);

            // Check for critical strike
            modifiedDamageAmount = DoCriticalStrikeCalculations(modifiedDamageAmount);

            // Check for block
            modifiedDamageAmount = DoBlockCalculations(modifiedDamageAmount);

            // Apply reward to character
            if (damagingEntity is Character && this is Dragon)
            {
                // positive reward for damaging dragon
                Character castedCharacter = (Character)damagingEntity;
                Dragon castedDragon = (Dragon)this;
                castedCharacter.mlAgentsController.GiveRewardWithTracking((modifiedDamageAmount / castedDragon.stats.maxHealth) * MlAgentsController.REWARD_DRAGON_DAMAGE, "REWARD_DRAGON_DAMAGE");
            }
            else if (this is Character)
            {
                // negative reward for receiving damage
                Character castedCharacter = (Character)this;
                castedCharacter.mlAgentsController.GiveRewardWithTracking((modifiedDamageAmount / castedCharacter.stats.maxHealth) * MlAgentsController.REWARD_CHARACTER_DAMAGE, "REWARD_CHARACTER_DAMAGE");
            }
            else if (damagingEntity.name != name) // exclude DOT status effects
                Debug.LogWarning(damagingEntity.name + " is attacking " + name + "???");

            // Check for damage absorbtion from status effects
            float damageAmountAfterAbsorption = GetDamageAfterAbsorption(modifiedDamageAmount);

            // Finally, adjust the entity's health
            stats.health -= damageAmountAfterAbsorption;
        }
        else
        {
            stats.health -= ability.damage;
        }

        if (stats.health < 0)
        {
            Die();
        }
        else if (this is PlainAI && damagingEntity != this && !isUpdatePerSecond)
        {
            UpdateThreatMap(ability.damage);
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
        if (isDead)
            return;

        bool alreadyExist = false;
        foreach(StatusEffect statusEffect in currentStatusEffects)
        {
            if (statusEffect.afflictingAbilityName == newStatusEffect.afflictingAbilityName &&
                statusEffect.type == newStatusEffect.type)
            {
                statusEffect.time = newStatusEffect.time;
                statusEffect.strength = newStatusEffect.strength;
                alreadyExist = true;
                break;
            }
        }

        if (!alreadyExist)
            currentStatusEffects.Add(newStatusEffect.Clone());
    }

    public virtual void ResetStatsAndStatusEffects()
    {
        while (currentStatusEffects.Count > 0)
            currentStatusEffects.RemoveAt(0);

        stats.health = stats.maxHealth;
        stats.mana = stats.maxMana;

        isDead = false;
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRangeRadius);
    }
}
