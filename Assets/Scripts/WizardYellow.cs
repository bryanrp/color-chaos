using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WizardYellow : Enemy
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
    [SerializeField] private float _minDistToTarget = 4;
    // Minimum allowed waypoint distance to target
    [SerializeField] private float _minWaypointToTarget = 3;

    // Shoot
    [SerializeField] private float _minDistToShoot = 10;

    // Animation
    private GameObject _eyes;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _eyes = transform.Find("Eyes").gameObject;
        StartPathfinding();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!_gameManager.IsGameOver)
        {
            Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;

            ProcessShoot(ref difference);
        }
    }

    private void FixedUpdate()
    {
        if (!_gameManager.IsGameOver)
        {
            ProcessMovement();
        }
    }

    private void ProcessAnimation(ref Vector2 forceVector)
    {
        if (forceVector.x > 2) _eyes.transform.localScale = new Vector3(1, 1, 1);
        else if (forceVector.x < -2) _eyes.transform.localScale = new Vector3(-1, 1, 1);
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

        ProcessAnimation(ref forceVector);
    }

    private void ProcessShoot(ref Vector2 difference)
    {
        _bulletCooldownTime += Time.deltaTime;

        if (_bulletCooldownTime > _bulletCooldown && difference.magnitude < _minDistToShoot)
        {
            _bulletCooldownTime = 0;

            Bullet bullet = Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation).GetComponent<Bullet>();
            Vector2 direction = difference.normalized;
            bullet.SetStats(direction, _bulletVelocity, _bulletTrackPlayer);
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

    private void StartPathfinding()
    {
        _seeker = GetComponent<Seeker>();
        StartCoroutine(UpdatePathfinding());
    }

    private IEnumerator UpdatePathfinding()
    {
        while (true)
        {
            _seeker.StartPath(transform.position, _gameManager.GetPlayerPosition(), OnPathComplete);

            yield return new WaitForSeconds(_updatePathTimeInterval);
        }
    }
}
