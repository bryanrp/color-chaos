using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player _player;
    private CameraManager _cameraManager;

    [Header("Spawn Area")]
    public readonly float LimitX = 14;
    public readonly float LimitY = 7;
    public readonly float OutX = 17;
    public readonly float OutY = 10;

    [Header("HUD or In-game UI")]
    private GameObject _cameraUI;
    private GameObject _grayOverlay;
    private RectTransform _killUI;
    private RectTransform _pauseUI;
    private GameObject _restartButton;
    private Animator _blackTransition;
    [SerializeField] private float _killUISmoothTime;
    [SerializeField] private float _blackTransitionTime;
    [SerializeField] private GameObject _prefabHeartRed;
    [SerializeField] private GameObject _prefabHeartYellow;
    private Text _killCount;
    private LinkedList<GameObject> _hearts;
    private const int _heartSpace = 100;


    [Header("Game State")]
    public int NumberOfEnemies = 0;
    public int NumberOfWhite = 0;
    public int NumberOfKill = 0;
    public bool IsGameOver { get; private set; } = false;
    private bool _isGamePaused = false;

    [Header("Audio")]
    [SerializeField] private AudioClip _audioGameOver;
    [SerializeField] private AudioClip _audioRestart;
    private AudioSource _audioSource;
    private AudioSource _enemyAudioSource;
    private AudioSource _powerupAudioSource;

    [Header("Powerup")]
    [SerializeField] private GameObject[] _prefabPowerups;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // _player = GameObject.Find("Player").GetComponent<Player>();
        _cameraManager = Camera.main.GetComponent<CameraManager>();

        _cameraUI = GameObject.Find("CameraUI");
        _killUI = _cameraUI.transform.Find("KillUI").GetComponent<RectTransform>();
        _killCount = _killUI.Find("KillCount").GetComponent<Text>();
        _restartButton = _cameraUI.transform.Find("RestartButton").gameObject;
        _pauseUI = _cameraUI.transform.Find("PauseUI").GetComponent<RectTransform>();
        _blackTransition = _cameraUI.transform.Find("BlackTransition").GetComponent<Animator>();
        _grayOverlay = _cameraUI.transform.Find("GrayOverlay").gameObject;

        _audioSource = GetComponent<AudioSource>();
        _enemyAudioSource = transform.Find("EnemyAudioSource").GetComponent<AudioSource>();
        _powerupAudioSource = transform.Find("PowerupAudioSource").GetComponent<AudioSource>();

        _hearts = new LinkedList<GameObject>();
        for (int i=0; i<3; i++)
        {
            AddHealth();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            _isGamePaused = !_isGamePaused;
            _grayOverlay.SetActive(_isGamePaused);
            _pauseUI.gameObject.SetActive(_isGamePaused);
            if (_isGamePaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GameOver()
    {
        if (!IsGameOver)
        {
            IsGameOver = true;
            _grayOverlay.SetActive(true);

            _audioSource.clip = _audioGameOver;
            _audioSource.Play();

            Vector2 target = new Vector2(0, Camera.main.scaledPixelHeight / 2);
            // Debug.Log("Target: " + target.x + " " + target.y);
            Vector2 killUIVelocity = Vector2.zero;
            while ((target - _killUI.anchoredPosition).magnitude > 10)
            {
                _killUI.anchoredPosition = Vector2.SmoothDamp(_killUI.anchoredPosition, target, ref killUIVelocity, _killUISmoothTime * Time.unscaledDeltaTime);
                yield return new WaitForEndOfFrame();
            }

            _restartButton.SetActive(true);
        }
    }

    /// <summary>
    /// Returns the player position.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    /// <summary>
    /// Play an audio clip from an enemy.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayEnemyAudio(AudioClip clip) {
        _enemyAudioSource.clip = clip;
        _enemyAudioSource.Play();
    }

    /// <summary>
    /// Play an audio clip from a powerup.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayPowerupAudio(AudioClip clip)
    {
        _powerupAudioSource.clip = clip;
        _powerupAudioSource.Play();
    }

    /// <summary>
    /// Restart the game. Should only be called when the game already over.
    /// </summary>
    public void RestartGame()
    {
        _audioSource.clip = _audioRestart;
        _audioSource.Play();
        StartCoroutine(ReloadScene());
    }

    /// <summary>
    /// Returns a rotated Vector2 from to angle radians.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector2 RotateVector(Vector2 from, float angle)
    {
        float curX = from.x * Mathf.Cos(angle) - from.y * Mathf.Sin(angle);
        float curY = from.x * Mathf.Sin(angle) + from.y * Mathf.Cos(angle);
        Vector2 result = new Vector2(curX, curY);
        return result;
    }

    /// <summary>
    /// Shakes the camera.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="strength"></param>
    public void ShakeCamera(float time = 0.2f, float strength = 0.1f)
    {
        StartCoroutine(_cameraManager.ShakeCamera(time, strength));
    }

    /// <summary>
    /// Spawns a random powerup at position.
    /// </summary>
    /// <param name="position"></param>
    public void SpawnPowerup(Vector3 position)
    {
        int type = Random.Range(0, _prefabPowerups.Length);
        Instantiate(_prefabPowerups[type], position, _prefabPowerups[type].transform.rotation);
    }

    /// <summary>
    /// Update the player's health UI to playerHealth. Call only when the player's health updated.
    /// </summary>
    /// <param name="playerHealth"></param>
    public void UpdateHealth(int playerHealth)
    {
        while (_hearts.Count < playerHealth)
        {
            AddHealth();
        }
        while (_hearts.Count > Mathf.Max(0, playerHealth))
        {
            RemoveHealth();
        }
    }

    /// <summary>
    /// Add delta to the number of enemy kill.
    /// </summary>
    /// <param name="delta"></param>
    public void UpdateKill(int delta)
    {
        NumberOfKill += delta;
        _killCount.text = NumberOfKill.ToString();
    }

    private void AddHealth()
    {
        RectTransform heart;
        if (_hearts.Count < 3) heart = Instantiate(_prefabHeartRed, _cameraUI.transform).GetComponent<RectTransform>();
        else heart = Instantiate(_prefabHeartYellow, _cameraUI.transform).GetComponent<RectTransform>();
        heart.anchoredPosition = new Vector2((_hearts.Count - 1) * _heartSpace, heart.anchoredPosition.y);
        _hearts.AddLast(heart.gameObject);
    }

    private IEnumerator ReloadScene()
    {
        _blackTransition.SetTrigger("increase_t");
        yield return new WaitForSecondsRealtime(_blackTransitionTime);
        SceneManager.LoadScene(0);
    }

    private void RemoveHealth()
    {
        Destroy(_hearts.Last.Value);
        _hearts.RemoveLast();
    }
}