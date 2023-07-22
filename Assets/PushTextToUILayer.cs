using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTextToUILayer : MonoBehaviour {

	void Start () {
		GetComponent<Renderer>().sortingLayerName = "UI";
	}

}
