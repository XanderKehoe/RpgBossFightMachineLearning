using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.IO;

public class MlAgentsController : Agent
{
    public GameObject simulation;
    public SimulationManager simulationManager;

    private List<int> selectedAbilityIndexes;
    private List<Vector3> selectedTargetPositions;

    [SerializeField] public List<Character> characters = new();

    [SerializeField] public Dragon dragon;

    private bool firstEpisode = true;

    // Agent Rewards
    //public static readonly float REWARD_NOT_IN_FLAME_BREATH_ANGLE = 0.005f;
    public static readonly float REWARD_CAST_ABILITY_WITHIN_RANGE = 0.0005f; // casted an ability and was within range
    public static readonly float REWARD_DRAGON_DAMAGE = 10f;
    public static readonly float REWARD_KILL_BOSS = 20f;
    public static readonly float REWARD_CHARACTER_DAMAGE = -0.05f;
    public static readonly float REWARD_CHARACTER_DEATH = -10f;
    //public static readonly float REWARD_TEAM_WIPE = -5f;

    private double lastDecisionTime = 0;
    private const int INVALID_CHARACTER_INDEX = -1;

    private Dictionary<string, float> totalPosRewards = new Dictionary<string, float>(); // used for debugging to track how rewards are given.
    private Dictionary<string, float> totalNegRewards = new Dictionary<string, float>(); // used for debugging to track how rewards are given.

    public void Start()
    {
        if (simulation == null)
        {
            Debug.LogError("MlAgentsController::Start() - simulation is NULL");
        }

        if (simulationManager == null)
        {
            Debug.LogError("MlAgentsController::Start() - simulationManager is NULL");
        }

        if (characters.Count != 5)
        {
            Debug.LogError("(" + simulation.name + ") " + "MlAgentsController::Start() - characters.count != 5");
        }

        selectedAbilityIndexes = new List<int>();
        for (int i = 0; i < characters.Count; i++) // for some reason, using the initial capacity in the constructor isn't working as desired
            selectedAbilityIndexes.Add(0); // so needing to do this instead

        selectedTargetPositions = new List<Vector3>();
        for (int i = 0; i < characters.Count; i++) // for some reason, using the initial capacity in the constructor isn't working as desired
            selectedTargetPositions.Add(new()); // so needing to do this instead

        if (selectedAbilityIndexes.Count != characters.Count)
        {
            Debug.LogError("(" + simulation.name + ") " + "MlAgentsController::Start() - selectedAbilityIndex.Count != characters.Count [" + selectedAbilityIndexes.Count+" != "+characters.Count+"]");
        }

        if (dragon == null)
        {
            Debug.LogError("(" + simulation.name + ") " + "MlAgentsController::Start() - dragon is NULL");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions.Length == selectedAbilityIndexes.Count)
        {
            for (int i = 0; i < actions.DiscreteActions.Length; i++)
                selectedAbilityIndexes[i] = actions.DiscreteActions[i];
        }
        else
            Debug.LogError("MlAgentsController::OnActionReceived - actions.DiscreteActions.Length != selectedAbilityIndex.Count");

        if (actions.ContinuousActions.Length == selectedTargetPositions.Count * 2)
        {
            // (0, characters.count - 1) contains target x coords, (characters.count, characters.count * 2) contains target z coords
            for (int i = 0; i < characters.Count; i++)
            {
                Vector2 this2dTargetPosition = new();
                this2dTargetPosition.x = actions.ContinuousActions[i];
                this2dTargetPosition.y = actions.ContinuousActions[characters.Count + i];

                selectedTargetPositions[i] = simulationManager.ConvertNormalizedLocalPositionToWorldPosition(this2dTargetPosition);
            }
        }
        else
            Debug.LogError("MlAgentsController::OnActionReceived - actions.DiscreteActions.Length != selectedAbilityIndex.Count");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        string outputTextToFile = "";
        bool doOutputTextToFile = false;

        void AddToOutputTextToFile(string name, float value)
        {
            if (doOutputTextToFile)
                outputTextToFile += name + ":" + value + ",";
        }

        void AddEntityInfoToObservation(Entity entity)
        {
            Dictionary<string, float> currentHealthManaRatios = entity.stats.GetHealthManaRatios();
            Dictionary<StatusEffectType, float> currentStatusEffects = entity.GetAllTotalStatusEffectModifiers();

            //if (doOutputTextToFile)
                //outputTextToFile += entity.name + ": ";

            //Debug.Log(name + ": Expected vector size: " + (currentHealthManaRatios.Count + currentStatusEffects.Count)); // this isn't even right, count*3

            // 2 observations 
            foreach (KeyValuePair<string, float> entry in currentHealthManaRatios)
            {
                int NUMBER_OF_BUCKETS = 10;

                int bucketedObservationValue = DataPreprocessor.BucketData(entry.Value, 0, 1, NUMBER_OF_BUCKETS);

                sensor.AddOneHotObservation(bucketedObservationValue, NUMBER_OF_BUCKETS + 1);
                AddToOutputTextToFile(entry.Key, bucketedObservationValue);
            }

            // 13 observations
            foreach (KeyValuePair<StatusEffectType, float> entry in currentStatusEffects)
            {
                // skip any unused status effect types
                if (!DataPreprocessor.IsStatusEffectTypeProcessed(entry.Key, characters))
                {
                    continue;
                }

                float observationValue = entry.Value;

                switch (entry.Key)
                {
                    case StatusEffectType.DamageAbsorption:
                        observationValue = entry.Value / Paladin.HAND_OF_PROTECTION_DAMAGE_ABSORPTION;
                        break;

                    case StatusEffectType.ThreatGeneration:
                        // this will need to be updated in the future if any more threat generation values appear
                        observationValue = entry.Value / (Paladin.HAND_OF_RECKONING_THREAT_GEN + Paladin.HAMMER_OF_THE_RIGHTEOUS_THREAT_GEN);
                        break;

                    case StatusEffectType.HealthPerSecond:
                        observationValue = entry.Value / characters[0].stats.maxHealth; // use paladin's max health since he has the most
                        break;

                    case StatusEffectType.ManaPerSecond:
                        observationValue = entry.Value / characters[3].stats.maxMana; // use mage's max mana since he has the most
                        break;
                }

                float min = DataPreprocessor.GetMinStatusEffectValue(entry.Key, characters);
                float max = DataPreprocessor.GetMaxStatusEffectValue(entry.Key, characters);

                float normalizedObservationValue = DataPreprocessor.NormalizeData(observationValue, min, max);

                sensor.AddObservation(normalizedObservationValue);
                AddToOutputTextToFile(entry.Key.ToString(), normalizedObservationValue);
            }

            float normalizedXPos;
            float normalizedZPos;
            if (entity is Dragon) 
            {
                normalizedXPos = entity.transform.localPosition.x / simulationManager.terrain.terrainData.size.x;
                normalizedZPos = entity.transform.localPosition.z / simulationManager.terrain.terrainData.size.z;
            }
            else 
            { 
                // get normalized position relative to dragon
                normalizedXPos = (dragon.transform.position.x - entity.transform.localPosition.x) / simulationManager.terrain.terrainData.size.x;
                normalizedZPos = (dragon.transform.position.z - entity.transform.localPosition.z) / simulationManager.terrain.terrainData.size.z;

                // get normalized rotation relative to dragon
                float normalizedRelativeRotion = MathHelper.GetAngleBetweenTwoVectors(dragon.transform.forward, entity.transform.position - dragon.transform.position) / 180f;
                if (float.IsNaN(normalizedRelativeRotion))
                    Debug.Log("normalizedRelativeRotion is NaN - " + dragon.transform.forward.ToString() + " | " + (entity.transform.position - dragon.transform.position).ToString());
                sensor.AddObservation(normalizedRelativeRotion);
                AddToOutputTextToFile("normalizedRelativeRotation", normalizedXPos);
            }

            sensor.AddObservation(normalizedXPos);
            sensor.AddObservation(normalizedZPos);
            AddToOutputTextToFile("normalizedXPos", normalizedXPos);
            AddToOutputTextToFile("normalizedZPos", normalizedZPos);

            if (doOutputTextToFile)
                outputTextToFile += "\n";
        }

        foreach (Character character in characters)
        {
            AddEntityInfoToObservation(character);
            sensor.AddOneHotObservation(character.isDead ? 1 : 0, 1);
            sensor.AddOneHotObservation(dragon.currentTarget == character ? 1 : 0, 1); // is the dragon currently targeting this character?
        }

        // insert inputs for dragon
        AddEntityInfoToObservation(dragon);
        sensor.AddObservation(dragon.transform.localRotation.eulerAngles / 360f);
        sensor.AddObservation(dragon.navMeshAgent.velocity.x / dragon.navMeshAgent.speed);
        sensor.AddObservation(dragon.navMeshAgent.velocity.z / dragon.navMeshAgent.speed);

        // write outputs
        if (doOutputTextToFile)
        {
            StreamWriter writer = new("C:\\Users\\Xander\\Documents\\Coding\\UnityProjects\\WowBossFightML\\DataTextOutput\\output.txt", true);
            writer.WriteLine(outputTextToFile);
            writer.Close();
        }
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("(" + simulation.name + ") " + "MlAgentsController::OnEpisodeBegin() - New Episode!");

        if (!firstEpisode)
        {

            foreach (Character character in characters)
            {
                character.transform.position = character.startPos;
                character.transform.rotation = character.startRot;
                character.ResetStatsAndStatusEffects();
                character.animator.CrossFade("Locomotion", 0.05f);

                character.currentTarget = dragon;

                if (dragon.threatMap.ContainsKey(character))
                    dragon.threatMap[character] = 0;
                else
                    dragon.threatMap.Add(character, 0);
            }

            dragon.transform.position = dragon.startPos;
            dragon.transform.rotation = dragon.startRot;
            dragon.ResetStatsAndStatusEffects();
            dragon.animator.CrossFade("Locomotion", 0.05f);

            dragon.currentTarget = characters[0];

            float totalPosReward = 0;
            foreach(KeyValuePair<string, float> kvp in totalPosRewards)
            {
                totalPosReward += kvp.Value;
            }

            float totalNegReward = 0;
            foreach (KeyValuePair<string, float> kvp in totalNegRewards)
            {
                totalNegReward += kvp.Value;
            }

            /* // used for debugging to track how rewards are given.
            Debug.Log("END OF EPISODE, PRINTING REWARDS...");
            foreach (KeyValuePair<string, float> kvp in totalPosRewards)
            {
                Debug.Log("\t" + kvp.Key + "(+): " + kvp.Value + " ("+((kvp.Value / totalPosReward) * 100)+"%)");
            }

            foreach (KeyValuePair<string, float> kvp in totalNegRewards)
            {
                Debug.Log("\t" + kvp.Key + "(-): " + kvp.Value + " (" + ((kvp.Value / totalNegReward) * 100)+"%)");
            }*/

            totalPosRewards.Clear();
            totalNegRewards.Clear();
        }
        else
            firstEpisode = false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // doing this to get rid of warnings
    }

    // makes it that redundant calls to make decision aren't made
    public void HandleRequestDecisionWithTime()
    {
        const float REFRESH_TIME = 0.01f;

        if (Time.timeAsDouble > lastDecisionTime + REFRESH_TIME)
        {
            lastDecisionTime = Time.time;
            this.RequestDecision();
            //this.RequestAction(); // accordinging to docs, this isn't necessary as RequestDecision already called this.
        }
    }

    private int GetCharacterIndex(Character character)
    {
        int retVal = INVALID_CHARACTER_INDEX;

        for (int i = 0; i < characters.Count; i++)
        {
            if (character.name == characters[i].name)
            {
                retVal = i;
                break;
            }
        }

        if (retVal == INVALID_CHARACTER_INDEX)
            Debug.LogError(name + ": GetCharacterIndex() - Did not find character's corresponding index");

        return retVal;
    }

    public int GetNextAbiliyIndex(Character character)
    {
        HandleRequestDecisionWithTime();

        // characters.findIndex() doesn't appear to work as desired, using this instead.
        int characterIndex = GetCharacterIndex(character);
       
        if (characterIndex == INVALID_CHARACTER_INDEX)
        {
            Debug.LogError("MlAgentsController::GetNextAbiliyIndex() - Did not find matching character in list for: " + character.name);
            return INVALID_CHARACTER_INDEX;
        }

        /*Debug.Log("MlAgentsController::GetNextAbilityIndex() - " +
            "character name/index [" + character.name + "][" + characterIndex + "]. Returning selectedAbilityIndex: "+selectedAbilityIndexes[characterIndex]);*/

        return selectedAbilityIndexes[characterIndex];
    }

    public Vector3 GetNewTargetPosition(Character character)
    {
        HandleRequestDecisionWithTime();

        // characters.findIndex() doesn't appear to work as desired, using this instead.
        int characterIndex = GetCharacterIndex(character);

        if (characterIndex == INVALID_CHARACTER_INDEX)
        {
            Debug.LogError("MlAgentsController::GetNewTargetPosition() - Did not find matching character in list for: " + character.name);
            return new();
        }

        return selectedTargetPositions[characterIndex];
    }

    public void GiveRewardWithTracking(float val, string str)
    {
        AddReward(val);

        // used for debugging to track how rewards are given.
        if (val >= 0)
        {
            if (totalPosRewards.ContainsKey(str))
            {
                totalPosRewards[str] += val;
            }
            else
            {
                //Debug.Log("Adding new positive reward: " + str);
                totalPosRewards.Add(str, val);
            }
        }
        else
        {
            if (totalNegRewards.ContainsKey(str))
            {
                totalNegRewards[str] += val * -1;
            }
            else
            {
                //Debug.Log("Adding new positive reward: " + str);
                totalNegRewards.Add(str, val * -1);
            }
        }
    }
}
