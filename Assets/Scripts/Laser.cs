using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This class's functionality has been covered by WhiteLaser.", true)]
public class Laser : MonoBehaviour
{
    private GameManager gameManager;
    private SpriteRenderer sprite;

    [Header("Laser")]
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] private float intensityVelocity = 0.1f;
    [SerializeField] private float intensityBoom = 1;
    [SerializeField] private float glowingTime = 1.5f;
    [SerializeField] private float laserTime = 2;
    private List<Light> lights;
    private bool chargedUp = false;
    private float time = 0;

    [Header("Audio")]
    [SerializeField] private AudioClip laserCharging;
    [SerializeField] private AudioClip laserActive;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        sprite = GetComponent<SpriteRenderer>();
        lights = new List<Light>();
        float radius = sprite.size.x / 2;
        for (int i=0; i<radius; i++)
        {
            Light light = Instantiate(lightPrefab, transform).GetComponent<Light>();
            light.transform.localPosition = new Vector3(i, 0, 4);
            lights.Add(light);
            if (i != 0)
            {
                light = Instantiate(lightPrefab, transform).GetComponent<Light>();
                light.transform.localPosition = new Vector3(-i, 0, 4);
                lights.Add(light);
            }
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = laserCharging;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsGameOver)
        {
            time += Time.deltaTime;
            if (time < glowingTime)
            {
                foreach (Light light in lights)
                {
                    light.intensity += intensityVelocity * Time.deltaTime;
                }
            }
            else if (time < glowingTime + laserTime)
            {
                if (!chargedUp)
                {
                    gameManager.ShakeCamera();
                    sprite.color = Color.white;
                    foreach (Light light in lights)
                    {
                        light.intensity += intensityBoom;
                    }
                    chargedUp = true;
                }
            }
            else Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((glowingTime < time) && (time < glowingTime + laserTime) && (collision.CompareTag("Player")))
        {
            collision.GetComponent<Player>().ChangeHealth(-1);
        }
    }
}
