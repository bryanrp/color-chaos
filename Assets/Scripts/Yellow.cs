using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Yellow : Enemy
{
    [Header("Movement")]
    [SerializeField] private float _movementForce = 3;
    [SerializeField] private float _minDistToPlayer = 4;
    [SerializeField] private float _minDistToShoot = 10;

    [Header("Pathfinding")]
    private float _nextWaypointDistance = 1;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        
        // seeker = GetComponent<Seeker>();
        // InvokeRepeating("UpdatePath", 0.0f, 0.5f);
    }

    // Update is called once per frame
    new void Update()
    {
        /*
        if (path == null) return;
        if (currentWaypoint < path.vectorPath.Count)
        {
            Vector2 difference = (path.vectorPath[currentWaypoint] - transform.position);
            Vector2 forceVector = difference.normalized * force;
            rb.AddForce(forceVector, ForceMode2D.Force);

            ProcessAnimation(forceVector);

            if (difference.magnitude < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
        */

        base.Update();
        if (!_gameManager.IsGameOver)
        {
            Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;
            
            // ProcessMovement(ref difference);
            ProcessShoot(ref difference);
        }
    }

    private void FixedUpdate()
    {
        Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;
        ProcessMovement(ref difference);
    }

    private void ProcessAnimation(Vector3 forceVector)
    {
        if (forceVector.x > 0.3f) transform.localScale = new Vector3(1, 1, 1);
        else if (forceVector.x < -0.3f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ProcessMovement(ref Vector2 difference)
    {
        Vector2 forceVector;
        if (difference.magnitude < _minDistToPlayer)
        {
            forceVector = difference.normalized * -_movementForce;
        }
        else
        {
            forceVector = difference.normalized * _movementForce;
        }
        _rigidbody.AddForce(forceVector);
        ProcessAnimation(forceVector);
    }

    private void ProcessShoot(ref Vector2 difference)
    {
        _bulletCooldownTime += Time.deltaTime;

        if (difference.magnitude < _minDistToShoot && _bulletCooldownTime > _bulletCooldown)
        {
            _bulletCooldownTime = 0;

            Bullet bullet = Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation).GetComponent<Bullet>();
            Vector2 direction = difference.normalized;
            bullet.SetStats(direction, _bulletVelocity, _bulletTrackPlayer);
        }
    }

    /*
    void UpdatePath()
    {
        seeker.StartPath(transform.position, _gameManager.GetPlayerPosition(), OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    */
}
