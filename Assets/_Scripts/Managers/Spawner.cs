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
    private int enemiesRemainingInWave;
    private int enemiesRemainingToSpawn;
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
        // Prevent exceeding number of waves
        if (++waveNumber > Waves.Count) return;

        wave = Waves[waveNumber - 1];

        enemiesRemainingToSpawn = wave.Enemies;
        enemiesRemainingInWave = enemiesRemainingToSpawn;

        // Start next wave (after timeout)
        Wait(TimeBetweenWaves, () =>
        {
            spawnRoutine = StartCoroutine(SpawnEnemy());
        });
    }

    /// <summary>
    /// Respond to enemy death
    /// </summary>
    private void OnEnemyDeath()
    {
        if (enemiesRemainingInWave-- > 0) return;

        // Next wave starts immediately after last enemy death
        NextWave();
    }

    /// <summary>
    /// Spawn enemies while a wave lasts
    /// </summary>
    private IEnumerator SpawnEnemy()
    {
        while (enemiesRemainingToSpawn > 0)
        {
            enemiesRemainingToSpawn--;

            Enemy spawnedEnemy = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;

            yield return new WaitForSeconds(wave.TimeBetweenSpawns);
        }

        StopCoroutine(spawnRoutine);
    }
}
