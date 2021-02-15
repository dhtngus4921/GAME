using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public GameObject player;
    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;
    public int heart = 3;
    public int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<EnemyController>().Length;

        if (enemyCount == 0) {
            SpawnPlayer();
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
            level++; 
        }
    }

    void SpawnEnemyWave(int enemiewToSpawn)
    {
        for (int i = 0; i < enemiewToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return randomPos;
    }

    private void SpawnPlayer()
    {
        if (player.transform.position.y < -10)
        {
            player.transform.position = new Vector3(0, 0, 0);
            //player 다시 스폰 되었을때 input값도 초기화 
            //일정 시간 지난 후 enemy와 player 움직이도록 -> game reset
            //게임 끝난 후 시간 delay

            enemyCount--;
            waveNumber--;
            heart--;
        }

        if(heart == 0)
        {
            System.Console.WriteLine("Game over!");
            SceneManager.LoadScene("GameOver");
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}