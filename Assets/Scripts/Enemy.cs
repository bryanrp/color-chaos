using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static GameManager _gameManager;

    protected Rigidbody2D _rigidbody;
    protected SpriteRenderer _sprite;

    // Stats
    protected int _health;
    [Header("Stats")]
    [SerializeField] protected int _initialHealth;

    // Bullet
    protected float _bulletCooldownTime = 0;
    [Header("Bullet")]
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected float _bulletVelocity;
    [SerializeField] protected bool _bulletTrackPlayer;
    [SerializeField] protected float _bulletCooldown;

    // Sprite and Animation
    protected Shader _shaderDefault;
    private const float _hitWhiteTime = 0.2f;
    [Header("Sprite and Animation")]
    [SerializeField] private GameObject _hitParticlePrefab;
    [SerializeField] protected Shader _shaderGUI;

    // Audio
    private AudioSource _audioSource;
    [Header("Audio")]
    [SerializeField] private AudioClip _audioHit;
    [SerializeField] private AudioClip _audioDead;

    // Start is called before the first frame update
    protected void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _health = _initialHealth;
        _shaderDefault = _sprite.material.shader;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (_gameManager.IsGameOver) _rigidbody.velocity = Vector2.zero;
    }

    /// <summary>
    /// Add delta to the enemy's health.
    /// </summary>
    /// <param name="delta"></param>
    public void ChangeHealth(int delta)
    {
        _health += delta;
        if (_health <= 0)
        {
            Dead();
        }
        else
        {
            StartCoroutine(Hit());
        }
    }

    protected virtual void Dead()
    {
        Instantiate(_hitParticlePrefab, transform.position, _hitParticlePrefab.transform.rotation);
        _gameManager.NumberOfEnemies--;
        _gameManager.UpdateKill(1);

        _gameManager.PlayEnemyAudio(_audioDead);
        if (Random.Range(0f, 1f) < 0.1f) _gameManager.SpawnPowerup(transform.position);

        Destroy(gameObject);
    }

    private IEnumerator Hit()
    {
        Instantiate(_hitParticlePrefab, transform.position, _hitParticlePrefab.transform.rotation);
        _audioSource.clip = _audioHit;
        _audioSource.Play();

        _sprite.material.shader = _shaderGUI;
        yield return new WaitForSeconds(_hitWhiteTime);
        _sprite.material.shader = _shaderDefault;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().ChangeHealth(-1);
        }
    }
}
