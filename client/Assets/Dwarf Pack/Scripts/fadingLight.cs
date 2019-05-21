using UnityEngine;
using System.Collections;

public class fadingLight : MonoBehaviour {

	private Light lt;
	public float duration;
	void Start()
	{
		lt = GetComponent<Light>();
	}
	void Update()
	{
		if (lt.intensity > 0)
			lt.intensity -= Time.deltaTime / duration;
	}
}
