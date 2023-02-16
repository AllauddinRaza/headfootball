using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackAndroid : MonoBehaviour 
{
	public string ParentLeveleName;
	public bool isMain;

	void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            OnMouseUpAsButton();
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

        if (!isMain)
            SceneManager.LoadScene(ParentLeveleName);
        else
            Application.Quit();
    }
}
