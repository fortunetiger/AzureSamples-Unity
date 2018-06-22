using UnityEngine;

/// <summary>
/// This class follows the Unity tutorial:
/// https://unity3d.com/learn/tutorials/topics/2d-game-creation/recycling-obstacles-object-pooling?playlist=17093
/// </summary>
public class ColumnPool : MonoBehaviour 
{
    [SerializeField]
    private GameObject columnPrefab;

    [SerializeField]
    private float spawnRate = 4;

    [SerializeField]
    private float minSpawnRate = 2;

    [SerializeField]
    private float columnMinY = -1;

    [SerializeField]
    private float columnMaxY = 3.5f;

    [SerializeField]
    private float columnSpawnX = 10;

    private int columnPoolSize = 6;
    private Vector2 objectPoolPosition = new Vector2(-15, -25);
    private GameObject[] columns;
    private float timeSinceLastSpawned;
    private int currentColumn = 0;
    private float spawnRateDecrementAmount;

	// Use this for initialization
	void Awake () 
	{
        spawnRateDecrementAmount = (spawnRate - minSpawnRate) / LevelMusicPlayer.MusicFileNamesInLevelOrder.Length;

        timeSinceLastSpawned = 0;

        columns = new GameObject[columnPoolSize];

        for (int i = 0; i < columnPoolSize; i++)
        {
            columns[i] = Instantiate(columnPrefab, objectPoolPosition, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
        timeSinceLastSpawned += Time.deltaTime;

        if (!GameControl.Instance.IsGameOver && timeSinceLastSpawned >= spawnRate)
        {
            timeSinceLastSpawned = 0;

            float spawnYPosition = Random.Range(columnMinY, columnMaxY);
            columns[currentColumn].transform.position = new Vector2(columnSpawnX, spawnYPosition);

            currentColumn++;
            if (currentColumn >= columnPoolSize)
            {
                currentColumn = 0;
            }
        }
	}

    private void OnStartedNewLevel(int levelIndex)
    {
        if (levelIndex > 0)
            spawnRate -= spawnRateDecrementAmount;
    }

    private void OnEnable()
    {
        GameControl.StartedNewLevel += OnStartedNewLevel;
    }

    private void OnDisable()
    {
        GameControl.StartedNewLevel -= OnStartedNewLevel;
    }
}
