using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class BirdAgent : Agent
{
    // Components
    public Rigidbody2D myRigidbody;
    public Animator myAnimator;
    public LogicScript logic;

    // Game parameters
    public float fly_strength = 5f;
    public float topBoundary = 5f;
    public float bottomBoundary = -5f;

    // State
    public bool birdIsAlive = true;
    public float distanceToNextPipe;  // Horizontal distance to the next pipe
    public float heightOfNextPipe;    // Height of the next pipe (top or bottom)

    // Pipe-related
    public List<GameObject> activePipes = new List<GameObject>();  // List to store active pipes

    private GameObject currentPipe;  // The current pipe being observed

    private ObjectPool pipePool;  // Object pool

    public override void Initialize()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.freezeRotation = true;
        myAnimator = GetComponent<Animator>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        pipePool = GameObject.FindObjectOfType<ObjectPool>();  // Get reference to the object pool
    }

    public override void OnEpisodeBegin()
    {
        // Reset the Bird's state and position
        birdIsAlive = true;
        myRigidbody.linearVelocity = Vector2.zero;  // Clear previous velocity
        transform.position = new Vector3(0f, 0f, 0f);  // Reset bird's position
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // Keep it unrotated
        
        // Rebind animations
        myAnimator.Rebind();

        // Disable all current pipes
        foreach (var pipe in activePipes)
        {
            pipe.SetActive(false);  // Disable pipe object
            pipePool.ReturnObjectToPool(pipe);  // Return pipe to object pool
        }
        activePipes.Clear();  // Clear pipe list

        // Restart pipe generator
        PipeGenerate pipeGenerate = GameObject.FindObjectOfType<PipeGenerate>();
        pipeGenerate.ResetGenerator();

        logic.ResetScore();  // Reset score
        
        currentPipe = null;  // Reset the current target pipe
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!birdIsAlive) return;

        // Add Bird's position and velocity as observations
        sensor.AddObservation(transform.position.y);  // Bird's position
        sensor.AddObservation(myRigidbody.linearVelocity.y); // Bird's velocity

        // Get information about the nearest pipe
        if (activePipes.Count > 0)
        {
            // Select the nearest pipe in view
            GameObject closestPipe = GetNextPipeInView();

            if (closestPipe != null)
            {
                // Update the current pipe
                currentPipe = closestPipe;

                // Get the pipe's x and y position
                float pipeX = closestPipe.transform.position.x;
                float pipeY = closestPipe.transform.position.y;

                // Calculate the horizontal distance and pipe height
                distanceToNextPipe = pipeX - transform.position.x;
                heightOfNextPipe = pipeY;

                // Add pipe information as observations
                sensor.AddObservation(distanceToNextPipe);  // Horizontal distance to the next pipe
                sensor.AddObservation(heightOfNextPipe);    // Height of the next pipe
            }
        }
    }

    // Select the next pipe in view
    private GameObject GetNextPipeInView()
    {
        GameObject nextPipe = null;

        foreach (var pipe in activePipes)
        {
            float distance = pipe.transform.position.x - transform.position.x;

            // Only consider pipes in the view and not passed yet
            if (distance > -2.5 && distance < 12)  // Ensure the pipe is in view
            {
                if (nextPipe == null || distance < nextPipe.transform.position.x - transform.position.x)
                {
                    nextPipe = pipe;  // Select the nearest pipe
                }
            }
        }

        return nextPipe;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!birdIsAlive) return;

        int action = actions.DiscreteActions[0];

        if (action == 1)  // Flap
        {
            myRigidbody.linearVelocity = Vector2.up * fly_strength;
            myAnimator.SetTrigger("WingTrigger");
            AddReward(-2.0f);  // Penalize for flapping
        }

        // Check if the pipe has passed
        if (currentPipe != null && -2 > currentPipe.transform.position.x && birdIsAlive)
        {
            AddReward(10f);  // Reward for passing the pipe
            currentPipe = null;  // Reset current pipe after passing
        }

        // Check survival reward
        if (birdIsAlive)
        {
            AddReward(0.5f);  // Reward for surviving each frame
        }

        // Check if bird hits the wall or floor
        if (transform.position.y >= topBoundary || transform.position.y <= bottomBoundary)
        {
            AddReward(-10f);  // Punish for hitting the wall or floor
            birdIsAlive = false;
            EndEpisode(); // End the episode when the bird dies
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!birdIsAlive) return;
        
        birdIsAlive = false;
        AddReward(-10f);  // Punish for hitting an obstacle
        EndEpisode(); // End the episode when the bird dies
    }
}
