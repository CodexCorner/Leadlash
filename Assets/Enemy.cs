using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData {

	public float speed;
	public float shootInterval;
	public int shotsPerBullet;
	public float anglePerBullet;
	public float health;

	public EnemyData (float _speed, float _shootInterval, int _shotsPerBullet, float _anglePerBullet, float _health) {

		speed = _speed;
		shootInterval = _shootInterval;
		shotsPerBullet = _shotsPerBullet;
		anglePerBullet = _anglePerBullet;
		health = _health;

	}

}

public class Enemy : MonoBehaviour {
	
	public float speed;

	public float angle;
	public float spriteAngleOffset;

	public GameObject enemyBulletPrefab;

	public float shootInterval;

	public int timesShot;
	public int shotPerSound;

	public int shotsPerBullet;
	private float bulletHalfSpread;
	public float anglePerBullet;

	private float t;

	public float health;

	public GameObject audioPrefab;
	public AudioClip shootSFX;
	public float shootVolume;

	public AudioClip deathSFX;
	public float deathVolume;

	public bool inPool;
	public static List<Enemy> pool = new List<Enemy>();

	private SpriteRenderer sprite;
	private Collider2D collider;

	private LevelManager manager;

	public int state;
	public AnimationCurve spawnDeathAnimation;
	public float animationDuration;

	public float size;

	public static Enemy GetFromPool (GameObject fallback) {
		if (pool.Count > 0) {
			return pool[pool.Count - 1];
		} else {
			return ((GameObject)Instantiate(fallback)).GetComponent<Enemy>();
		}
	}

	public void Initialize (LevelManager _manager, EnemyData data, Vector3 position, float _angle) {

		manager = _manager;
		speed = data.speed;
		angle = _angle;
		shootInterval = data.shootInterval;
		shotsPerBullet = data.shotsPerBullet;
		anglePerBullet = data.anglePerBullet;
		health = data.health;

		transform.position = position;

		if (sprite == null) sprite = GetComponent<SpriteRenderer>();
		sprite.enabled = true;

		bulletHalfSpread = (-anglePerBullet/2) * (float)(shotsPerBullet - 1);
		t = 0;

		if (collider == null) collider = GetComponent<Collider2D>();
		collider.enabled = true;

		inPool = false;
		pool.Remove(this);
		manager.enemiesLeft.Add(this);
		state = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (!inPool) {

			sprite.color = LevelManager.foregroundColor;
			
			if (state == 0) {

				Vector3 velocity = RadianToVector3(angle * Mathf.Deg2Rad) * speed * Player.timeScale;

				angle = VectorAngle(Player.position - transform.position);

				transform.position += velocity * Time.deltaTime;

				if (shotsPerBullet > 0 && t >= 1) {

					for (int i = 0; i < shotsPerBullet; i++) {
						EnemyBullet.GetFromPool(enemyBulletPrefab).Initialize(manager,transform.position,
							angle + bulletHalfSpread + (anglePerBullet * i));
					}

					if (timesShot % shotPerSound == 0) {
						AudioClipPlayer.Play(shootSFX,AudioClipPlayer.GetRandomStep(),shootVolume,audioPrefab);
					}

					t = 0;
					timesShot++;

				}

				if (t < 1) {
					t += (Time.deltaTime/shootInterval) * Player.timeScale;
				}

				transform.eulerAngles = new Vector3(0,0,angle + spriteAngleOffset);

				if (health < 0) {
					AudioClipPlayer.Play(deathSFX,AudioClipPlayer.GetRandomStep(),deathVolume,audioPrefab);
					t = 1;
					state = 2;
				}

			} else if (state == 1) {

				if (t < 1) {
					t += (Time.deltaTime/animationDuration);
					transform.localScale = Vector3.one * (size * spawnDeathAnimation.Evaluate(t));
				} else {
					collider.enabled = true;
					state = 0;
					t = 0;
					transform.localScale = Vector3.one * size;
				}
				
			} else if (state == 2) {

				collider.enabled = false;

				if (t > 0) {
					t -= (Time.deltaTime/animationDuration);
					transform.localScale = Vector3.one * (size * spawnDeathAnimation.Evaluate(t));
				} else {
					inPool = true;
					pool.Add(this);
					manager.enemiesLeft.Remove(this);
					sprite.enabled = false;
					transform.localScale = new Vector3(0,0,1);
				}

			}
		}


	}

	float VectorAngle (Vector3 v) {
		return Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg;
	}

	Vector3 RadianToVector3 (float radian) {
		return new Vector3(Mathf.Cos(radian),Mathf.Sin(radian));
	}
}
