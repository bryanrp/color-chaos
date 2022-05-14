using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green : Enemy
{
    [Header("Movement")]
    [SerializeField] private float _movementForce = 2;
    [SerializeField] private float _minDistToPlayer = 4;
    [SerializeField] private float _minDistToShoot = 10;
    [SerializeField] private float _shootAngle = 30;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!_gameManager.IsGameOver)
        {
            Vector2 difference = _gameManager.GetPlayerPosition() - (Vector2)transform.position;

            ProcessMovement(ref difference);
            ProcessShoot(ref difference);
        }
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
        // update the last time this enemy shoots
        _bulletCooldownTime += Time.deltaTime;

        // check if allowed to shoot now
        if (difference.magnitude < _minDistToShoot && _bulletCooldownTime > _bulletCooldown)
        {
            // reset timer
            _bulletCooldownTime = 0;
            
            // shoot three bullet
            Vector2 direction = (_gameManager.GetPlayerPosition() - (Vector2)transform.position).normalized;
            for (int i=-1; i<2; i++)
            {
                Bullet bullet = Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation).GetComponent<Bullet>();
                float angle = Mathf.Deg2Rad * _shootAngle * i;
                float curX = direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle);
                float curY = direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle);
                Vector2 curDirection = new Vector2(curX, curY);
                bullet.SetStats(curDirection, _bulletVelocity, _bulletTrackPlayer);
            }
        }
    }
}
