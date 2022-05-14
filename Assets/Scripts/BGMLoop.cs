using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMLoop : MonoBehaviour
{
    private const float _startLoopTime = 93.438f;
    private const float _endLoopTime = 195.679f;

    private GameManager _gameManager;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioSource.time > _endLoopTime)
        {
            _audioSource.time = _startLoopTime;
        }
        if (_gameManager.IsGameOver)
        {
            _audioSource.volume = 0.3f;
        }
    }
}
