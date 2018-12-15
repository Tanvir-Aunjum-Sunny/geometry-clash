using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : ExtendedMonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public int Enemies;
        public float TimeBetweenSpawns;
    }

    public int TimeBetweenWaves = 2;
    public List<Wave> Waves = new List<Wave>();
    public Enemy EnemyPrefab;

    private Wave wave;
    private Coroutine spawnRoutine;
    private int remainingEnemies;
    private int waveNumber;

    
    void Start()
    {
        NextWave();
    }

    void Update()
    {

    }


    /// <summary>
    /// Prepare next wave of enemies
    /// </summary>
    private void NextWave()
    {
        waveNumber++;
        wave = Waves[waveNumber - 1];

        remainingEnemies = wave.Enemies;

        // Start next wave (after timeout)
        Wait(TimeBetweenWaves, () =>
        {
            spawnRoutine = StartCoroutine(SpawnEnemy());
        });
    }

    /// <summary>
    /// Spawn an enemy
    /// </summary>
    private IEnumerator SpawnEnemy()
    {
        while (remainingEnemies > 0)
        {
            remainingEnemies--;

            Enemy spawnedEnemy = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity);

            yield return new WaitForSeconds(wave.TimeBetweenSpawns);
        }

        StopCoroutine(spawnRoutine);
        yield return null;
    }
}
