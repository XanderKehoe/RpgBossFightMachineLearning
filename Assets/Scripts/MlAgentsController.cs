using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MlAgentsController : Agent
{
    public GameObject simulation;

    private List<int> selectedAbilityIndexes;

    [SerializeField] public List<Character> characters = new();

    [SerializeField] public Dragon dragon;

    private bool firstEpisode = true;

    public static readonly float REWARD_DAMAGE = 10f;
    public static readonly float REWARD_KILL_BOSS = 20f;
    public static readonly float REWARD_CHARACTER_DEATH = -1f;
    public static readonly float REWARD_TEAM_WIPE = -5f;

    public void Start()
    {
        if (simulation == null)
        {
            Debug.LogError("MlAgentsController::Start() - simulation is NULL");
        }

        if (characters.Count != 5)
        {
            Debug.LogError("(" + simulation.name + ") " + "MlAgentsController::Start() - characters.count != 5");
        }

        selectedAbilityIndexes = new List<int>();
        for (int i = 0; i < characters.Count; i++) // for some reason, using the initial capacity in the constructor isn't working as desired
            selectedAbilityIndexes.Add(0); // so needing to do this instead

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
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        void AddEntityInfoToObservation(Entity entity)
        {
            Dictionary<string, float> currentHealthManaRatios = entity.stats.GetHealthManaRatios();
            Dictionary<StatusEffectType, float> currentStatusEffects = entity.GetAllTotalStatusEffectModifiers();

            //Debug.Log(name + ": Expected vector size: " + (currentHealthManaRatios.Count + currentStatusEffects.Count)); // this isn't even right, count*3

            foreach (KeyValuePair<string, float> entry in currentHealthManaRatios)
                sensor.AddObservation(entry.Value);

            foreach (KeyValuePair<StatusEffectType, float> entry in currentStatusEffects)
                sensor.AddObservation(entry.Value);
        }

        foreach (Character character in characters)
        {
            AddEntityInfoToObservation(character);
        }

        // insert inputs for dragon
        AddEntityInfoToObservation(dragon);
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("(" + simulation.name + ") " + "MlAgentsController::OnEpisodeBegin() - New Episode!");

        if (!firstEpisode)
        {

            foreach (Character character in characters)
            {
                character.transform.position = character.startPos;
                character.transform.rotation = character.startRot;
                character.ResetStatsAndStatusEffects();
                character.animator.CrossFade("Locomotion", 0.05f);

                character.currentTarget = dragon;
            }

            dragon.transform.position = dragon.startPos;
            dragon.transform.rotation = dragon.startRot;
            dragon.threatMap.Clear();
            dragon.ResetStatsAndStatusEffects();
            dragon.animator.CrossFade("Locomotion", 0.05f);

            dragon.currentTarget = characters[0]; 
        }
        else
            firstEpisode = false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // doing this to get rid of warnings
    }

    public int GetNextAbiliyIndex(Character character)
    {
        this.RequestDecision();
        this.RequestAction();

        // characters.findIndex() doesn't appear to work as desired, using this instead.
        int characterIndex = -1;
        for (int i = 0; i < characters.Count; i++)
        {
            if (character.name == characters[i].name)
            {
                characterIndex = i;
                break;
            }
        }

        if (characterIndex == -1)
        {
            Debug.LogError("MlAgentsController::GetNextAbiliyIndex() - Did not find matching character in list for: " + character.name);
            return -1;
        }

        /*Debug.Log("MlAgentsController::GetNextAbilityIndex() - " +
            "character name/index [" + character.name + "][" + characterIndex + "]. Returning selectedAbilityIndex: "+selectedAbilityIndexes[characterIndex]);*/

        return selectedAbilityIndexes[characterIndex];
    }
}
