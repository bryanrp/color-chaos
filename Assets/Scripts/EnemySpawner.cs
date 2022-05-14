using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static GameManager _gameManager;

    private float _limitX;
    private float _limitY;
    private float _outX;
    private float _outY;

    [Header("Enemies")]
    [SerializeField] private GameObject[] _prefabEnemies;
    [SerializeField] private float _minDistToPlayer = 4;

    private float blueSpawnTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (_gameManager == null)
        {
            _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }
        _limitX = _gameManager.LimitX;
        _limitY = _gameManager.LimitY;
        _outX = _gameManager.OutX;
        _outY = _gameManager.OutY;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_gameManager.IsGameOver) return;

        int kill = _gameManager.NumberOfKill;
        int enemy = _gameManager.NumberOfEnemies;
        float rng = Random.Range(0f, 1f);

        if (kill < 3)
        {
            if (enemy <= 0) SpawnEnemyAtRandomPosition(0);
        }
        else if (kill < 5)
        {
            if (enemy <= 1)
            {
                if (rng < 0.5) SpawnEnemyAtRandomPosition(0);
                else SpawnEnemyAtRandomPosition(1);
            }
        }
        else if (kill < 8)
        {
            if (enemy <= 1)
            {
                if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                else SpawnEnemyAtRandomPosition(1);
            }
        }
        else if (kill < 12)
        {
            if (enemy <= 2)
            {
                if (rng < 0.4) SpawnEnemyAtRandomPosition(0);
                else SpawnEnemyAtRandomPosition(1);
            }
        }
        else if (kill < 15)
        {
            if (enemy <= 3)
            {
                if (rng < 0.4) SpawnEnemyAtRandomPosition(0);
                else SpawnEnemyAtRandomPosition(1);
            }
        }
        else if (kill < 25)
        {
            if (enemy <= 4)
            {
                if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                else if (rng < 0.6) SpawnEnemyAtRandomPosition(1);
                else SpawnEnemyAtRandomPosition(3);
            }
        }
        else if (kill < 30)
        {
            if (enemy <= 4)
            {
                if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                else if (rng < 0.6) SpawnEnemyAtRandomPosition(1);
                else if (rng < 0.85) SpawnEnemyAtRandomPosition(3);
                else SpawnEnemyAtRandomPosition(4);
            }
        }
        else if (kill < 40)
        {
            if (enemy <= 5)
            {
                if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                else if (rng < 0.6) SpawnEnemyAtRandomPosition(1);
                else if (rng < 0.9) SpawnEnemyAtRandomPosition(3);
                else SpawnEnemyAtRandomPosition(4);
            }
        }
        else if (kill < 50)
        {
            if (enemy <= 6)
            {
                if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                else if (rng < 0.6) SpawnEnemyAtRandomPosition(1);
                else if (rng < 0.9) SpawnEnemyAtRandomPosition(3);
                else SpawnEnemyAtRandomPosition(4);
            }
        }
        else if (kill < 70)
        {
            if (enemy <= 6)
            {
                if (_gameManager.NumberOfWhite > 0)
                {
                    if (rng < 0.3) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.6) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.9) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
                else
                {
                    if (rng < 0.1) SpawnWhite();
                    else if (rng < 0.4) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.65) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.9) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
            }
        }
        else if (kill < 90)
        {
            if (enemy <= 7)
            {
                if (_gameManager.NumberOfWhite > 0)
                {
                    if (rng < 0.2) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.5) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.75) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
                else
                {
                    if (rng < 0.1) SpawnWhite();
                    else if (rng < 0.2) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.5) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.75) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
            }
        }
        else
        {
            if (enemy <= Mathf.Min(12, Mathf.RoundToInt(kill / 10.0f) - 2))
            {
                if (_gameManager.NumberOfWhite > 1)
                {
                    if (rng < 0.2) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.5) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.75) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
                else
                {
                    if (rng < 0.1) SpawnWhite();
                    else if (rng < 0.2) SpawnEnemyAtRandomPosition(0);
                    else if (rng < 0.5) SpawnEnemyAtRandomPosition(1);
                    else if (rng < 0.75) SpawnEnemyAtRandomPosition(3);
                    else SpawnEnemyAtRandomPosition(4);
                }
            }
        }

        blueSpawnTime += Time.deltaTime;
        if (kill >= 8)
        {
            if (kill < 60)
            {
                if (blueSpawnTime > 8)
                {
                    blueSpawnTime = 0;
                    SpawnBlue();
                }
            }
            else
            {
                if (blueSpawnTime > 4)
                {
                    blueSpawnTime = 0;
                    SpawnBlue();
                }
            }
        }
    }

    private Vector2 RandomPosition()
    {
        return new Vector2(Random.Range(-_limitX, _limitX), Random.Range(-_limitY, _limitY));
    }

    private void SpawnBlue()
    {
        bool goingRight = (Random.Range(0, 2) == 0);
        float posY = Random.Range(-_limitY + 2, _limitY - 2);
        float angle = Random.Range(-10f, 10f);

        Vector2 spawnPos = new Vector2(goingRight ? -_outX : _outX, posY);
        Vector2 direction = _gameManager.RotateVector(goingRight ? Vector2.right : Vector2.left, angle * Mathf.Deg2Rad);
        Blue blue = Instantiate(_prefabEnemies[2], spawnPos, _prefabEnemies[2].transform.rotation).GetComponent<Blue>();
        blue.SetStats(direction, 3);
        _gameManager.NumberOfEnemies++;
    }

    private void SpawnEnemyAtRandomPosition(int enemyIndex)
    {
        Vector2 spawnPosition = RandomPosition();
        if ((spawnPosition - _gameManager.GetPlayerPosition()).magnitude < _minDistToPlayer) return;
        Instantiate(_prefabEnemies[enemyIndex], spawnPosition, _prefabEnemies[enemyIndex].transform.rotation);
        _gameManager.NumberOfEnemies++;
    }

    private void SpawnWhite()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(-_limitX / 2, _limitX / 2), Random.Range(-_limitY / 2, _limitY / 2));
        if ((spawnPosition - _gameManager.GetPlayerPosition()).magnitude < _minDistToPlayer) return;
        Instantiate(_prefabEnemies[5], spawnPosition, _prefabEnemies[5].transform.rotation);
        _gameManager.NumberOfEnemies++;
        _gameManager.NumberOfWhite++;
    }
}
