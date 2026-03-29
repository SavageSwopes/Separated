using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float respawnTime = 1f;
    private SpriteRenderer spriteRenderer;

    AudioManager audioManager;

  

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            audioManager.PlaySFX(audioManager.death);
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(Respawn(respawnTime));
    }

    IEnumerator Respawn(float duration)
    {
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(duration);
        transform.position = startPos;
        spriteRenderer.enabled = true;
    }
}
