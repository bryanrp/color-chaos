using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private readonly int _minX = -15;
    [SerializeField] private readonly int _maxX = 15;
    [SerializeField] private readonly int _minY = -8;
    [SerializeField] private readonly int _maxY = 8;
    [SerializeField] private readonly int _posZ = 5;
    private int lengthX, lengthY;

    [Header("Floor")]
    [SerializeField] private GameObject[] _prefabFloors;
    private GameObject[][] _floors;

    // Start is called before the first frame update
    void Start()
    {
        lengthX = _maxX - _minX + 1;
        lengthY = _maxY - _minY + 1;
        _floors = new GameObject[lengthX][];
        for (int i=0; i<lengthX; i++)
        {
            _floors[i] = new GameObject[lengthY];
            for (int j=0; j<lengthY; j++)
            {
                int rng = Random.Range(0, _prefabFloors.Length);
                _floors[i][j] = Instantiate(_prefabFloors[rng], transform);
                _floors[i][j].transform.localPosition = new Vector3(_minX + i, _minY + j, _posZ);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
