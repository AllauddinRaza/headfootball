using UnityEngine;
using System.Collections;

public class BallForceCollider : MonoBehaviour
{
    private GameManager gm;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball" && gm.gameMode == GameMode.PLAY)
        {
            ContactPoint2D contact = collision.contacts[0];
            Debug.DrawRay(contact.point, contact.normal, Color.white);

            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();

            Vector2 force;
            force.x = transform.position.x - contact.point.x;
            force.y = transform.position.y - contact.point.y;

            rig.AddForce(-force * 3, ForceMode2D.Impulse);

            if (Random.Range(0, 2.1f) > 1)
                AssetManager.Use.PlaySound(4);
            else
                AssetManager.Use.PlaySound(5);

            gm.CreateHitEffect(collision.transform.position);
        }
    }
}
