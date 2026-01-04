using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndDeactivateForExample : MonoBehaviour
{
	[SerializeField] GameObject[] objects;
	private void Start()
	{
		StartCoroutine("Enable");
	}
	IEnumerator Enable()
	{
		while (true)
		{
			for (int i = 0; i < 3; i++)
			{
				objects[i].SetActive(true);
			}
			yield return new WaitForSeconds(1f);
			for (int i = 0; i < 3; i++)
			{
				objects[i].SetActive(false);
			}
		}
	}
}
