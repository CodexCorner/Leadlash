using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	public static Vector3 mousePosition;

	private new Camera camera;

	public Transform cursorTransform;

	public static float maxX; // if maxX < 14 then up down instead of left right


	// Use this for initialization
	void Start () {

		camera = GetComponent<Camera>();
		mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
		maxX = camera.ViewportToWorldPoint(new Vector3(1,0,0)).x;

	}

	// Update is called once per frame
	void Update () {

		mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
		cursorTransform.position = new Vector3(mousePosition.x,mousePosition.y,0);
		maxX = camera.ViewportToWorldPoint(new Vector3(1,0,0)).x;

	}
}
