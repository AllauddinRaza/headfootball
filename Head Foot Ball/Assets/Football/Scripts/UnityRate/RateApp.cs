using UnityEngine;
using System.Collections;

public class RateApp : MonoBehaviour 
{
    void OnMouseDown ()
	{
		transform.localScale = new Vector3(0.9f, 0.9f, 1);
	}

	void OnMouseUp ()
	{
		transform.localScale = new Vector3(1, 1, 1);
    }

    void OnMouseUpAsButton()
    {
        transform.localScale = new Vector3(1, 1, 1);

        Rate();
    }

    public void OnClickForUI()
    {
        Rate();
    }

    public void Rate()
    {
        #if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
        #elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + Application.identifier);
        #endif
    }
}
