﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class CarAgent : Agent
{
    public CarAcademy academy;
    public CarSceneManager sceneManager;
    private Rigidbody rb;
    private BoxCollider bc;
    public float forceMultiplier;
    public GameObject[] sensorObjects;
    public LayerMask goalLayer;
    private LayerMask maskLayers;
    private bool[] parkingSensors;
    private int customMaxStepCount;
    private int customCurrentStepCount;

    private void Start()
    {
        customMaxStepCount = 100;
        customCurrentStepCount = 0;

        parkingSensors = new bool[4];

        academy = FindObjectOfType<CarAcademy>();
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();

        maskLayers = ~(1 << goalLayer);
    }

    public override void AgentAction(float[] vectorAction)
    {
        if (InsideGoal())
        {
            AddReward(1.0f);
            Done();
        }

        if (transform.position.y < -5.0f)
        {
            AddReward(-0.5f);
            Done();
        }

        if (customCurrentStepCount > customMaxStepCount)
        {
            Done();
        }

        customCurrentStepCount++;

        var moveAction = Mathf.FloorToInt(vectorAction[0]);
        var rotateAction = Mathf.FloorToInt(vectorAction[1]);

        switch(moveAction)
        {
            case 1:
                rb.AddForce(transform.forward * forceMultiplier, ForceMode.Force);
                break;
            case 2:
                rb.AddForce(transform.forward * forceMultiplier * 2.0f, ForceMode.Force);
                break;
            case 3:
                rb.AddForce(transform.forward * -forceMultiplier, ForceMode.Force);
                break;
            case 4:
                rb.AddForce(transform.forward * -forceMultiplier * 1.6f, ForceMode.Force);
                break;
        }

        float velocity = Mathf.Clamp01(rb.velocity.magnitude);

        if (velocity > 0.15f)
        {
            velocity *= 0.5f;

            switch(rotateAction)
            {   
                case 1:
                    rb.AddTorque(-transform.up * velocity, ForceMode.VelocityChange);
                    break;
                case 2:
                    rb.AddTorque(transform.up * velocity, ForceMode.VelocityChange);
                    break;
            }
        }
    }

    public override float[] Heuristic()
    {
        float[] action = new float[2];

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            action[0] = 3.0f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            action[0] = 2.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            action[0] = 4.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2.0f;
        }

        return action;
    }

    public override void CollectObservations()
    {
        for (int i = 0; i < 4; i++)
        {
            AddVectorObs(parkingSensors[i]);
        }
    }

    public override void AgentReset()
    {
        customCurrentStepCount = 0;

        rb.velocity = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.Rotate(0.0f, 90.0f, 0.0f);
        transform.localPosition = new Vector3 (0.0f, 0.0f, -5.0f);

        int rows = Random.Range((int)academy.resetParameters["rMin"], ((int)academy.resetParameters["rMax"] + 1));
        int cols = Random.Range((int)academy.resetParameters["cMin"], ((int)academy.resetParameters["cMax"] + 1));
        int groups = Random.Range((int)academy.resetParameters["gMin"], ((int)academy.resetParameters["gMax"] + 1));
        float carChance = academy.resetParameters["carChance"];

        customMaxStepCount = (int)academy.resetParameters["steps"];

        if ((int)academy.resetParameters["bigSpawn"] == 1)
        {
            sceneManager.bigSpawn = true;
        }
        else
        {
            sceneManager.bigSpawn = false;
        }

        sceneManager.rows = rows;
        sceneManager.cols = cols;
        sceneManager.groups = groups;
        sceneManager.spawnChance = carChance;

        rb.isKinematic = true;

        StopAllCoroutines();
        StartCoroutine(sceneManager.SpawnCars());
    }

    public bool InsideGoal()
    {
        bool inGoal = false;

        for (int i = 0; i < sensorObjects.Length; i++)
        {
            if (Physics.Raycast(sensorObjects[i].transform.position, sensorObjects[i].transform.TransformDirection(Vector3.down), 1.5f, maskLayers))
            {   
                Debug.DrawRay(sensorObjects[i].transform.position, sensorObjects[i].transform.TransformDirection(Vector3.down) * 1.5f, Color.magenta);
                parkingSensors[i] = true;
                AddReward(0.00003f);
            }
            else
            {
                Debug.DrawRay(sensorObjects[i].transform.position, sensorObjects[i].transform.TransformDirection(Vector3.down) * 1.5f, Color.cyan);
                parkingSensors[i] = false;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (parkingSensors[i] == false)
            {
                inGoal = false;
                break;
            }

            inGoal = true;
        }

        return inGoal;
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("car"))
        {
            AddReward(-0.1f);
        }
    }

    private void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("car"))
        {
            AddReward(-0.01f);
        }
    }

}
