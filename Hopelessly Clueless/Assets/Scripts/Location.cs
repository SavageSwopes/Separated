using UnityEngine;
//This code handels sending out the echo object and destroying it once it leaves a wall.
public class Location : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float force;
    private bool secondWave;
    private bool charging;
    private bool waveHold;

    [Header("Animation Variables")]
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("secondWave", secondWave);
        animator.SetBool("isCharging", charging);
        animator.SetBool("waveHold", waveHold);
    }

    public void OnChargeEnd()
    {
        charging = false;
        waveHold = true;
    }

    public void OnAnimationEnd()
    {
        secondWave = true;
    }

    public void Launch(Vector2 direction)
    {
      waveHold = false;
      charging = false;
      rb.AddForce(direction * force);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }


}
