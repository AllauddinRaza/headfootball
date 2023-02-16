using UnityEngine;
using System.Collections;

public class HitEffectScript : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer sp;
    private int index;

	void Start ()
	{
        sp = GetComponent<SpriteRenderer>();
        index = 0;
        sp.sprite = sprites[index];

        StartCoroutine(Effect());
	}

    private IEnumerator Effect()
    {
        yield return new WaitForSeconds(0.05f);
        index++;

        if (index < sprites.Length)
        {
            sp.sprite = sprites[index];
            StartCoroutine(Effect());
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
