using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This class's functionality has been covered by WhiteLaser.", true)]
public class LaserLaser : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserTotalTime;
    private float angularVelocity;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.1f);
        Instantiate(laserPrefab, transform);
        Instantiate(laserPrefab, transform).transform.localRotation = Quaternion.Euler(0, 0, 90);
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float nextAngle = transform.rotation.eulerAngles.z + angularVelocity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, nextAngle % 360);
        if (Time.time - startTime > laserTotalTime) Destroy(gameObject);
    }

    public void SetStats(float angularVelocity)
    {
        this.angularVelocity = angularVelocity;
    }
}
