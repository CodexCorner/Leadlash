using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public List<Enemy> enemiesLeft = new List<Enemy>();

	public static float totalShot;
	public static float hits;

	public float baseDifficulty;
	public float[] escalationDifficulty;
	public float proceedDifficultyRate;

	public float maxDifficulty;

	private int levelCount;
	private int escalationSetCount;
	private float currentDifficulty;

	public Range enemyTypes;
	public Range enemyCount;
	public Range enemySpeed;
	public Range enemyShootInterval;
	public Range enemyShotsPerBullet;
	public Range enemyBulletArc;
	public Range enemyHealth;

	public GameObject enemyPrefab;
	public Player player;

	public bool playingGame;

	public float masterVolume;

	public static Color foregroundColor;
	public static Color backgroundColor;

	private Color newForegroundColor;
	private Color newBackgroundColor;

	public float colorLerpSpeed;

	public SpriteRenderer cursorMiddleRenderer;

	public Transform titleScreenTransform;
	public float textLerpSpeed;

	public Transform scoreScreenTransform;
	public float textTargetDistance;
	public float sideMinimumCameraWidth;

	public TMPro.TextMeshPro levelTextMesh;
	public TMPro.TextMeshPro accuracyTextMesh;

	public bool firstTime;

	private float t;

	public float deathLockScreenDuration;

	// Use this for initialization
	void Start () {

		escalationSetCount = escalationDifficulty.Length;
		GenerateNewColorSet();
		backgroundColor = Color.white;
		foregroundColor = Color.white;

		firstTime = true;

		Cursor.visible = false;

	}

	void GenerateNewColorSet () {

		newBackgroundColor = new Color(Random.Range(0,0.5f),Random.Range(0,0.5f),Random.Range(0,0.5f));
		newForegroundColor = new Color(1 - newBackgroundColor.r,1 - newBackgroundColor.g,1 - newBackgroundColor.b);

		newBackgroundColor = Color.Lerp(newBackgroundColor,Color.black,0.2f);
		newForegroundColor = Color.Lerp(newForegroundColor,Color.white,0.2f);

	}

	void InitiateGame () {

		for (int i = 0; i < enemiesLeft.Count; i++) {
			enemiesLeft[i].state = 2;
		}

		for (int i = 0; i < EnemyBullet.all.Count; i++) {
			EnemyBullet.all[i].Discard();
		}

		player.Initialize();
		playingGame = true;

		levelCount = 0;
		currentDifficulty = 0;
		ProceedToNextLevel();

		Cursor.visible = false;

		firstTime = false;

		hits = 0;
		totalShot = 0;

		t = 0;

	}

	void ProceedToNextLevel () {

		Cursor.visible = false;

		levelCount++;
		if (levelCount % escalationSetCount == 0 && currentDifficulty < maxDifficulty) {
			currentDifficulty += proceedDifficultyRate;
			if (currentDifficulty > maxDifficulty) {
				currentDifficulty = maxDifficulty;
			}
			GenerateNewColorSet();
		}

		levelTextMesh.text = "level " + levelCount;
			
		float diff = currentDifficulty + escalationDifficulty[levelCount % escalationSetCount];

		float[] dist = RandomDistribution(3);

		for (int i = 0; i < 3; i++) {
			dist[i] *= diff;
		}

		//Debug.Log(diff);

		int _enemyTypes = Mathf.RoundToInt(enemyTypes.Get(dist[0]));
		EnemyData[] datas = new EnemyData[_enemyTypes];

		for (int i = 0; i < _enemyTypes; i++) {

			float[] dist2 = RandomDistribution(2); // defense vs offense

			float _health = enemyHealth.Get(dist2[0] * dist[1]);

			if (Random.value < 0.5f) {

				datas[i] = new EnemyData(enemySpeed.Get(dist2[0] * dist[1]),1,0,0,_health);

			} else {

				float[] dist3 = RandomDistribution(3);

				float arc = enemyBulletArc.Get(dist3[2] * dist[1]);
				int shot = 1 + Mathf.RoundToInt(enemyShotsPerBullet.Get(dist3[1] * dist[1]));

				datas[i] = new EnemyData(0,enemyShootInterval.Get(dist3[0] * dist[1]), 
					shot,arc / shot,_health);

			}

		}

		int _enemyCount = Mathf.RoundToInt(enemyCount.Get(dist[2]));

		for (int i = 0; i < _enemyCount; i++) {

			Enemy.GetFromPool(enemyPrefab).Initialize(this,datas[Random.Range(0,_enemyTypes)],new Vector3(Random.Range(-7f,7f),Random.Range(-7f,7f),0),0);

		}


	}
	
	// Update is called once per frame
	void Update () {

		AudioListener.volume = masterVolume;

		if (playingGame) {

			if (GameCamera.maxX < sideMinimumCameraWidth) {

				titleScreenTransform.position = Vector3.Lerp(titleScreenTransform.position,new Vector3(0,-textTargetDistance,0),Time.deltaTime * textLerpSpeed);
				scoreScreenTransform.position = Vector3.Lerp(scoreScreenTransform.position,new Vector3(0,textTargetDistance,0),Time.deltaTime * textLerpSpeed);

			} else {

				titleScreenTransform.position = Vector3.Lerp(titleScreenTransform.position,new Vector3(-textTargetDistance,0,0),Time.deltaTime * textLerpSpeed);
				scoreScreenTransform.position = Vector3.Lerp(scoreScreenTransform.position,new Vector3(textTargetDistance,0,0),Time.deltaTime * textLerpSpeed);

			}

			if (enemiesLeft.Count <= 0) {
				ProceedToNextLevel();
			}

			if (totalShot <= 0) {
				accuracyTextMesh.text = "N/A";
			} else {
				accuracyTextMesh.text = Mathf.RoundToInt((hits/totalShot) * 100f) + "% accuracy";
			}

		} else {

			if (!firstTime) {
				scoreScreenTransform.position = Vector3.Lerp(scoreScreenTransform.position,Vector3.zero,Time.deltaTime * textLerpSpeed);
			}

			if (t > deathLockScreenDuration) {
				if (Input.anyKeyDown) {
					InitiateGame();
					t = 0;
					playingGame = true;
				}
			} else {
				t += Time.deltaTime;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		backgroundColor = Color.Lerp(backgroundColor,newBackgroundColor,Time.deltaTime * colorLerpSpeed);
		Camera.main.backgroundColor = backgroundColor;
		cursorMiddleRenderer.color = backgroundColor;
		foregroundColor = Color.Lerp(foregroundColor,newForegroundColor,Time.deltaTime * colorLerpSpeed);

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
