using System.Collections;
using UnityEngine;

//This code handles sprites going from transparent to seen
//Also handels whether the player or echo collided with the object.

public class ChangeColor : MonoBehaviour
{
    [Header("Colors")]
    private SpriteRenderer sprite;
    [SerializeField] private Color idleColor;
    [SerializeField] private Color activeColor;

    [Header("Settings")]
    [SerializeField] private float fadeInSpeed;
    [SerializeField] private float fadeOutSpeed = 2f;

    public bool isSeen = false;
    private bool PlayerOn;

    private Color targetColor; //Color trying to be reached
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = idleColor;
        targetColor = idleColor;
    }

    private void FixedUpdate()
    {
        isSeen = false;
    }

    void Update()
    {
        float currentSpeed = (targetColor == activeColor) ? fadeInSpeed : fadeOutSpeed;
        //Color.Lerp calculates the color between A and B
        //Everyframe is a step closer to targetColor
        sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * currentSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fadeInSpeed = 9f;
            isSeen = true;
            PlayerOn = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            fadeInSpeed = 8f;
            targetColor = activeColor;
            isSeen = true;
            PlayerOn = true;
        }
        /*
        else if (collision.gameObject.TryGetComponent(out ChangeColor other))
        {
            if (other.isSeen)
            {
                isSeen = true;
            }
        }*/
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        targetColor = idleColor;
        PlayerOn = false;
    }


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSeen)
        {
            if (collision.gameObject.CompareTag("EchoLocation"))
            {
                fadeInSpeed = 15f;
                isSeen = true;
                targetColor = activeColor;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isSeen)
        {
            if (collision.gameObject.CompareTag("EchoLocation"))
            {
                targetColor = activeColor;
                isSeen = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PlayerOn)
        {
            targetColor = idleColor;
        }
    }
}
