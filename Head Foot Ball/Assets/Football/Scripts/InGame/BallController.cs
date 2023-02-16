using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private Rigidbody2D myRig;
    private GameManager gm;

    void Awake()
    {
        myRig = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        Vector3 temp = myRig.velocity;

        if (temp.x > 15 || temp.x < -15)
        {
            temp.x = 15 * (temp.x / Mathf.Abs(temp.x));
            myRig.velocity = temp;
        }

        if (temp.y > 15 || temp.y < -15)
        {
            temp.y = 15 * (temp.y / Mathf.Abs(temp.y));
            myRig.velocity = temp;
        }

        if (gm.gameMode == GameMode.PLAY)
        {
            transform.Rotate(new Vector3(0, 0, temp.x / (Mathf.Abs(temp.x) + float.Epsilon) * -6));
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (gm.gameMode == GameMode.PLAY)
        {
            if (col.gameObject.name == "P1Goal")
            {
                Vector3 temp = myRig.velocity;
                temp.x /= 2f;
                temp.y /= 2f;
                myRig.velocity = temp;

                gm.gameMode = GameMode.SLOWMOTION;
                StartCoroutine(gm.CreateBallWait(2));
                AssetManager.Use.PlaySound(6);
            }

            if (col.gameObject.name == "P2Goal")
            {
                Vector3 temp = myRig.velocity;
                temp.x /= 2f;
                temp.y /= 2f;
                myRig.velocity = temp;

                myRig.velocity = new Vector2(5, 5);
                gm.gameMode = GameMode.SLOWMOTION;
                StartCoroutine(gm.CreateBallWait(1));
                AssetManager.Use.PlaySound(6);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (gm.gameMode == GameMode.PLAY)
        {
            if (col.gameObject.tag == "Ground")
            {
                if (Random.Range(0, 2.1f) > 1)
                    AssetManager.Use.PlaySound(4);
                else
                    AssetManager.Use.PlaySound(5);
            }

            if (col.gameObject.tag == "Holder")
            {
                if (Random.Range(0, 2.1f) > 1)
                    AssetManager.Use.PlaySound(4);
                else
                    AssetManager.Use.PlaySound(5);
            }

            if (col.gameObject.tag == "Ram")
            {
                if (Random.Range(0, 2.1f) > 1)
                    AssetManager.Use.PlaySound(2);
                else
                    AssetManager.Use.PlaySound(3);
            }
        }
    }
}
