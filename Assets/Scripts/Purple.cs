using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purple : Enemy
{
    private static readonly Vector2[] Dir = new Vector2[] { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

    [Header("Movement")]
    [SerializeField] private float _angularVelocity = 30;

    private GameObject _eyes;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _eyes = transform.Find("Eyes").gameObject;
        StartCoroutine(ProcessShoot());
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!_gameManager.IsGameOver)
        {
            ProcessMovement();
            ProcessEyes();
        }
    }

    private void ProcessEyes()
    {
        float dx = _gameManager.GetPlayerPosition().x - transform.position.x;
        if (dx > 0.1f) _eyes.transform.localScale = new Vector3(1, 1, 1);
        else if (dx < -0.1f) _eyes.transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ProcessMovement()
    {
        float nextAngle = transform.rotation.eulerAngles.z + _angularVelocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, nextAngle % 360);
    }

    private IEnumerator ProcessShoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(_bulletCooldown);
            if (gameObject == null) break;
            float angle = transform.rotation.eulerAngles.z;
            if (angle > 180) angle -= 360;
            angle *= Mathf.Deg2Rad;
            for (int i = 0; i<4; i++)
            {
                Bullet bullet = Instantiate(_bulletPrefab, transform.position,  _bulletPrefab.transform.rotation).GetComponent<Bullet>();
                Vector2 direction = _gameManager.RotateVector(Dir[i], angle);
                bullet.SetStats(direction, _bulletVelocity, _bulletTrackPlayer);
            }
        }
    }
}
