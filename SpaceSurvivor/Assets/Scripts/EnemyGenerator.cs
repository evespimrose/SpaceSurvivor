using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private int enemyLimit = 5;
    private float spawnDelay = 0.5f;
    private float limitIncreaseInterval = 5f;


    private void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseEnemyLimit());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitUntil(() => activeEnemies.Count < enemyLimit);

            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            activeEnemies.Add(newEnemy);

            newEnemy.GetComponent<Enemy>().OnDestroyed += () => activeEnemies.Remove(newEnemy);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private IEnumerator IncreaseEnemyLimit()
    {
        while (true)
        {

            yield return new WaitForSeconds(limitIncreaseInterval);

            enemyLimit++;
        }
    }

    private void TrackAndNotifyClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            float distance = Vector2.Distance(activeEnemies[i].GetComponent<Enemy>().targetTransform.position, activeEnemies[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = activeEnemies[i];
            }
        }

        // Send the closest enemy information to the Player via PacketManager
        if (closestEnemy != null)
        {
            ClosestEnemyMessage message = new ClosestEnemyMessage(closestEnemy);
            PacketManager.Instance.SendMessage(message);
        }
    }
}
