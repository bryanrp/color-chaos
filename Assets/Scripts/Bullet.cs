using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    protected static GameManager _gameManager;
    private Rigidbody2D _rigidbody;

    [Header("Stats")]
    [SerializeField] private bool _isFromPlayer;
    [SerializeField] private GameObject _deadParticlePrefab;
    [SerializeField] private int _bounceMax = 0;
    private int _bounceDone = 0;

    [Header("Movement")]
    private bool _canTrackPlayer;
    private float _movementVelocity;
    private Vector2 _movementDirection;

    // Sprite and animation
    private float _spriteDirection;

    // Start is called before the first frame update
    void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        SetMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (_canTrackPlayer)
        {
            _movementDirection = (_gameManager.GetPlayerPosition() - (Vector2)transform.position).normalized;
            SetMovement();
        }
        else if (_bounceMax > 0)
        {
            _movementDirection = _rigidbody.velocity.normalized;
            SetMovement();
        }
    }

    private void LateUpdate()
    {
        if (_gameManager.IsGameOver) _rigidbody.velocity = Vector2.zero;
    }

    /// <summary>
    /// Kill this bullet
    /// </summary>
    public void Dead()
    {
        Instantiate(_deadParticlePrefab, transform.position, _deadParticlePrefab.transform.rotation);
        Destroy(gameObject);
    }

    /// <summary>
    /// Returns whether this bullet is from player or not
    /// </summary>
    /// <returns></returns>
    public bool IsFromPlayer()
    {
        return _isFromPlayer;
    }

    /// <summary>
    /// Set the bullet's direction, velocity, and whether it canTrackPlayer.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="velocity"></param>
    /// <param name="canTrackPlayer"></param>
    public void SetStats(Vector2 direction, float velocity = 0, bool canTrackPlayer = false)
    {
        _movementDirection = direction;
        _movementVelocity = velocity;
        _canTrackPlayer = canTrackPlayer;
    }

    /// <summary>
    /// Play an audio clip in this object's audioSource component.
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isFromPlayer)
        {
            Dead();
            collision.GetComponent<Player>().ChangeHealth(-1);
        }
        else if (collision.CompareTag("Enemy") && _isFromPlayer)
        {
            Dead();
            collision.GetComponent<Enemy>().ChangeHealth(-1);
        }
        else if (collision.CompareTag("Wall"))
        {
            if (_bounceDone < _bounceMax)
            {
                _bounceDone++;
            }
            else
            {
                Dead();
            }
        }
    }

    private void SetMovement()
    {
        _rigidbody.velocity = _movementDirection * _movementVelocity;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, _movementDirection));
    }
}
