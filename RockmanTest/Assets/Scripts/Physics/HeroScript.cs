using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomHeroScript))]
public class HeroScript : MonoBehaviour {

	public Vector2 NatureGravity = new Vector2 (0, -0.983f);
	public int FixedTimeMulti = 100;

	public Vector2 CounterGravity = new Vector2(0, 0);

	public CustomHeroScript CustomHeroScript;

	private Vector2 currentVelocity = new Vector2(0.0f, 0.0f);

	private bool doubleJumpEnabled = true;

	private InputInfo inputInfo;

	private int dashCounter = 0;
	private bool dashable = true;
	private int dashableCoolDownCounter = 0;

	private int hoverCounter = 0;
	private bool hoverable = true;
	private int hoverCoolDownCounter = 0;

	// Use this for initialization
	private void Start () {
		
	}

	private void processInput() {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			inputInfo.Left = true;
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			inputInfo.Left = false;
		}

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			inputInfo.Right = true;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			inputInfo.Right = false;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			inputInfo.Up = true;
		} 
		else if (Input.GetKeyUp(KeyCode.UpArrow)) {
//			inputInfo.Up = false;
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			inputInfo.C = true;
		} 
		else if (Input.GetKeyUp(KeyCode.C)) {
			//			inputInfo.Up = false;
		}

//		if (Input.GetKeyDown (KeyCode.DownArrow)) {
//			inputInfo.Down = true;
//		}
//		else if (Input.GetKeyUp (KeyCode.DownArrow)) {
//			inputInfo.Down = false;
//		}
	}

	private void Update() {
		processInput ();

		if(Input.GetKeyDown(KeyCode.X) && dashable) {
			inputInfo.X = true;
			dashCounter = 0;
			dashable = false;
		}

		if (inputInfo.X) {
			dashCounter++;
			if (dashCounter > 20) {
				inputInfo.X = false;
			}
		}

		if (!dashable) {
			dashableCoolDownCounter++;
			if (dashableCoolDownCounter > 120) {
				dashable = true;
				dashableCoolDownCounter = 0;
			}
		}

		if (Input.GetKeyDown (KeyCode.Z)) {
			if (CustomHeroScript.collisionInfo.collideBottom) {
				currentVelocity.y = 0.3f;
			} else if (doubleJumpEnabled) {
				currentVelocity.y = 0.3f;
				doubleJumpEnabled = false;	
			}
		}
		else if (CustomHeroScript.collisionInfo.collideBottom) {
			currentVelocity.y = 0;
			doubleJumpEnabled = true;
		}

		if (CustomHeroScript.collisionInfo.collideRightCount > 1 && inputInfo.Right) {
			print ("Stick right");
			CounterGravity = NatureGravity * -1;
			currentVelocity.y = -0.05f;
		} else {
			CounterGravity.Set (0, 0);
		}

		if (hoverable) {
			
		}

		if (hoverable && Input.GetKeyDown (KeyCode.C)) {
			inputInfo.C = true;
			hoverCounter = 0;
			hoverable = false;
		}

		if (inputInfo.C) {
			CounterGravity = NatureGravity * -0.8f;
			currentVelocity.y = -0.05f;

			hoverCounter++;
			if (hoverCounter > 120) {
				inputInfo.C = false;
			}
		}

		if (!hoverable) {
			hoverCoolDownCounter++;
			if (hoverCoolDownCounter > 180) {
				hoverable = true;
				hoverCoolDownCounter = 0;
			}
		}

		if (CustomHeroScript.collisionInfo.collideLeftCount > 1 && inputInfo.Left) {
			print ("Stick left");
			CounterGravity = NatureGravity * -1;
			currentVelocity.y = -0.05f;
		} else {
			CounterGravity.Set (0, 0);
		}

		calculateMovementVector (inputInfo, ref currentVelocity);

	}

	private void FixedUpdate() {

		currentVelocity += (NatureGravity + CounterGravity) / FixedTimeMulti;

		CustomHeroScript.Move (currentVelocity);
	}

	private void calculateMovementVector(InputInfo inputInfo, ref Vector2 movementVector) {

		movementVector.x = 0;

		if(inputInfo.Left && !inputInfo.X) {
			movementVector.x = -0.2f;	
		}

		if (inputInfo.Right && !inputInfo.X) {
			movementVector.x = 0.2f;
		}

		if (inputInfo.Left && inputInfo.X) {
			movementVector.x = -0.4f;
		}

		if (inputInfo.Right && inputInfo.X) {
			movementVector.x = 0.4f;
		}
	}

	struct InputInfo {
		public bool Up, Down, Left, Right;
		public bool X, Z, C;

		public void Reset() {
			Up = Down = Left = Right = false;
			X = Z = C = false;
		}
	}
}