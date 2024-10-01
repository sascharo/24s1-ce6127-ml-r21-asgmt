using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgentOptimized : Agent
{
    public Transform Target;
    public float ForceMultiplier = 10f;
    public int seed = 20241007;

    private Rigidbody rBody;
    private System.Random systemRandom;

    private const float ResetYPosition = 0.5f;
    private const float TargetThreshold = 1.42f;
    private const float EpisodeTimeoutReward = -0.001f; // Small negative reward per step

    protected override void Awake()
    {
        base.Awake();

        systemRandom = new System.Random(seed);

        if (Target == null)
        {
            Debug.LogError("Target reference is null! Please assign a Target in the inspector.");
        }
    }

    protected void Start()
    {
        rBody = GetComponent<Rigidbody>();

        InitializeRandomState();
    }

    private void InitializeRandomState()
    {
        // Initialize Unity's Random with the fixed seed
        Random.InitState(seed);
        // If using other random generators, ensure they're also seeded
        // For example, if using System.Random elsewhere:
        // systemRandom = new System.Random(seed);
    }

    public override void OnEpisodeBegin()
    {
        InitializeRandomState();

        ResetAgentIfFallen();
        MoveTargetToRandomPosition();
    }

    private void ResetAgentIfFallen()
    {
        // Reset agent's physics if fallen
        if (transform.position.y < 0f)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, ResetYPosition, 0f);
        }
    }

    private void MoveTargetToRandomPosition()
    {
        // Move the target to a new position using systemRandom
        float xPos = (float)(systemRandom.NextDouble() * 8f - 4f);
        float zPos = (float)(systemRandom.NextDouble() * 8f - 4f);
        Target.localPosition = new Vector3(xPos, ResetYPosition, zPos);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add target and agent positions (each as Vector3)
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Add agent velocity (only x and z axes)
        sensor.AddObservation(new Vector2(rBody.velocity.x, rBody.velocity.z));
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = new Vector3(actionBuffers.ContinuousActions[0], 0f, actionBuffers.ContinuousActions[1]);
        rBody.AddForce(controlSignal * ForceMultiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);

        // Reward shaping
        if (distanceToTarget < TargetThreshold)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (transform.localPosition.y < 0f)
        {
            EndEpisode();
        }
        else
        {
            // Optional: Small negative reward to encourage efficiency
            AddReward(EpisodeTimeoutReward);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
