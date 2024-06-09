using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public SpawnData[] stage1SpawnData;
    public SpawnData[] stage2SpawnData;
    public float levelTime;

    int level;
    float timer;
    List<SpawnData> currentSpawnData;

    void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        currentSpawnData = new List<SpawnData>();
        SetStageSpawnData(1); // 초기 스테이지는 1로 설정
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        if (currentSpawnData.Count == 0)
            return;

        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), currentSpawnData.Count - 1);

        if (timer > currentSpawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        if (level >= currentSpawnData.Count)
            return;

        GameObject enemyPrefab = currentSpawnData[level].enemyPrefab;
        GameObject enemy = GameManager.instance.pool.Get(enemyPrefab);

        if (enemy == null)
        {
            Debug.LogError("Failed to spawn enemy: " + enemyPrefab.name);
            return;
        }

        if (level == currentSpawnData.Count - 1)
        {
            enemy.transform.position = spawnPoints[1].position;
            enemy.GetComponent<Enemy>().Init(currentSpawnData[level]);
            currentSpawnData[level].spawnTime = 5000;
        }
        else if (level == currentSpawnData.Count - 2)
        {
            enemy.transform.position = spawnPoints[Random.Range(1, 4)].position;
            enemy.GetComponent<Enemy>().Init(currentSpawnData[level]);
        }
        else
        {
            enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
            enemy.GetComponent<Enemy>().Init(currentSpawnData[level]);
        }
    }

    public void SetStageSpawnData(int stage)
    {
        currentSpawnData.Clear();
        SpawnData[] selectedSpawnData = stage == 1 ? stage1SpawnData : stage2SpawnData;

        foreach (SpawnData data in selectedSpawnData)
        {
            currentSpawnData.Add(data);
        }

        if (currentSpawnData.Count > 0)
        {
            levelTime = GameManager.instance.maxGameTime / currentSpawnData.Count;
        }
        else
        {
            levelTime = GameManager.instance.maxGameTime;
        }

    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public GameObject enemyPrefab;
    public int spriteType;
    public int health;
    public float speed;
}
