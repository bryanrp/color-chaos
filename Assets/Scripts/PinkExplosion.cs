using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkExplosion : MonoBehaviour
{
    private CircleCollider2D _circleCollider;
    private bool _isColliderExpanding = true;
    [Header("Collider")]
    [SerializeField] private float _colliderExpandVelocity = 8;
    [SerializeField] private float _colliderExpandTimeLimit = 0.4f;

    private AudioSource _audioSource;
    [Header("Audio")]
    [SerializeField] private AudioClip _audioExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = _audioExplosion;
        _audioSource.Play();

        StartCoroutine(StopExpanding());
    }

    // Update is called once per frame
    void Update()
    {
        if (_isColliderExpanding)
        {
            _circleCollider.radius += _colliderExpandVelocity * Time.deltaTime;
        }
    }

    private IEnumerator StopExpanding()
    {
        yield return new WaitForSeconds(_colliderExpandTimeLimit);
        _circleCollider.radius = 0.0001f;
        _isColliderExpanding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().ChangeHealth(-1);
        }
    }
}
