using UnityEngine;
using System.Collections;

public class SoundBtn : MonoBehaviour
{
    public Sprite onIcon, offIcon;
    private SpriteRenderer mysp;

	void Start ()
	{
        mysp = GetComponent<SpriteRenderer>();

        SetSprite();
    }

    void SetSprite()
    {
        if (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1)
        {
            mysp.sprite = onIcon;
        }
        else
        {
            mysp.sprite = offIcon;
        }
    }

    void OnMouseDown()
    {
        transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }

    void OnMouseUp()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    void OnMouseUpAsButton()
    {
        transform.localScale = new Vector3(1, 1, 1);

        AssetManager.Use.PlaySound(7);

        if (PlayerPrefs.GetInt(VariablesName.Sound, 1) == 1)
        {
            PlayerPrefs.SetInt(VariablesName.Sound, 0);
        }
        else
        {
            PlayerPrefs.SetInt(VariablesName.Sound, 1);
        }

        PlayerPrefs.Save();
        AssetManager.Use.SetSoundVolume();
        SetSprite();
    }
}
