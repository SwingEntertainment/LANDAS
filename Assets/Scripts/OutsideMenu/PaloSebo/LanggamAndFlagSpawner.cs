using UnityEngine;

public class LanggamAndFlagSpawner : MonoBehaviour
{
    [Header("🪲 Prefabs")]
    public GameObject LanggamLeftPrefab;
    public GameObject LanggamRightPrefab;
    public GameObject FlagLeftPrefab;
    public GameObject FlagRightPrefab;

    [Header("⚙️ Spawn Settings")]
    public float spawnInterval = 1.5f; // DECREASED from 3f: Tries to spawn objects twice as often.
    public float minLanggamCooldown = 3f; // DECREASED from 8f: Ants spawn sooner.
    public float maxLanggamCooldown = 5f; // DECREASED from 12f: Ants spawn sooner.
    public float minFallSpeed = 60f; // INCREASED from 10f: Minimum fall speed is much higher.
    public float maxFallSpeed = 100f; // INCREASED from 25f: Maximum fall speed is much higher.

    [Header("📍 Spawn Positions")]
    public Vector2 leftSpawnPos = new Vector2(500.4f, 423.8f);
    public Vector2 rightSpawnPos = new Vector2(800.3f, 422.2f);

    [Header("📏 Object Size Settings")]
    public Vector3 langgamScale = new Vector3(3.5f, 2.8f, 1f);
    public Vector3 flagScale = new Vector3(7f, 5.5f, 1f);

    [Header("🧱 Layer Settings")]
    public string sortingLayer = "Default";
    public int sortingOrder = 11;

    private float timer;
    private bool spawnLeftNext = true;
    private float lastLanggamSpawnTime = -999f;
    private float currentLanggamCooldown;

    void Start()
    {
        currentLanggamCooldown = Random.Range(minLanggamCooldown, maxLanggamCooldown);
    }

    void Update()
    {
        if (!PaloSeboGameManager.GameStarted)
            return;
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnObjectOnSide(spawnLeftNext);
            spawnLeftNext = !spawnLeftNext;
            timer = 0f;
        }
    }

    public void ResetSpawner()
    {
        timer = 0f;
        lastLanggamSpawnTime = -999f;
        currentLanggamCooldown = Random.Range(minLanggamCooldown, maxLanggamCooldown);
    }

    void TrySpawnObjectOnSide(bool spawnLeft)
    {
        int activeLanggams = GameObject.FindGameObjectsWithTag("Langgam").Length;
        // ADJUSTED FOR TESTING: This makes the flag spawn 8 times more often (1/5 chance).
        // Change back to Random.Range(0, 40) for a rare flag.
        bool spawnFlag = Random.Range(0, 15) == 0;

        bool canSpawnLanggam = (Time.time - lastLanggamSpawnTime >= currentLanggamCooldown);

        if (!spawnFlag && (!canSpawnLanggam || activeLanggams >= 3))
            return;

        GameObject prefabToSpawn = spawnFlag
            ? (spawnLeft ? FlagLeftPrefab : FlagRightPrefab)
            : (spawnLeft ? LanggamLeftPrefab : LanggamRightPrefab);

        Vector2 spawnPos = spawnLeft ? leftSpawnPos : rightSpawnPos;

        if (prefabToSpawn != null)
        {
            // Add small random vertical offset for spacing
            spawnPos.y += Random.Range(-10f, 10f);

            GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            obj.transform.localScale = spawnFlag ? flagScale : langgamScale;

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = sortingLayer;
                sr.sortingOrder = sortingOrder;
            }

            // Random fall speed is now significantly faster
            float randomFallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
            FallingObject fall = obj.GetComponent<FallingObject>();
            if (fall != null)
            {
                fall.fallSpeed = randomFallSpeed;
            }

            if (!spawnFlag)
            {
                lastLanggamSpawnTime = Time.time;
                currentLanggamCooldown = Random.Range(minLanggamCooldown, maxLanggamCooldown);
            }
        }
    }
}