using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class White : Enemy
{
    [Header("Movement")]
    [SerializeField] private float _angularVelocity = 25;

    [Header("Laser")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _laserCooldown = 7;
    [SerializeField] private GameObject _absorbParticlePrefab;
    [SerializeField] private float _absorbParticleTime = 0.1f;
    private float _laserCooldownTime = 0;
    private bool _isAbsorbing = false;

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
            float nextAngle = transform.rotation.eulerAngles.z + _angularVelocity * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, nextAngle % 360);

            _laserCooldownTime += Time.deltaTime;
            if (!_isAbsorbing && _laserCooldownTime > _laserCooldown - _absorbParticleTime)
            {
                Instantiate(_absorbParticlePrefab, transform.position, transform.rotation);
                _isAbsorbing = true;
            }
            if (_laserCooldownTime > _laserCooldown)
            {
                _laserCooldownTime = 0;
                InstantiateLaser();
            }
        }
    }

    private void InstantiateLaser()
    {
        Instantiate(_laserPrefab, transform);
        _isAbsorbing = false;
    }

    protected override void Dead()
    {
        _gameManager.NumberOfWhite--;
        base.Dead();
    }
}
