using UnityEngine;

public class EchoChangeColor : MonoBehaviour
{
    [Header("Colors")]
    private SpriteRenderer sprite;
    [SerializeField] private Color idleColor;
    [SerializeField] private Color activeColor;
    private Color targetColor;

    [Header("Settings")]
    [SerializeField] private float fadeInSpeed = 7f;
    [SerializeField] private float fadeOutSpeed = 2f;
    public bool isSeen = false;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EchoLocation"))
        {
            isSeen = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EchoLocation"))
        {
            targetColor = activeColor;
            isSeen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        targetColor = idleColor;
    }

}
