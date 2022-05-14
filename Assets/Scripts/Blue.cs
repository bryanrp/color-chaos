using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : Enemy
{
    private const float Radius = 2;

    private Vector2 _movementDirection;
    private float _movementVelocity;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _rigidbody.velocity = _movementDirection * _movementVelocity;
        _rigidbody.angularVelocity = (_movementVelocity / Radius) * Mathf.Rad2Deg;
        if (_rigidbody.velocity.x > 0) _rigidbody.angularVelocity = -_rigidbody.angularVelocity;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!_gameManager.IsGameOver)
        {
            if (Mathf.Abs(transform.position.x) > _gameManager.OutX + 3 || Mathf.Abs(transform.position.y) > _gameManager.OutY + 3)
            {
                _gameManager.UpdateKill(-1);
                Dead();
            }
        }
        else
        {
            _rigidbody.angularVelocity = 0;
        }
    }

    /// <summary>
    /// Set the direction and linear velocity.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="velocity"></param>
    public void SetStats(Vector2 direction, float velocity)
    {
        _movementDirection = direction;
        _movementVelocity = velocity;
    }
}
