using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private static GameManager _gameManager;

    [Header("Stats")]
    [SerializeField] private int _type;
    [SerializeField] private AudioClip _audioPickup;

    private float _startPosY;
    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        _startPosY = transform.position.y;
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float y = _startPosY + Mathf.Abs(Mathf.Sin((Time.time - _startTime) * 2)) / 2;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (_type == 0) player.ChangeHealth(1);
            else player.ChangeTimeFastShoot(5);
            _gameManager.PlayPowerupAudio(_audioPickup);
            Destroy(gameObject);
        }
    }
}
