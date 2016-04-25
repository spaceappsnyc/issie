using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {


	// Update is called once per frame
	void Update () {

		transform.Rotate (new Vector3 (1, 3, 0) * Time.deltaTime);
	}
}

