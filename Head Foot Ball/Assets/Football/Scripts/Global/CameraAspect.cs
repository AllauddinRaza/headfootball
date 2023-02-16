using UnityEngine;

public class CameraAspect : MonoBehaviour
{
	void Start ()
	{
		GetComponent<Camera>().aspect = 16/10f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
}