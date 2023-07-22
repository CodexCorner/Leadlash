using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

	public static List<Particle> pool = new List<Particle>();
	public bool inPool;

	public float increment;
	public float life;

	public float length;
	public float width;

	public Vector3 velocity;

	public SpriteRenderer sprite;

	public static Particle GetFromPool (GameObject fallback) {
		if (pool.Count > 0) {
			return pool[pool.Count - 1];
		} else {
			return ((GameObject)Instantiate(fallback)).GetComponent<Particle>();
		}
	}

	public void Initialize (Vector3 _position, float _angle, float _duration, float _length, float _width, float _speed) {

		inPool = false;
		pool.Remove(this);
		life = 0;

		length = _length;
		width = _width;

		Vector3 unit = RadianToVector3(_angle * Mathf.Deg2Rad); 
		velocity = unit * _speed;

		transform.position = _position + (unit * (length/2));
		transform.eulerAngles = new Vector3(0,0,_angle);
		increment = 1/_duration;

		transform.localScale = new Vector3(length,width,1);

		if (sprite == null) sprite = GetComponent<SpriteRenderer>();
		sprite.enabled = true;

	}

	void Update () {

		if (!inPool) {

			life += increment * Time.deltaTime;
			transform.localScale = new Vector3(length * (1 - life),width * (1 - life),1);
			transform.position += velocity * Time.deltaTime;
			sprite.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

			if (life >= 1) {
				
				pool.Add(this);
				inPool = true;

				sprite.enabled = false;

			}

		}

	}

	Vector3 RadianToVector3 (float radian) {
		return new Vector3(Mathf.Cos(radian),Mathf.Sin(radian));
	}
}
