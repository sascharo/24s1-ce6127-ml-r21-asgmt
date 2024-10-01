using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    public Transform Target;
    public float ResetYPosition = 0.5f;
    public float TargetThreshold = 1.42f;
    public float forceMultiplier = 10f;

    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (transform.position.y < 0f)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, ResetYPosition, 0f);
        }

        // Move the target to a new spot
        float xPos = Random.Range(-4f, 4f);
        float zPos = Random.Range(-4f, 4f);
        Target.localPosition = new Vector3(xPos, ResetYPosition, zPos);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < TargetThreshold)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (transform.localPosition.y < 0f)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
