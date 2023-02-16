using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpenLevel : MonoBehaviour 
{
	public string levelName;

	void OnMouseDown ()
	{
		transform.localScale = new Vector3(0.9f, 0.9f, 1);
	}
	
	void OnMouseUp ()
	{
		transform.localScale = new Vector3(1, 1, 1);
	}

	void OnMouseUpAsButton ()
	{
		transform.localScale = new Vector3(1, 1, 1);

        AssetManager.Use.PlaySound(7);

        SceneManager.LoadScene(levelName);
	}
}