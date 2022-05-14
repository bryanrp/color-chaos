using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WhiteLaser : MonoBehaviour
{
    private const int _numberOfLasers = 2;

    private static GameManager _gameManager;

    private GameObject[] _lasers;
    private BoxCollider2D[] _laserColliders;
    private List<Light2D>[] _laserLights;

    [Header("Laser")]
    [SerializeField] private float _laserChargingTime = 1.5f;
    [SerializeField] private float _laserChargingIntensityVelocity = 0.1f;
    [SerializeField] private float _laserActiveIntensityBoom = 1;
    [SerializeField] private float _laserActiveTime = 2;
    private GameObject _laserLightPrefab;
    private float _laserTime = 0;
    private bool _hasLaserActivated = false;

    private AudioSource _audioSource;
    [Header("Audio")]
    [SerializeField] private AudioClip _audioLaserCharging;
    [SerializeField] private AudioClip _audioLaserActive;

    // Start is called before the first frame update
    void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        _laserLightPrefab = transform.Find("Light").gameObject;

        _lasers = new GameObject[_numberOfLasers];
        _laserColliders = new BoxCollider2D[_numberOfLasers];
        _laserLights = new List<Light2D>[_numberOfLasers];
        for (int i = 0; i < _numberOfLasers; i++)
        {
            _lasers[i] = transform.Find("Laser" + i.ToString()).gameObject;
            
            _laserColliders[i] = _lasers[i].GetComponent<BoxCollider2D>();
            _laserColliders[i].enabled = false;

            _laserLights[i] = new List<Light2D>();
            float length = _lasers[i].GetComponent<SpriteRenderer>().size.x / 2;
            for (int pos = 0; pos < length; pos++)
            {
                Light2D light = Instantiate(_laserLightPrefab, _lasers[i].transform).GetComponent<Light2D>();
                light.transform.localPosition = new Vector3(pos, 0, 4);
                _laserLights[i].Add(light);
                if (pos != 0)
                {
                    light = Instantiate(_laserLightPrefab, _lasers[i].transform).GetComponent<Light2D>();
                    light.transform.localPosition = new Vector3(-pos, 0, 4);
                    _laserLights[i].Add(light);
                }
            }
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _audioLaserCharging;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        _laserTime += Time.deltaTime;
        if (_laserTime < _laserChargingTime)
        {
            for (int i = 0; i < _numberOfLasers; i++)
            {
                foreach (Light2D light in _laserLights[i])
                {
                    light.intensity += _laserChargingIntensityVelocity * Time.deltaTime;
                }
            }
        }
        else if (_laserTime < _laserChargingTime + _laserActiveTime)
        {
            if (!_hasLaserActivated)
            {
                _hasLaserActivated = true;
                _gameManager.ShakeCamera();
                for (int i = 0; i < _numberOfLasers; i++)
                {
                    _laserColliders[i].enabled = true;
                    _lasers[i].GetComponent<SpriteRenderer>().color = Color.white;
                    foreach (Light2D light in _laserLights[i])
                    {
                        light.intensity += _laserActiveIntensityBoom;
                    }
                }
            }
        }
        else Destroy(gameObject);
    }
}
