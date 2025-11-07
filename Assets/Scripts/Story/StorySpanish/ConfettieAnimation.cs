using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConfettiAnimation : MonoBehaviour
{
    [Header("Confetti Settings")]
    public Sprite confettiSprite;
    public int confettiCount = 40;
    public float spawnDuration = 2f;
    public float fallSpeed = 200f;
    public float spreadX = 200f;
    public Vector2 lifetimeRange = new Vector2(1.5f, 2.5f);
    public Vector2 sizeRange = new Vector2(20f, 40f);

    [Header("Parent Canvas")]
    public RectTransform canvasRect;

    private List<GameObject> activeConfetti = new List<GameObject>();
    private Coroutine confettiRoutine;

    public void PlayConfetti()
    {
        if (confettiSprite == null || canvasRect == null) return;

        ClearConfetti();

        if (confettiRoutine != null)
            StopCoroutine(confettiRoutine);

        confettiRoutine = StartCoroutine(SpawnConfettiRoutine());
    }

    IEnumerator SpawnConfettiRoutine()
    {
        float startTime = Time.time;
        float halfWidth = canvasRect.rect.width / 2f;
        float halfHeight = canvasRect.rect.height / 2f;

        while (Time.time - startTime < spawnDuration)
        {
            float spawnY = halfHeight + 50f;

            SpawnSingleConfetti(Random.Range(-halfWidth, 0f), spawnY);
            SpawnSingleConfetti(Random.Range(0f, halfWidth), spawnY);

            yield return new WaitForSeconds(0.05f);
        }
    }

    void SpawnSingleConfetti(float xPos, float yPos)
    {
        GameObject confettiObj = new GameObject("Confetti", typeof(Image));
        confettiObj.transform.SetParent(canvasRect, false);
        activeConfetti.Add(confettiObj); // 🔹 Track this

        RectTransform rt = confettiObj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(xPos, yPos);

        float size = Random.Range(sizeRange.x, sizeRange.y);
        rt.sizeDelta = new Vector2(size, size);

        Image img = confettiObj.GetComponent<Image>();
        img.sprite = confettiSprite;
        img.color = new Color(Random.value, Random.value, Random.value);

        rt.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        float lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);

        StartCoroutine(MoveConfetti(rt, lifetime));
    }

    IEnumerator MoveConfetti(RectTransform rt, float lifetime)
    {
        float timer = 0f;
        Vector2 velocity = new Vector2(Random.Range(-spreadX, spreadX), -fallSpeed);

        while (timer < lifetime)
        {
            rt.anchoredPosition += velocity * Time.deltaTime;
            rt.Rotate(Vector3.forward * 200f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (rt != null)
        {
            activeConfetti.Remove(rt.gameObject);
            Destroy(rt.gameObject);
        }
    }

    public void ClearConfetti()
    {
        foreach (var c in activeConfetti)
        {
            if (c != null) Destroy(c);
        }
        activeConfetti.Clear();
    }

    private void OnDisable()
    {
        ClearConfetti();
    }
}
