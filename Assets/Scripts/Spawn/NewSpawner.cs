using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewSpawner : MonoBehaviour {

	private GameObject player;

	[Header("Grid")]
	public int width;
	public int height;
	public float squareWidth;

	[Header("Spawn")]
	public int iterations;

	[Header("Level")]
	public LevelConfig[] levelConfigs;
	private int currentLevelNum;

	[Space(10)]
	public float minWeaponSpawnScale;

	private SquareGrid squareGrid;

	private intVector2 playerPos;
	private intVector2 prevPlayerPos;
	
	private bool isAwake = false;

	private Transform objectsHolder;

	public void Awake () {
		player = GameObject.FindWithTag("Player");
		squareGrid = new SquareGrid (height, width, squareWidth, levelConfigs, minWeaponSpawnScale);
		playerPos = new intVector2(Mathf.RoundToInt(player.transform.position.x / squareGrid.squareWidth), Mathf.RoundToInt(player.transform.position.y / squareGrid.squareWidth));
		prevPlayerPos = playerPos;
		currentLevelNum = 0;
		isAwake = true;
		if (width % 2 != 1 || height % 2 != 1)
			Debug.LogError("width and height have to be odd numbers");
		objectsHolder = new GameObject ("Object Holder").transform;
	}

	public void Start () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (i != width / 2 || j != height / 2) {
					squareGrid.squares[i,j].iterationTimes += (squareGrid.squares[i,j].levelConfig.doSpawn) ? (iterations) : (0);
				}
			}
		}
	}

	public void Update () {
		playerPos = new intVector2 (Mathf.RoundToInt (player.transform.position.x / squareGrid.squareWidth),
		                            Mathf.RoundToInt (player.transform.position.y / squareGrid.squareWidth));

		if (prevPlayerPos.x - playerPos.x == -1)
			squareGrid.MoveGridRightAtX(iterations);
		if (prevPlayerPos.x - playerPos.x == 1)
			squareGrid.MoveGridLeftAtX(iterations);
		if (prevPlayerPos.y - playerPos.y == -1)
			squareGrid.MoveGridUpAtY(iterations);
		if (prevPlayerPos.y - playerPos.y == 1)
			squareGrid.MoveGridDownAtY(iterations);

		prevPlayerPos = playerPos;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (squareGrid.squares[i,j].iterationTimes > 0) 
					squareGrid.SpawnAtSquare (i ,j, objectsHolder);
			}
		}
	}

	public void OnDrawGizmos() {
		if (isAwake) {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					Gizmos.color = new Color (1f, 0f, 0f, 0.5f);
					if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
						Gizmos.color = new Color (1f, 1f, 1f, 0.25f);
					Gizmos.DrawCube(new Vector3(squareGrid.squares[i,j].x, squareGrid.squares[i,j].y, 1.5f) * Square.width, Vector3.one * (Square.width - 2f));
					foreach (SpawnObject spObj in squareGrid.squares[i,j].insideSpawnObjects){
						if (spObj.obj != null) {
							Gizmos.color = new Color (0f, 1f, 0f, 0.5f);
							Gizmos.DrawWireSphere(spObj.obj.transform.position, spObj.scale / 2f);
						}
					}
				}
			}
		}
	}


	// ------- *
	// Classes *
	// ------- *


	// ----------- *
	// Square Grid *
	// ----------- *


	public class SquareGrid {
		public Square[,] squares;

		public int width, height;
		public float squareWidth;

		public LevelConfig[] levelConfigs;
		public float minWeaponSpawnScale;

		private SpawnObject currentSpawnObject;
		private int objectType;						// 1 -- asteroid, 2 -- coin, 3 -- bonus
		private int prefabNum;

		public SquareGrid (int _height, int _width, float _squareWidth, LevelConfig[] _levelConfigs, float _minWeaponSpawnScale) {
			width = _width;
			height = _height;
			squareWidth = _squareWidth;
			squares = new Square[width, height];
			levelConfigs = _levelConfigs;
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					squares[i,j] = new Square(i - width / 2, j - height / 2, squareWidth, ChooseConfig(j - height / 2));
				}
			}
			minWeaponSpawnScale = _minWeaponSpawnScale;
		}

		// ----------- *
		// Grid Moving *

		public void MoveGridUpAtY(int iterations) {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					if (j == 0) {
						foreach (SpawnObject spObj in squares[i,j].insideSpawnObjects)
							DestroyObject(spObj.obj);
						squares[i,j].insideSpawnObjects.Clear();
					}
					if (j < height - 1) {
						squares[i,j] = squares[i,j+1];
					}
					if (j == height - 1) {
						Square oldSquare = squares[i,j];
						Square newSquare = new Square(oldSquare.x, oldSquare.y + 1, Square.width, ChooseConfig(oldSquare.y + 1));
						newSquare.iterationTimes += (newSquare.levelConfig.doSpawn) ? (iterations) : (0);
						squares[i,j] = newSquare;
					}
				}
			}
		}

		public void MoveGridDownAtY(int iterations) {
			for (int i = 0; i < width; i++) {
				for (int j = height - 1; j >= 0; j--) {
					if (j == height - 1) {
						foreach (SpawnObject spObj in squares[i,j].insideSpawnObjects)
							DestroyObject(spObj.obj);
						squares[i,j].insideSpawnObjects.Clear();
					}
					if (j > 0) {
						squares[i,j] = squares[i,j-1];
					}
					if (j == 0) {
						Square oldSquare = squares[i,j];
						Square newSquare = new Square(oldSquare.x, oldSquare.y - 1, Square.width, ChooseConfig(oldSquare.y - 1));
						newSquare.iterationTimes += (newSquare.levelConfig.doSpawn) ? (iterations) : (0);
						squares[i,j] = newSquare;
					}
				}
			}
		}

		public void MoveGridRightAtX(int iterations) {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					if (i == 0) {
						foreach (SpawnObject spObj in squares[i,j].insideSpawnObjects)
							DestroyObject(spObj.obj);
						squares[i,j].insideSpawnObjects.Clear();
					}
					if (i < width - 1) {
						squares[i,j] = squares[i+1,j];
					}
					if (i == width - 1) {
						Square oldSquare = squares[i,j];
						Square newSquare = new Square(oldSquare.x + 1, oldSquare.y, Square.width, oldSquare.levelConfig);
						newSquare.iterationTimes += (newSquare.levelConfig.doSpawn) ? (iterations) : (0);
						squares[i,j] = newSquare;
					}
				}
			}
		}

		public void MoveGridLeftAtX(int iterations) {
			for (int i = width - 1; i >= 0; i--) {
				for (int j = 0; j < height; j++) {
					if (i == width - 1) {
						foreach (SpawnObject spObj in squares[i,j].insideSpawnObjects) 
							DestroyObject(spObj.obj);
						squares[i,j].insideSpawnObjects.Clear();
					}
					if (i > 0) {
						squares[i,j] = squares[i-1,j];
					}
					if (i == 0) {
						Square oldSquare = squares[i,j];
						Square newSquare = new Square(oldSquare.x - 1, oldSquare.y, Square.width, oldSquare.levelConfig);
						newSquare.iterationTimes += (newSquare.levelConfig.doSpawn) ? (iterations) : (0);
						squares[i,j] = newSquare;
					}
				}
			}
		}

		// ----------- *

		public void SpawnAtSquare (int i, int j, Transform objectsHolder) {
			currentSpawnObject = new SpawnObject();
			TakeRandomObject (squares[i,j].levelConfig);
			if (objectType == 1) {
				AsteroidPrefab asteroid = squares[i,j].levelConfig.asteroids[prefabNum];
				if (SpawnTheObject (currentSpawnObject, i, j, asteroid.minScale, asteroid.maxScale, objectType, objectsHolder) && currentSpawnObject.scale >= minWeaponSpawnScale) //make min weapon spaw scale
					SpawnWeapon(squares[i,j].levelConfig.weapons);
			} else if (objectType == 2) {
				PickUpPrefab coin = squares[i,j].levelConfig.coins[prefabNum];
				SpawnTheObject (currentSpawnObject, i, j, coin.minScale, coin.minScale, objectType, objectsHolder);
			} else if (objectType == 3) {
				PickUpPrefab bonus = squares[i,j].levelConfig.bonuses[prefabNum];
				SpawnTheObject (currentSpawnObject, i, j, bonus.minScale, bonus.minScale, objectType, objectsHolder);
			}
			squares[i,j].iterationTimes --;
		}

		private bool SpawnTheObject (SpawnObject spawnObject ,int x, int y, float minScale, float maxScale, int objectType, Transform parentTrans) {
			Vector3 position = squares[x,y].RandomPosition();
			bool sutable = true;
			float maxPossibleScale = 10000f;

			for (int i = x - 1; i <= x + 1; i++) {
				for (int j = y - 1; j <= y + 1; j++) {
					if (i >= 0 && j >= 0 && i < width && j < height) {
						float newMaxPossibleScale = squares[i,j].MaxPossibleScale(position, spawnObject);
						if (newMaxPossibleScale < maxPossibleScale){
							maxPossibleScale = newMaxPossibleScale;
						}
					}
				}
			}

			sutable = maxPossibleScale > minScale;

			if (sutable) {
				if (objectType == 1)
					SpawnNewASteroid(spawnObject, position, maxScale, maxPossibleScale);
				else if (objectType == 2 || objectType == 3)
					SpawnNewPickUp(spawnObject, position, minScale);
				squares[x,y].insideSpawnObjects.Add(spawnObject);
				spawnObject.obj.transform.SetParent (parentTrans);
			}

			return sutable;
		}

		private void TakeRandomObject(LevelConfig levelConfig) {
			int intChance = Random.Range(0, 3);
			
			if (intChance == 0) {
				currentSpawnObject = RandomPrefab <AsteroidPrefab> (levelConfig.asteroids, levelConfig.minDistBetweenAsteroids);
				objectType = 1;
			}
			else if (intChance == 1) {
				currentSpawnObject = RandomPrefab <PickUpPrefab> (levelConfig.coins, levelConfig.minDistBetweenCoins);
				objectType = 2;
			}
			else {
				currentSpawnObject = RandomPrefab <PickUpPrefab> (levelConfig.bonuses, levelConfig.minDistBetweenBonuses);
				objectType = 3;
			}
		}
		
		private LevelConfig ChooseConfig (int height) {
			int heightOfsset = 0;
			
			for (int i = 0; i < levelConfigs.Length; i ++) {
			if (height < levelConfigs[i].levelScale + heightOfsset) {
					return levelConfigs[i];
				}
				else 
					heightOfsset += levelConfigs[i].levelScale;
			}
			
			return levelConfigs[0];
		}
		
		private SpawnObject RandomPrefab <PrefabType> (PrefabType[] spawnPrefabs, float minDistBetween)
			where PrefabType : PickUpPrefab {

			float chance = 0f;
			foreach (PrefabType spPref in spawnPrefabs) {
				chance += spPref.chance;
			}
			chance = Random.Range(0f, chance);
			
			for (int i = 0; i < spawnPrefabs.Length; i++) {
				if (spawnPrefabs[i].chance >= chance){
					prefabNum = i;
					return new SpawnObject(spawnPrefabs[i].obj, minDistBetween);
				}
				else
					chance -= spawnPrefabs[i].chance;
			}
			
			return new SpawnObject();
		}

		private void SpawnNewASteroid(SpawnObject spawnObject, Vector3 position, float maxScale, float maxPossibleScale) {
			GameObject obj;
			float scale;
			obj = Instantiate(spawnObject.obj, position, Quaternion.identity) as GameObject;
			scale = ((maxPossibleScale < maxScale) ? (maxPossibleScale) : (maxScale));
			obj.transform.localScale *= scale;
			obj.GetComponent<Rigidbody2D>().mass = scale * scale;
			obj.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			obj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1000f, 1000f) * scale * scale);
			spawnObject.obj = obj;
			spawnObject.scale = scale;
			spawnObject.obj.transform.position += (Vector3) (Random.insideUnitCircle * spawnObject.distBetween);
		}

		private void SpawnWeapon (WeaponPrefab[] weapons){
			float chance = Random.Range(0f, 100f);
			float offsetChance = 0f;
			GameObject weapon = null;
			for (int i = 0; i <  weapons.Length; i++){
				offsetChance += weapons[i].chance;
				if (offsetChance >= chance){
					weapon = weapons[i].obj;
					break;
				}
			}
			
			if (weapon != null){
				Vector3 position = WeaponSpawnPosition(currentSpawnObject.obj);
				GameObject currentWeapon = Instantiate(weapon, position, Quaternion.LookRotation(currentSpawnObject.obj.transform.position - position)) as GameObject;
				currentWeapon.transform.SetParent(currentSpawnObject.obj.transform);
				currentWeapon.transform.up = currentWeapon.transform.position - currentSpawnObject.obj.transform.position;
			}
		}
		
		private Vector3 WeaponSpawnPosition(GameObject asteroid) {
			float radius = currentSpawnObject.scale / 2f + 0.1f;
			float angle = Random.Range(0f, 2f * Mathf.PI);
			Vector3 rayStartPos = new Vector3(radius * Mathf.Sin(angle), radius * Mathf.Cos(angle), 0f) + asteroid.transform.position;
			RaycastHit2D asteroidHit;
			Vector3 SpawnPosition = Vector3.zero;
			
			Debug.DrawRay(rayStartPos, asteroid.transform.position - rayStartPos, Color.red, 10f);
			
			asteroidHit = Physics2D.Raycast (rayStartPos, asteroid.transform.position - rayStartPos);
			if (asteroidHit.transform != null)
				if (asteroidHit.transform.tag == "Asteroid")
					SpawnPosition = asteroidHit.point;
			
			return SpawnPosition;
		}

		private void SpawnNewPickUp(SpawnObject spawnObject, Vector3 position, float scale) {
			spawnObject.obj = Instantiate(spawnObject.obj, position, Quaternion.identity) as GameObject;
			spawnObject.scale = scale;
			spawnObject.obj.transform.localScale *= spawnObject.scale;
		}

	}


	// ------ *
	// Square *
	// ------ *


	public class Square {

		public int x, y;
		public static float width;
		public Vector3 centerPos;

		public List<SpawnObject> insideSpawnObjects;
		public int iterationTimes;
		public LevelConfig levelConfig;

		public Square(int _x, int _y, float _width, LevelConfig _levelConfig) {
			x = _x;
			y = _y;
			width = _width;
			centerPos = new Vector2 (x * width, y * width);
			insideSpawnObjects = new List<SpawnObject>();
			levelConfig =  _levelConfig;
			iterationTimes = 0;
		}

		public Vector3 RandomPosition() {
			return new Vector3(Random.Range(-width / 2f, width / 2f),
			                   Random.Range(-width / 2f, width / 2f), 0f) + centerPos;
		}

		public float MaxPossibleScale(Vector3 pos, SpawnObject spawnObject) {
			float maxPossibleScale = 10000f;

			foreach (SpawnObject spObj in insideSpawnObjects) {
				if (spObj.obj != null) {
					if ((Vector3.Magnitude(pos - spObj.obj.transform.position) -
					     ((spObj.scale / 2f) + ((spawnObject.distBetween < spObj.distBetween) ? (spawnObject.distBetween):(spObj.distBetween)))) < maxPossibleScale) {
							maxPossibleScale = (Vector3.Magnitude(pos - spObj.obj.transform.position) -
											   ((spObj.scale / 2f) + ((spawnObject.distBetween < spObj.distBetween) ? (spawnObject.distBetween):(spObj.distBetween))));
					}
				}
			}

			return maxPossibleScale * 2f;
		}

	}


	// ------- *
	// Structs *
	// ------- *


	[System.Serializable]
	public class LevelConfig {

		[Header("Scale and doSpawn")]
		public int levelScale;
		public bool doSpawn;

		[Header("Asteroids & Weapons")]
		public AsteroidPrefab[] asteroids;
		public WeaponPrefab[] weapons;

		[Header("PickUp")]
		public PickUpPrefab[] coins;
		public PickUpPrefab[] bonuses;

		[Header("Distances")]
		public float minDistBetweenAsteroids;
		public float minDistBetweenCoins;
		public float minDistBetweenBonuses;
		
	}

	public class SpawnObject {
		public GameObject obj;
		public float scale;
		public float distBetween;

		public SpawnObject() {
			obj = null;
			scale = 1f;
			distBetween = 0f;
		}

		public SpawnObject(GameObject _obj){
			obj = _obj;
			scale = 1f;
			distBetween = 0f;
		}

		public SpawnObject(GameObject _obj, float _distBetween){
			obj = _obj;
			scale = 0f;
			distBetween = _distBetween;
		}

	}

	[System.Serializable]
	public class WeaponPrefab {
		public GameObject obj;

		[Range(0f, 100f)]
		public float chance;

	}

	[System.Serializable]
	public class PickUpPrefab {
		public GameObject obj;

		[Range(0f, 100f)]
		public float chance;

		public float minScale;

	}

	[System.Serializable]
	public class AsteroidPrefab : PickUpPrefab {
		public float maxScale;

	}

	public struct intVector2 {
		public int x, y;

		public intVector2(int _x, int _y) {
			x= _x;
			y = _y;
		}
	}
}
