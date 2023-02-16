using UnityEngine;
using System.Collections;

public class OpenUrl : MonoBehaviour
{
	public string webUrls;
	public string appUrls;

	private bool launchedApp;

	void OnMouseDown()
	{
		transform.localScale = new Vector3(0.9f, 0.9f, 1);
	}

	void OnMouseUp()
	{
		transform.localScale = new Vector3(1, 1, 1);
		
//		Application.OpenURL(url);

		// Checks which device it's running on - STANDALONE is windows/mac/linux
		#if UNITY_STANDALONE
		Application.OpenURL (webUrls);
		#endif
		
		#if UNITY_ANDROID
		Application.OpenURL (appUrls);
		
		// Do a check to see if they have the app installed
		StartCoroutine(CheckApp());
		launchedApp = false;
		#endif
	}

	// If switched app, set to true so it won't launch the browser
	void OnApplicationPause()
	{
		launchedApp = true;
	}
	
	IEnumerator CheckApp()
	{
		// Wait for a time
		yield return new WaitForSeconds (0.5f);
		
		// If app hasn't launched, default to opening in browser
		if(!launchedApp)
		{
			Application.OpenURL (webUrls);
		}
	}
}