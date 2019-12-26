using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSceneManager : MonoBehaviour
{
    public GameObject parkingLotSurface;
    public GameObject goal;
    public GameObject parkingSpotsHolder;
    public GameObject parkingSpotsPrefab;
    public GameObject barriersRoot;
    public GameObject[] barriers;
    public CarAgent agent;
    public float spawnChance;
    public int rows;
    public int cols;
    public int groups;
    public bool bigSpawn;

    public IEnumerator SpawnCars()
    {
        if (parkingSpotsHolder.transform.childCount > 0)
        {
            yield return ClearLot();
        }

        ResizeParkingLotSurface();

        float rPos = 0.0f;
        float cPos = 0.0f;
        float gPos = 0.0f;

        int goalR = Random.Range(0, rows);
        int goalC = Random.Range(0, cols);
        int goalG = Random.Range(0, groups);

        int randomIndex = -1;

        for (int g = 0; g < groups; g++)
        {
            for (int r = 0; r < rows; r++)
            {
                cPos = 0.0f + gPos;

                for (int c = 0; c < cols; c++)
                {
                    GameObject parkingSpots = (GameObject)Instantiate(parkingSpotsPrefab);
                    parkingSpots.transform.SetParent(parkingSpotsHolder.transform, false);
                    parkingSpots.transform.localPosition = new Vector3(rPos, 0.0f, cPos);
                    if (c == 0)
                    {
                        parkingSpots.transform.GetChild(5).gameObject.SetActive(true);
                    }
                    else if (c == cols - 1)
                    {
                        parkingSpots.transform.GetChild(6).gameObject.SetActive(true);
                    }

                    if (r == goalR && c == goalC && g == goalG)
                    {
                        randomIndex = Random.Range(0, 4);
                    }

                    for (int i = 0; i < 4; i++)
                    {

                        float randomChance = Random.value;

                        if (i == randomIndex)
                        {
                            randomChance = 2.0f;
                            goal.transform.position = parkingSpots.transform.GetChild(i).position;
                        }

                        if (randomChance < spawnChance)
                        {
                            float yRot = 90.0f;
                            if (i % 2 != 0)
                            {
                                yRot *= -1.0f;
                            }
                            
                            GameObject car = (GameObject)Instantiate(CarPrefabsManager.GetCar(), parkingSpots.transform.GetChild(i).position, Quaternion.Euler(0.0f, yRot, 0.0f));
                            car.transform.SetParent(parkingSpots.transform.GetChild(i), true);
                            car.GetComponent<Rigidbody>().isKinematic = false;
                        }
                    }

                    cPos += 4.0f;
                }
                rPos += 15.0f;
            }

            rPos = 0.0f;
            gPos += cols * 8.0f;
        }

        Vector3 goalScale;

        if (bigSpawn)
        {
            goalScale = new Vector3(7.0f, 0.15f, 7.0f);
        }
        else
        {
            goalScale = new Vector3(3.75f, 0.15f, 2.1f);
        }

        goal.transform.GetChild(0).localScale = goalScale;

        agent.GetComponent<Rigidbody>().isKinematic = false;
    }

    public IEnumerator ClearLot()
    {
        int childCount = parkingSpotsHolder.transform.childCount;
        Debug.Log("Child count: " + childCount.ToString());

        for (int i = 0; i < childCount; i++)
        {
            Destroy(parkingSpotsHolder.transform.GetChild(0).gameObject);
            yield return null;
        }
    }

    public void ResizeParkingLotSurface()
    {
        float zPos = groups * cols * 8.0f - 10.0f;
        float xPos = (rows - 1) * 15.0f;

        zPos *= 0.5f;
        xPos *= 0.5f;

        float zScale = groups * cols + 1;
        float xScale = rows * 2 + 1;

        parkingLotSurface.transform.localPosition = new Vector3(xPos, 0.0f, zPos);
        parkingLotSurface.transform.localScale = new Vector3(xScale, 1.0f, zScale);

        barriersRoot.transform.localPosition = new Vector3(xPos, 0.0f, zPos);

        barriers[0].transform.localPosition = new Vector3(xPos + 15.0f, 2.0f, 0.0f);
        barriers[0].GetComponent<BoxCollider>().size = new Vector3(0.5f, 5.0f, 150.0f);

        barriers[1].transform.localPosition = new Vector3(-xPos - 15.0f, 2.0f, 0.0f);
        barriers[1].GetComponent<BoxCollider>().size = new Vector3(0.5f, 5.0f, 150.0f);

        barriers[2].transform.localPosition = new Vector3(0.0f, 2.0f, zPos + 13.0f);
        barriers[2].GetComponent<BoxCollider>().size = new Vector3(150.0f, 5.0f, 0.5f);

        barriers[3].transform.localPosition = new Vector3(0.0f, 2.0f, -zPos - 13.0f);
        barriers[3].GetComponent<BoxCollider>().size = new Vector3(150.0f, 5.0f, 0.5f);
    }
}
