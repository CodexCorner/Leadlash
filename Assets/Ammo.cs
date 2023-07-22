using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

	public Player player;

	public float ammoAngle;
	public float distance;

	public float targetDistance;

	public float distanceLerpSpeed;

	public SpriteRenderer renderer;

	public bool inPool;
	public static List<Ammo> pool = new List<Ammo>();

	public void Initialize (Player _player, float _ammoAngle, float _targetDistance, float _length, float _width, Color _color) {

		renderer = GetComponent<SpriteRenderer>();
		renderer.enabled = true;
		inPool = false;
		pool.Remove(this);

		player = _player;
		ammoAngle = _ammoAngle;
		targetDistance = _targetDistance;
		renderer.color = _color;

		transform.localScale = new Vector3(_length,_width,1);

	}

	public static Ammo GetFromPool (GameObject fallback) {
		if (pool.Count > 0) {
			return pool[pool.Count - 1];
		} else {
			return ((GameObject)Instantiate(fallback)).GetComponent<Ammo>();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!inPool) {
			distance = Mathf.Lerp(distance,targetDistance,Time.deltaTime * distanceLerpSpeed);
			transform.position = player.transform.position + RadianToVector3((player.angle + ammoAngle) * Mathf.Deg2Rad) * distance;
			transform.eulerAngles = new Vector3(0,0,player.angle + ammoAngle);
		}

	}

	public void Discard () {
		inPool = true;
		pool.Add(this);
		renderer.enabled = false;
	}

	Vector3 RadianToVector3 (float radian) {
		return new Vector3(Mathf.Cos(radian),Mathf.Sin(radian));
	}

}
