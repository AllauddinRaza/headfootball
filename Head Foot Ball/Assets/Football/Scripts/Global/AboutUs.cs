using System.Collections;
using System.Collections.Generic;
using UnityEngine;
	
public class AboutUs : MonoBehaviour {
	public string url = "http://yoursite.com/aboutus";
	void OnMouseUp()
	{
		transform.localScale = new Vector3(1, 1, 1);
		Application.OpenURL(url);
	}

	void OnMouseDown()
	{
		transform.localScale = new Vector3(0.9f, 0.9f, 1);
	}
}
