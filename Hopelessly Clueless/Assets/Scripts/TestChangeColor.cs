using UnityEngine;
using UnityEngine.UI;

public class TestChangeColor : MonoBehaviour
{
    [Header("Colors")]
    private Image groundRender;
    private Image image;
    [SerializeField] private Color idleColor = new Color(1.0f, 1.0f, 1.0f, 1);
    [SerializeField] private Color activeColor = new Color(1.0f, 1.0f, 1.0f, 0);

    [Header("Settings")]
    [SerializeField] private float fadeInSpeed = 7f;
    [SerializeField] private float fadeOutSpeed = 2f;

    public bool isSeen = false;

    private Color targetColor; //Color trying to be reached
    void Start()
    {
        image = GetComponent<Image>();
        image.color = idleColor;
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
        image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * currentSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Contact");
            isSeen = true;
        }
    }

    //Whenever the play keeps colliding with the wall.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundRender = collision.gameObject.GetComponent<Image>();
            Debug.Log($"Collision with {collision.gameObject.name}");
        }

        //If the player, then set targetColor to active color and set isSeen=true.
        if (collision.gameObject.CompareTag("Player"))
        {
            targetColor = activeColor;
            isSeen = true;
        }
        else if (collision.gameObject.TryGetComponent(out ChangeColor other))
        {
            if (other.isSeen)
            {
                isSeen = true;
            }
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        targetColor = idleColor;
    }
}

