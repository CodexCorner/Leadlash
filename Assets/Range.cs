using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range {

	public float worst;
	public float best;

	public float step;

	public float Get (float t) {
		if (step > 0) {
			return Mathf.Round(Mathf.Lerp(worst,best,t)/step)*step;
		} else {
			return Mathf.Lerp(worst,best,t);
		}
	}

}