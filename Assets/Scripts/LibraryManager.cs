using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LibraryManager : MonoBehaviour
{
    private GameManager _gameManager;
    private AstarPath _astarPath;

    // Spawn
    [SerializeField] private float _limitX = 22;
    [SerializeField] private float _limitY = 12;
    [SerializeField] private float _minDistToPlayer = 5;

    // Wizards
    [SerializeField] private List<GameObject> _enemyPrefabs;

    // Walls
    private List<GameObject> _walls;
    [SerializeField] private float _wallChangeTime = 10;
    [SerializeField] private float _wallLifeTime = 20;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _astarPath = GameObject.Find("AStar").GetComponent<AstarPath>();

        StartWall();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.IsGameOver) return;

        SpawnEnemy();
    }

    private Vector2 GetSpawnPosition()
    {
        float x = Random.Range(-_limitX, _limitX);
        float y = Random.Range(-_limitY, _limitY);
        return new Vector2(x, y);
    }

    private void SpawnEnemy()
    {
        int numberOfEnemies = _gameManager.NumberOfEnemies;
        int numberOfKills = _gameManager.NumberOfKill;

        if (numberOfKills < 3)
        {
            if (numberOfEnemies < 1)
            {
                SpawnEnemy(0);
            }
        }
        else if (numberOfKills < 5)
        {
            if (numberOfEnemies < 2)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.4f) SpawnEnemy(0);
                else SpawnEnemy(1);
            }
        }
        else if (numberOfKills < 10)
        {
            if (numberOfEnemies < 3)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.3f) SpawnEnemy(0);
                else SpawnEnemy(1);
            }
        }
        else if (numberOfKills < 15)
        {
            if (numberOfEnemies < 3)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.3f) SpawnEnemy(0);
                else if (rng < 0.7f) SpawnEnemy(1);
                else SpawnEnemy(2);
            }
        }
        else if (numberOfKills < 22)
        {
            if (numberOfEnemies < 4)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.3f) SpawnEnemy(0);
                else if (rng < 0.7f) SpawnEnemy(1);
                else SpawnEnemy(2);
            }
        }
        else if (numberOfKills < 30)
        {
            if (numberOfEnemies < 5)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.2f) SpawnEnemy(0);
                else if (rng < 0.6f) SpawnEnemy(1);
                else SpawnEnemy(2);
            }
        }
        else if (numberOfKills < 40)
        {
            if (numberOfEnemies < 6)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.2f) SpawnEnemy(0);
                else if (rng < 0.5f) SpawnEnemy(1);
                else if (rng < 0.8f) SpawnEnemy(2);
                else SpawnEnemy(3);
            }
        }
        else
        {
            if (numberOfEnemies < 7)
            {
                float rng = Random.Range(0f, 1f);
                if (rng < 0.1f) SpawnEnemy(0);
                else if (rng < 0.4f) SpawnEnemy(1);
                else if (rng < 0.7f) SpawnEnemy(2);
                else SpawnEnemy(3);
            }
        }
    }

    private void SpawnEnemy(int enemyIndex)
    {
        Vector2 spawnPosition = GetSpawnPosition();
        if ((_gameManager.GetPlayerPosition() - spawnPosition).magnitude > _minDistToPlayer)
        {
            Instantiate(_enemyPrefabs[enemyIndex], spawnPosition, _enemyPrefabs[enemyIndex].transform.rotation);
            _gameManager.NumberOfEnemies++;
        }
    }

    private void StartWall()
    {
        _walls = new List<GameObject>();
        foreach (Transform child in transform)
        {
            _walls.Add(child.gameObject);
        }

        StartCoroutine(UpdateWall());
    }

    private IEnumerator UpdateWall()
    {
        while (true)
        {
            yield return new WaitForSeconds(_wallChangeTime);
            if (_gameManager.IsGameOver) break;

            int wallIndex = Random.Range(0, _walls.Count);
            _walls[wallIndex].SetActive(true);

            _astarPath.Scan();

            yield return new WaitForSeconds(_wallLifeTime);
            if (_gameManager.IsGameOver) break;

            _walls[wallIndex].SetActive(false);

            _astarPath.Scan();
        }
    }
}
