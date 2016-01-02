using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    public float speed = 1f;
    public LayerMask blockingLayer;

    // Use this for initialization
    void Start() {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }


    //protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    //{
    //    Vector2 start = transform.position;
    //    Vector2 end = start + new Vector2(xDir, yDir);

    //    boxCollider.enabled = false;
    //    hit = Physics2D.Linecast(start, end, blockingLayer);
    //    boxCollider.enabled = true;

    //    if (hit.transform == null)
    //    {
    //        StartCoroutine(SmoothMovement(end));
    //        return true;
    //    }

    //    return false;
    //}

    //protected IEnumerator SmoothMovement(Vector3 end)
    //{
    //    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

    //    while (sqrRemainingDistance > float.Epsilon)
    //    {
    //        Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, Time.deltaTime);
    //        rb2D.MovePosition(newPosition);
    //        sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    //        yield return null;
    //    }
    //}

    // Update is called once per frame

    void Update ()
    {
        var jump = Input.GetAxisRaw("Jump");
        var horizontal = Input.GetAxisRaw("Horizontal");

        Vector2 start = rb2D.position;
        Vector2 step = start + new Vector2(horizontal, jump);

        if (jump != 0.0f || horizontal != 0.0f)
        {
            var newPosition = Vector3.MoveTowards(rb2D.position, step, 5 * Time.deltaTime);

            rb2D.MovePosition(newPosition);
        }
    }
}
