using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

	public float size;

	public float speed;

	private SpriteRenderer sprite;
	private Collider2D collider;
	private Vector3 unit;
	private float angle;

	public bool inPool;

	public float poolingDistance;

	public static List<EnemyBullet> pool = new List<EnemyBullet>();
	public static List<EnemyBullet> all = new List<EnemyBullet>();
	public bool inAll;

	public bool isDespawning;
	public AnimationCurve despawnAnimation;
	public float animationDuration;

	public LevelManager manager;

	private float t;

	// Use this for initialization
	public void Initialize (LevelManager _manager, Vector3 pos, float a) {

		manager = _manager;

		if (sprite == null) sprite = GetComponent<SpriteRenderer>();
		sprite.enabled = true;
		angle = a;
		unit = AngleVector(angle);

		if (collider == null) collider = GetComponent<Collider2D>();
		collider.enabled = true;

		transform.position = new Vector3(pos.x,pos.y,0);
		transform.localScale = new Vector3(size,size,1);
		transform.eulerAngles = Vector3.forward * a;

		isDespawning = false;
		inPool = false;
		pool.Remove(this);

		if (!inAll) {
			inAll = true;
			all.Add(this);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!isDespawning) {
			
			if (!inPool) {
				transform.position += unit * speed * Time.deltaTime * Player.timeScale;

				if (transform.position.x > poolingDistance || transform.position.x < -poolingDistance ||
					transform.position.y > poolingDistance || transform.position.y < -poolingDistance) {

					inPool = true;
					pool.Add(this);
					sprite.enabled = false;
					collider.enabled = false;

				}
				sprite.color = LevelManager.foregroundColor;

			}

		} else {

			if (!inPool) {
				t -= (Time.deltaTime/animationDuration) * Player.timeScale;
				transform.localScale = Vector3.one * size * despawnAnimation.Evaluate(t);

				sprite.color = LevelManager.foregroundColor;

				if (t <= 0) {
					inPool = true;
					pool.Add(this);
					sprite.enabled = false;
					transform.position = new Vector3(poolingDistance,poolingDistance);
				}
			}

		}

	}

	public void Discard () {
		
		if (!inPool) {
			isDespawning = true;
			t = 1;
			collider.enabled = false;
			pool.Remove(this);
		}

	}

	public static EnemyBullet GetFromPool (GameObject fallback) {
		if (pool.Count > 0) {
			return pool[pool.Count - 1];
		} else {
			return ((GameObject)Instantiate(fallback)).GetComponent<EnemyBullet>();
		}
	}

	Vector3 AngleVector (float d) {
		return new Vector3(Mathf.Cos(d * Mathf.Deg2Rad),Mathf.Sin(d * Mathf.Deg2Rad),0);
	}
}
