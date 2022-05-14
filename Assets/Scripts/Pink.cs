using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pink : Enemy
{
    [Header("Movement")]
    [SerializeField] private float _movementForce = 8;
    [SerializeField] private float _movementDirectionInterpolate = 1;

    [Header("Explode")]
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _distanceToExplode = 3;
    [SerializeField] private float _timeToExplode = 0.3f;
    private bool _isExploded = false;

    [Header("Explode Animation")]
    [SerializeField] private float _explodeAnimationTime = 2;

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

            if (difference.magnitude < _distanceToExplode && !_isExploded)
            {
                _isExploded = true;
                StartCoroutine(Exploding());
            }
        }
    }

    private IEnumerator Exploding()
    {
        _sprite.material.shader = _shaderGUI;
        _sprite.color = Color.red;

        yield return new WaitForSeconds(_timeToExplode);

        if (_health > 0 && !_gameManager.IsGameOver)
        {
            Instantiate(_explosionPrefab, transform.position, _explosionPrefab.transform.rotation);
            Dead();
        }

        /*
        int blip = 3;
        for (int i=0; i<blip; i++)
        {
            sprite.material.shader = shaderGUI;
            sprite.color = Color.red;

            yield return new WaitForSeconds(blipTime);
            if (health <= 0) break;

            sprite.material.shader = shaderDefault;
            sprite.color = Color.white;

            yield return new WaitForSeconds(Mathf.Max(0, timeToExplode / blip - blipTime));
            if (health <= 0) break;
        }

        if (health > 0)
        {
            Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
            Dead();
        }
        */
    }

    private Vector2 ProcessDirection(Vector2 direction)
    {
        float angle = transform.rotation.eulerAngles.z;
        Vector2 newDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        newDirection = Vector2.Lerp(newDirection, direction, _movementDirectionInterpolate);

        float newAngle = Vector2.SignedAngle(Vector2.right, newDirection);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
        return newDirection;
    }

    private void ProcessMovement(ref Vector2 difference)
    {
        Vector2 forceDirection = ProcessDirection(difference.normalized);
        float angleConstant = Mathf.Max(0, 1 - Vector2.Angle(forceDirection, difference.normalized) / 30);
        _rigidbody.AddForce(forceDirection * angleConstant * _movementForce, ForceMode2D.Force);
    }
}