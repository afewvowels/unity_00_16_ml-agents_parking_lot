using UnityEngine;

public class CarPrefabsManager : MonoBehaviour
{
    public GameObject[] carPrefabsHelper;
    public static GameObject[] carPrefabs;

    private void Awake()
    {
        carPrefabs = new GameObject[carPrefabsHelper.Length];

        for (int i = 0; i < carPrefabsHelper.Length; i++)
        {
            carPrefabs[i] = carPrefabsHelper[i];
        }
    }

    public static GameObject GetCar()
    {
        return carPrefabs[Random.Range(0, carPrefabs.Length)];
    }
}