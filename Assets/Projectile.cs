using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float width;

	public float speed;

	private SpriteRenderer sprite;
	public LayerMask mask;
	private Vector3 unit;
	private float angle;

	public bool inPool;
	public static List<Projectile> pool = new List<Projectile>();

	private Vector3 endPosition;
	private float length;

	public GameObject audioPrefab;
	public AudioClip hitSFX;
	public float audioVolume;

	public GameObject particlePrefab;

	public int particleCount;
	public float particleAngleSpread;
	public Range particleDuration;
	public Range particleLength;
	public float particleWidth;
	public Range particleSpeed;

	// Use this for initialization
	public void Initialize (Vector3 pos, float a, float damage, Color color) {

		angle = a;
		unit = AngleVector(angle);

		RaycastHit2D hit = Physics2D.Raycast(pos,unit,Mathf.Infinity,mask);
		if (hit) {

			if (sprite == null) sprite = GetComponent<SpriteRenderer>();
			sprite.enabled = true;
			sprite.color = color;

			inPool = false;
			pool.Remove(this);

			length = hit.distance;
			endPosition = hit.point;

			transform.position = endPosition - (unit * (length/2));
			transform.localScale = new Vector3(length,width,1);
			transform.eulerAngles = new Vector3(0,0,angle);

			LevelManager.totalShot++;

			Enemy target = hit.collider.GetComponent<Enemy>();
			if (target) {
				target.health -= damage;
				AudioClipPlayer.Play(hitSFX,AudioClipPlayer.GetRandomStep(),audioVolume,audioPrefab);
				LevelManager.hits++;
			} else {
				AudioClipPlayer.Play(hitSFX,AudioClipPlayer.GetRandomStep() - 12,audioVolume,audioPrefab);
			}

			for (int i = 0; i < particleCount; i++) {
				Particle.GetFromPool(particlePrefab).Initialize(hit.point,
					180 + angle + Random.Range(-particleAngleSpread,particleAngleSpread),
					particleDuration.Get(Random.value),particleLength.Get(Random.value),particleWidth,particleSpeed.Get(Random.value));
			}

		} else return;

	}
	
	// Update is called once per frame
	void Update () {

		if (!inPool) {
			
			length -= speed * Time.deltaTime;

			if (length < 0) {
				inPool = true;
				pool.Add(this);
				sprite.enabled = false;
			} else {
				transform.position = endPosition - (unit * (length/2));
				transform.localScale = new Vector3(length,width,1);
			}

		}
		
	}

	public static Projectile GetFromPool (GameObject fallback) {
		if (pool.Count > 0) {
			return pool[pool.Count - 1];
		} else {
			return ((GameObject)Instantiate(fallback)).GetComponent<Projectile>();
		}
	}

	Vector3 AngleVector (float d) {
		return new Vector3(Mathf.Cos(d * Mathf.Deg2Rad),Mathf.Sin(d * Mathf.Deg2Rad),0);
	}
}
