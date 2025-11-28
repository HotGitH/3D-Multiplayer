using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform[] spawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //makes the spawn points invisable
        foreach(Transform spawn in spawnPoints)
        {
            spawn.gameObject.SetActive(false);
        }
    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
