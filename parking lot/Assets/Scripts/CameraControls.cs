using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private List<GameObject> scenes;
    private int activeIndex;
    private CarAgent activeAgent;
    private bool followAgent;

    private void Start()
    {
        activeIndex = 0;
        scenes = new List<GameObject>();
        followAgent = false;

        foreach (GameObject scene in GameObject.FindGameObjectsWithTag("scene"))
        {
            scenes.Add(scene);
        }

        ChangeActiveScene();
    }

    private void Update()
    {
        if (followAgent)
        {
            transform.position = activeAgent.transform.position;
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right;
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0.0f, -5.0f, 0.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0.0f, 5.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeActiveScene();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFollowAgent();
        }
    }

    private void ChangeActiveScene()
    {
        activeIndex++;
        if (activeIndex >= scenes.Count)
        {
            activeIndex = 0;
        }
        transform.position = scenes[activeIndex].GetComponent<CarSceneManager>().parkingLotSurface.transform.position;
        activeAgent = scenes[activeIndex].GetComponent<CarSceneManager>().agent;
    }

    private void ToggleFollowAgent()
    {
        followAgent = !followAgent;
    }
}