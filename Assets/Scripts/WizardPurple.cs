using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WizardPurple : Enemy
{
    // Pathfinding
    private Path _path;
    private Seeker _seeker;
    [SerializeField] private float _updatePathTimeInterval = 1;
    [SerializeField] private float _nextWaypointDistance = 1;
    private int _currentWaypointIndex = 0;

    // Movement
    [SerializeField] private float _movementForce = 3;
    // Minimum allowed distance to target
    [SerializeField] private float _minDistToTarget = 3;
    // Minimum allowed waypoint distance to target
    [SerializeField] private int _minWaypointToTarget = 2;

    // Bullet
    [SerializeField] private int _maxNumberOfBullets = 4;
    [SerializeField] private float _bulletRadius = 2;
    /// <summary>
    /// Angular speed of the bullets respective to WizardPurple in 2PI rad/s <br/>
    /// Speed equal to 0.5 means that the bullets will rotate 0.5 of the circle per second
    /// </summary>
    [SerializeField] private float _bulletSpeed = 0.5f;
    private const int _fullAngle = 360;
    private bool[] _isBulletSlotEmpty;
    private Transform _bullets;
    private int _numberOfBullets = 0;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        StartPathfinding();

        _bullets = transform.Find("Bullets");
        _isBulletSlotEmpty = new bool[_maxNumberOfBullets];
        for (int i = 0; i < _maxNumberOfBullets; i++)
        {
            _isBulletSlotEmpty[i] = true;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!_gameManager.IsGameOver)
        {
            Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;

            ProcessShoot(ref difference);
            ProcessBulletMovement();
        }
    }

    /// <summary>
    /// Restore the bullet slot indexed type and decrement the number of bullets
    /// </summary>
    /// <param name="type"></param>
    public void RestoreBullet(int type)
    {
        _isBulletSlotEmpty[type] = true;
        _numberOfBullets--;
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void ProcessBulletMovement()
    {
        float angle = _bullets.eulerAngles.z;
        angle = ((angle % _fullAngle) + _fullAngle) % _fullAngle;
        angle += Mathf.Rad2Deg * 2 * Mathf.PI * _bulletSpeed * Time.deltaTime;
        _bullets.eulerAngles = new Vector3(0, 0, angle);
    }

    private void ProcessMovement()
    {
        Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;
        float distanceToPlayer = difference.magnitude;

        if (_path != null)
        {
            while (_currentWaypointIndex < _path.vectorPath.Count)
            {
                difference = _path.vectorPath[_currentWaypointIndex] - transform.position;
                if (difference.magnitude < _nextWaypointDistance) _currentWaypointIndex++;
                else break;
            }
        }

        if ((_path == null || _currentWaypointIndex >= _path.vectorPath.Count) && distanceToPlayer < _minDistToTarget)
        {
            difference = -difference;
        }
        else if (_path != null && _path.vectorPath.Count - _currentWaypointIndex <= _minWaypointToTarget && distanceToPlayer < _minDistToTarget)
        {
            difference = -difference;
        }

        Vector2 forceVector = _movementForce * difference.normalized;
        _rigidbody.AddForce(forceVector);
    }

    private void ProcessShoot(ref Vector2 difference)
    {
        if (_numberOfBullets < _maxNumberOfBullets)
        {
            _bulletCooldownTime += Time.deltaTime;
            if (_bulletCooldownTime > _bulletCooldown)
            {
                _bulletCooldownTime = 0;
                SpawnBullet();
            }
        }

    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypointIndex = 0;
        }
    }

    private void SpawnBullet()
    {
        int type = 0;
        for (int i = 0; i < _maxNumberOfBullets; i++)
        {
            if (_isBulletSlotEmpty[i])
            {
                type = i;
                break;
            }
        }

        float x = _bulletRadius;
        if (type % 2 == 1)
        {
            x = -x;
        }
        float y = _bulletRadius;
        if (type / 2 == 1)
        {
            y = -y;
        }

        WizardPurpleBullet bullet = Instantiate(_bulletPrefab, _bullets).GetComponent<WizardPurpleBullet>();
        bullet.transform.localPosition = new Vector3(x, y, 0);
        bullet.SetStats(this, type);
        _isBulletSlotEmpty[type] = false;
        _numberOfBullets++;
    }

    private void StartPathfinding()
    {
        _seeker = GetComponent<Seeker>();
        StartCoroutine(UpdatePathfinding());
    }

    private IEnumerator UpdatePathfinding()
    {
        while (true)
        {
            Debug.Log("starting path");
            _seeker.StartPath(transform.position, _gameManager.GetPlayerPosition(), OnPathComplete);

            yield return new WaitForSeconds(_updatePathTimeInterval);
        }
    }
}
