using UnityEngine;

public class LanggamAndFlagSpawner : MonoBehaviour
{
    [Header("🪲 Prefabs")]
    public GameObject LanggamLeftPrefab;
    public GameObject LanggamRightPrefab;
    public GameObject FlagLeftPrefab;
    public GameObject FlagRightPrefab;

    [Header("⚙️ Spawn Settings")]
    public float spawnInterval = 3f;
    public float minLanggamCooldown = 8f;
    public float maxLanggamCooldown = 12f;
    public float minFallSpeed = 10f;
    public float maxFallSpeed = 25f;

    [Header("📍 Spawn Positions")]
    public Vector2 leftSpawnPos = new Vector2(489.8f, 429.5f);
    public Vector2 rightSpawnPos = new Vector2(594.8f, 423.6f);

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
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnObjectOnSide(spawnLeftNext);
            spawnLeftNext = !spawnLeftNext;
            timer = 0f;
        }
    }

    void TrySpawnObjectOnSide(bool spawnLeft)
    {
        int activeLanggams = GameObject.FindGameObjectsWithTag("Langgam").Length;
        bool spawnFlag = Random.Range(0, 40) == 0;
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

            // Random fall speed
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
