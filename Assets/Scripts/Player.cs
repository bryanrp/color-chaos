using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private GameManager _gameManager;

    // Stats
    private int _health;
    private bool _isInvicible = false;
    [Header("Stats")]
    [SerializeField] private int _initialHealth = 3;
    private const float _invicibilityTime = 1;

    // Movement
    private Rigidbody2D _rigidbody;
    [Header("Movement")]
    private const float _velocity = 7;

    // Dash
    private bool _isDashActivated = false;
    private bool _isDashRecharged = true;
    [Header("Dash")]
    private ParticleSystem _dashRechargeParticle;
    private const float _dashVelocity = 25;
    private const float _dashTime = 0.4f;
    private const float _dashCooldown = 1.5f;
    private const float _dashRechargeParticleTime = 0.4f;

    // Shoot
    private bool _canShoot = true;
    private float _timeLeftFastShoot;
    [Header("Shoot")]
    [SerializeField] private GameObject _prefabBullet;
    private const float _bulletVelocity = 22;
    private const float _bulletCooldown = 0.8f;

    // Sprite and Animation
    private Animator _anim;
    private SpriteRenderer _sprite;
    private Shader _shaderDefault;
    private GameObject _body;
    private GameObject _eyes;
    [Header("Sprite and Animation")]
    [SerializeField] private GameObject _hitParticlePrefab;
    [SerializeField] private Shader _shaderGUI;
    private const float _hitWhiteTime = 0.2f;

    // Audio
    private AudioSource _audioSource;
    [Header("Audio")]
    [SerializeField] private AudioClip _audioShoot;
    [SerializeField] private AudioClip _audioDash;
    [SerializeField] private AudioClip _audioHit;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        _dashRechargeParticle = transform.Find("DashRechargeParticle").GetComponent<ParticleSystem>();
        _body = transform.Find("Body").gameObject;
        _eyes = transform.Find("Eyes").gameObject;

        _anim = _body.GetComponent<Animator>();
        _sprite = _body.GetComponent<SpriteRenderer>();
        _shaderDefault = _sprite.material.shader;

        _health = _initialHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager.IsGameOver)
        {
            if (!_isDashActivated) ProcessMovement();
            ProcessShoot();
            ProcessAnimation();

            TimeCycle();
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Add delta to the player's health.
    /// </summary>
    /// <param name="delta"></param>
    public void ChangeHealth(int delta)
    {
        // if enemy hit and invicible, then do nothing
        if (delta < 0 && _isInvicible) return;

        // apply the change
        _health += delta;
        if (_health <= 0)
        {
            StartCoroutine(_gameManager.GameOver());
        }
        else if (delta < 0)
        {
            StartCoroutine(Hit());
        }

        // update health UI
        _gameManager.UpdateHealth(_health);
    }
    
    /// <summary>
    /// Add delta seconds to timeLeftFastShoot.
    /// </summary>
    /// <param name="delta"></param>
    public void ChangeTimeFastShoot(float delta)
    {
        _timeLeftFastShoot += delta;
    }

    private IEnumerator Hit()
    {
        // start stats
        _isInvicible = true;

        // shake camera
        _gameManager.ShakeCamera();

        // creating hit particle
        Instantiate(_hitParticlePrefab, transform.position, _hitParticlePrefab.transform.rotation);

        // playing audio
        _audioSource.clip = _audioHit;
        _audioSource.Play();

        // hit white effect
        _sprite.material.shader = _shaderGUI;
        yield return new WaitForSeconds(_hitWhiteTime);
        _sprite.material.shader = _shaderDefault;

        // invicible
        _sprite.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(Mathf.Max(0, _invicibilityTime - _hitWhiteTime));
        _sprite.color = Color.white;

        // end stats
        _isInvicible = false;
    }

    private void ProcessAnimation()
    {
        if (_rigidbody.velocity.x > 0.1f) _eyes.transform.localScale = new Vector3(1, 1, 1);
        else if (_rigidbody.velocity.x < -0.1f) _eyes.transform.localScale = new Vector3(-1, 1, 1);
    }

    private IEnumerator ProcessDash()
    {
        // start stats
        _isInvicible = true;
        _isDashActivated = true;
        _isDashRecharged = false;

        // process dash input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (horizontalInput == 0 && verticalInput == 0) horizontalInput = _eyes.transform.localScale.x; // in case player just pressing shift
        Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;
        _rigidbody.velocity = direction * _dashVelocity;

        // start animation
        _anim.SetBool("dash_b", true);
        _body.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));

        // playing audio
        _audioSource.clip = _audioDash;
        _audioSource.Play();

        // waiting dash to end
        yield return new WaitForSeconds(_dashTime);

        // end stats
        _isInvicible = false;
        _isDashActivated = false;

        // end animation
        _anim.SetBool("dash_b", false);
        _body.transform.rotation = Quaternion.Euler(0, 0, 0);

        // waiting for starting dashParticleRecharge
        yield return new WaitForSeconds(_dashCooldown - _dashTime - _dashRechargeParticleTime);

        // start dashParticleRecharge
        _dashRechargeParticle.Play();

        // waiting for dashParticleRechargeTime
        yield return new WaitForSeconds(_dashRechargeParticleTime);
        
        // dash recharged
        _isDashRecharged = true;
    }

    private void ProcessMovement()
    {
        // process basic movement
        float horizontalInput = Input.GetAxis("Horizontal") * _velocity;
        float verticalInput = Input.GetAxis("Vertical") * _velocity;
        _rigidbody.velocity = new Vector2(horizontalInput, verticalInput);

        // process dash
        if (_isDashRecharged && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space)))
        {
            StartCoroutine(ProcessDash());
        }
    }

    private void ProcessShoot()
    {
        if (_canShoot && Input.GetKey(KeyCode.Mouse0))
        {
            // determining bullet direction
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (target - (Vector2)transform.position).normalized;

            // creating bullet
            Bullet bullet = Instantiate(_prefabBullet, transform.position, _prefabBullet.transform.rotation).GetComponent<Bullet>();
            bullet.SetStats(direction, _bulletVelocity, false);

            // playing audio
            bullet.PlaySound(_audioShoot);

            // end stats
            _canShoot = false;
            if (_timeLeftFastShoot < 0.01) StartCoroutine(RechargeShoot(_bulletCooldown));
            else StartCoroutine(RechargeShoot(_bulletCooldown / 4));
        }
    }

    private IEnumerator RechargeShoot(float timeRecharge)
    {
        yield return new WaitForSeconds(timeRecharge);
        _canShoot = true;
    }

    private void TimeCycle()
    {
        _timeLeftFastShoot = Mathf.Max(0, _timeLeftFastShoot - Time.deltaTime);
    }
}
