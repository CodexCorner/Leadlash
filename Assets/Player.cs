using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static float timeScale;
	public static Vector3 position;

	public float speed;

	public float angle;
	public float spriteAngleOffset;

	public GameObject projectilePrefab;

	public float shootInterval;
	public int maxBullet;
	public float bulletReloadSpeed;
	public float bulletAngleSpread;

	public float ammoAngleSpread;
	public float ammoReadyDistance;
	public float ammoDistance;
	public float ammoSpentDistance;

	public Color ammoColor;
	public Color ammoLoadedColor;
	public Color ammoReadyColor;
	public Color ammoSpentColor;

	public int shotsPerBullet;

	public bool isReloading;

	public float bulletDamage;

	private float t;

	private int bulletCount;

	public GameObject ammoPrefab;

	public List<Ammo> ammos = new List<Ammo>();

	public float minimumTimeScale;
	public float maximumTimeScale;

	public float minimumAmmoWidth;
	public float maximumAmmoWidth;

	public float ammoLength;

	private Rigidbody2D rigidbody;

	public GameObject particlePrefab;

	public int particleCount;
	public float particleAngleSpread;
	public Range particleDuration;
	public Range particleLength;
	public float particleWidth;
	public Range particleSpeed;

	public LevelManager manager;

	public int deathParticleCount;
	public Range deathParticleDuration;
	public Range deathParticleLength;
	public float deathParticleWidth;
	public Range deathParticleSpeed;

	private SpriteRenderer sprite;

	public float positionReturnLerpSpeed;
	public float deathAngleSpinRate;

	private int currentReloading;

	public GameObject audioPrefab;

	public AudioClip deathSFX;
	public float deathVolume;
	public AudioClip reloadCompleteSFX;
	public float reloadCompleteVolume;


	void Start () {
		Initialize();
		Die();
	}

	public void Initialize () {

		t = 0;
		isReloading = false;
		bulletCount = maxBullet;

		ammoAngleSpread = 360 / maxBullet;
		float a = (-ammoAngleSpread/2) * (float)(maxBullet - 1);

		float c = Mathf.Clamp((2 * Mathf.PI * (ammoSpentDistance - ammoLength)) / maxBullet,minimumAmmoWidth,maximumAmmoWidth);

		for (int i = ammos.Count - 1; i >= maxBullet; i--) {
			ammos[i].Discard();
			ammos.RemoveAt(i);
		}

		for (int i = 0; i < maxBullet; i++) {
			if (ammos.Count - 1 < i) ammos.Add(Ammo.GetFromPool(ammoPrefab));
			ammos[i].Initialize(this,a + (ammoAngleSpread * i),ammoDistance,ammoLength,c,ammoColor);
		}

		timeScale = 1;

		rigidbody = GetComponent<Rigidbody2D>();

		sprite = GetComponent<SpriteRenderer>();
		sprite.enabled = true;

	}
	
	// Update is called once per frame
	void Update () {

		if (manager.playingGame) {
			rigidbody.velocity = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0).normalized * speed;

			angle = VectorAngle(GameCamera.mousePosition - transform.position);

			timeScale = Mathf.Lerp(minimumTimeScale,maximumTimeScale,(float)bulletCount/(float)maxBullet);

			if (Input.GetMouseButton(0)) {
				
				if (!isReloading && bulletCount > 0 && t >= 1) {

					for (int i = 0; i < shotsPerBullet; i++) {
						Projectile.GetFromPool(projectilePrefab).Initialize(transform.position,
							angle + Random.Range(-bulletAngleSpread,bulletAngleSpread),bulletDamage,Color.white);
					}

					for (int i = 0; i < particleCount; i++) {
						Particle.GetFromPool(particlePrefab).Initialize(transform.position,
							angle + Random.Range(-particleAngleSpread,particleAngleSpread),
							particleDuration.Get(Random.value),particleLength.Get(Random.value),particleWidth,particleSpeed.Get(Random.value));
					}
						
					ammos[maxBullet - bulletCount].targetDistance = ammoSpentDistance;
					ammos[maxBullet - bulletCount].renderer.color = ammoSpentColor;

					t = 0;
					bulletCount -= 1;

					if (bulletCount <= 0) {
						isReloading = true;
						AudioClipPlayer.Play(reloadCompleteSFX,AudioClipPlayer.GetRandomStep(),reloadCompleteVolume,audioPrefab);
					}
						
				}

			}

			if (!isReloading) {
				if (t < 1) {
					t += Time.deltaTime/shootInterval;
				}
				if (t >= 1) {
					ammos[maxBullet - bulletCount].targetDistance = ammoReadyDistance;
					ammos[maxBullet - bulletCount].renderer.color = ammoReadyColor;
				}
			} else {

				if (t < 1) {
					t += Time.deltaTime/bulletReloadSpeed;

					int cr = Mathf.RoundToInt(t * (maxBullet - 1));
					if (currentReloading != cr) {

						currentReloading = cr;

						ammos[currentReloading].targetDistance = ammoDistance;
						ammos[currentReloading].renderer.color = ammoLoadedColor;

					}
						
					timeScale = Mathf.Lerp(minimumTimeScale,maximumTimeScale,t);

				} else {

					for (int i = 0; i < maxBullet; i++) {
						ammos[i].targetDistance = ammoDistance;
						ammos[i].renderer.color = ammoColor;
					}

					AudioClipPlayer.Play(reloadCompleteSFX,AudioClipPlayer.GetRandomStep(),reloadCompleteVolume,audioPrefab);

					currentReloading = -1;

					bulletCount = maxBullet;
					t = 0;
					isReloading = false;
					timeScale = 2;
				}
			}

			transform.eulerAngles = new Vector3(0,0,angle + spriteAngleOffset);
			position = transform.position;
		
		} else {

			transform.position = Vector3.Lerp(transform.position,Vector3.zero,Time.deltaTime * positionReturnLerpSpeed);
			rigidbody.velocity = Vector2.zero;
			angle += deathAngleSpinRate * Time.deltaTime;

		}

	}

	float VectorAngle (Vector3 v) {
		return Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg;
	}

	public void OnTriggerEnter2D () {

		if (manager.playingGame) {
			
			Die();
		}

	}

	public void Die () {

		manager.playingGame = false;

		for (int i = 0; i < ammos.Count; i++) {
			ammos[i].targetDistance = 5;
			ammos[i].renderer.color = ammoColor;
		}

		float[] angledist = RandomDistribution(deathParticleCount);

		float ca = 0;

		for (int i = 0; i < deathParticleCount; i++) {

			ca += angledist[i] * 360;

			Particle.GetFromPool(particlePrefab).Initialize(transform.position,
				ca,
				deathParticleDuration.Get(Random.value),deathParticleLength.Get(Random.value),deathParticleWidth,deathParticleSpeed.Get(Random.value));

		}

		timeScale = 0.1f;
		sprite.enabled = false;

		AudioClipPlayer.Play(deathSFX,1,deathVolume,audioPrefab);

	}

	float[] RandomDistribution (int size) {

		float[] result = new float[size];
		float total = 0;

		for (int i = 0; i < size; i++) {
			result[i] = Random.value;
			total += result[i];
		}

		for (int i = 0; i < size; i++) {
			result[i] /= total;
		}

		return result;

	}
}
