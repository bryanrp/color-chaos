using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private static GameManager _gameManager;

    [Header("Movement")]
    [SerializeField] private float _smoothTime = 5;
    [SerializeField] private float _limitX = 3;
    [SerializeField] private float _limitY = 3;
    Vector3 velocity = Vector3.zero;

    /*
    [Header("CameraUI")]
    public GameObject _cameraUI;
    public RectTransform _killUI;
    public Text _killText;
    public RectTransform _pauseUI;
    public RectTransform _grayOverlay;
    public Animator _blackTransition;
    */

    // Start is called before the first frame update
    void Start()
    {
        if (_gameManager == null) _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        MoveCamera();
    }

    /// <summary>
    /// Restart the game. Should only be called by RestartButton.
    /// </summary>
    public void RestartGame()
    {
        _gameManager.RestartGame();
    }

    /// <summary>
    /// Shakes the camera.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="strength"></param>
    /// <returns></returns>
    public IEnumerator ShakeCamera(float time = 0.2f, float strength = 0.1f)
    {
        while (time > 0)
        {
            float dx = Random.Range(-strength, strength);
            float dy = Random.Range(-strength, strength);
            transform.position = new Vector3(transform.position.x + dx, transform.position.y + dy, transform.position.z);

            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void MoveCamera()
    {
        Vector3 target = _gameManager.GetPlayerPosition();
        target.z = transform.position.z;
        target.x = Mathf.Max(-_limitX, Mathf.Min(_limitX, target.x));
        target.y = Mathf.Max(-_limitY, Mathf.Min(_limitY, target.y));
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, _smoothTime * Time.deltaTime);
    }
}
