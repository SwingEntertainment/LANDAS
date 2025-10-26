using System.Collections;
using UnityEngine;

public class PlayerKickAnimation : MonoBehaviour
{
    [Header("Setup")]
    public SpriteRenderer playerRenderer; 
    public Sprite[] kickFrames;        
    public float frameDelay = 0.1f;         

    private bool isKicking = false;
    private Sprite defaultSprite;

    private void Awake()
    {
        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (playerRenderer != null)
            defaultSprite = playerRenderer.sprite;
        else
            Debug.LogError("❌ PlayerKickAnimation: SpriteRenderer not assigned!");
    }

    public IEnumerator PlayKickAnimation()
    {
        if (isKicking) yield break;
        if (playerRenderer == null || kickFrames.Length == 0)
        {
            Debug.LogWarning("⚠️ PlayerKickAnimation: Missing frames or SpriteRenderer!");
            yield break;
        }

        isKicking = true;
        Debug.Log("✅ Kick animation started!");

        foreach (Sprite frame in kickFrames)
        {
            playerRenderer.sprite = frame;
            yield return new WaitForSeconds(frameDelay);
        }

        playerRenderer.sprite = defaultSprite;
        isKicking = false;
    }
}
