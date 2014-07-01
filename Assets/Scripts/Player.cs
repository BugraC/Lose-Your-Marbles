using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
		private Vector3 startPos;
		private Vector3 direction;
		private bool objectChosenForYAxis;
		private bool objectChosenForXAxis;
		private Vector3 gameObjectStartPosition;
		public int speed = 100;
		// This value calibrates the touch range of the object. If you give this range a big value then one touch can choose two marbles at one touch began action which we don't want that.
		public int touchSensitivity = 20;
		public float marbleMovementSensitivity = 0.1f;
		
		
		//These are my private variables:
		private Vector3 smoothVectorGoal = Vector3.zero;
		private int returnDirection;
		private float touchYPosition;
		private float touchXPosition;
		private enum whichDirection
		{
				None,
				UpOrDown,
				LeftOrRight
		}

		private static whichDirection _WhichDirection;

		// Use this for initialization
		void Start ()
		{
				_WhichDirection = whichDirection.None;
		}
#if DEBUG
	void OnGUI(){
		//This is for debug purposes:
		//GUI.Label (new Rect (0, 0, 300, 300), startPos.x.ToString () + " " + startPos.y.ToString () + " " +startPos.z.ToString () + " "+ direction.x + " " + direction.y +" " + direction.z);
		//GUI.Label (new Rect (0, 0, 300, 300), message);
	}
#endif
		// Update is called once per frame
		void Update ()
		{
				
				// Track a single touch as a direction control.
				if (Input.touchCount > 0) {
						var touch = Input.GetTouch (0);
			
						// Handle finger movements based on touch phase.
						switch (touch.phase) {
						// Record initial touch position.
						case TouchPhase.Began:
								smoothVectorGoal = Vector3.zero;
								startPos = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 1f));
								touchYPosition = touch.position.y;
								touchXPosition = touch.position.x;
								//Let's get the start position and then we will subtract this value with the touch move value.
								gameObjectStartPosition = gameObject.transform.position;
								
								
								objectChosenForYAxis = (touch.position.x >= Camera.main.WorldToScreenPoint (gameObject.transform.position).x - touchSensitivity
										&& touch.position.x <= Camera.main.WorldToScreenPoint (gameObject.transform.position).x + touchSensitivity);

								objectChosenForXAxis = (touch.position.y >= Camera.main.pixelHeight / 2 - touchSensitivity
										&& touch.position.y <= Camera.main.pixelHeight / 2 + touchSensitivity 
										&& touch.position.y >= Camera.main.WorldToScreenPoint (gameObject.transform.position).y - touchSensitivity
										&& touch.position.y <= Camera.main.WorldToScreenPoint (gameObject.transform.position).y + touchSensitivity);
								
								break;
						// Determine direction by comparing the current touch
						// position with the initial one.
						case TouchPhase.Moved:

								//After getting touch start position, we will subtract this value with the current touch position.
								//direction variable will give you the necessary movement pixels.
								//after adding the gameObjectStartPosition value with this direction you get the right movement.
								direction = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 1f)) - startPos;
								if ((_WhichDirection == whichDirection.None || _WhichDirection == whichDirection.UpOrDown) 
										&& objectChosenForYAxis && Mathf.Abs (touchYPosition - touch.position.y) > Mathf.Abs (touchXPosition - touch.position.x)) {

										_WhichDirection = whichDirection.UpOrDown;
										gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObjectStartPosition.z + direction.z);
										gameObject.transform.Rotate (touch.deltaPosition.y * speed * Time.deltaTime, 0, 0, Space.World);
										returnDirection = touch.deltaPosition.y <= 0 ? -1 : 1;
								} else if ((_WhichDirection == whichDirection.None || _WhichDirection == whichDirection.LeftOrRight) 
										&& objectChosenForXAxis && Mathf.Abs (touchYPosition - touch.position.y) < Mathf.Abs (touchXPosition - touch.position.x)) {
										returnDirection = touch.deltaPosition.x <= 0 ? 1 : -1;
										_WhichDirection = whichDirection.LeftOrRight;

										gameObject.transform.position = new Vector3 (gameObjectStartPosition.x + direction.x, gameObject.transform.position.y, gameObject.transform.position.z);
										gameObject.transform.Rotate (0, 0, touch.deltaPosition.x * speed * Time.deltaTime, Space.World);
								}
								break;
						// Report that a direction has been chosen when the finger is lifted.
						case TouchPhase.Ended:
								 
								if (objectChosenForYAxis) {
										objectChosenForYAxis = false;
										
										smoothVectorGoal = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, Mathf.CeilToInt (gameObject.transform.position.z + direction.z));
										StartCoroutine (SetLocation (smoothVectorGoal, returnDirection, _WhichDirection));

										
								} else if (objectChosenForXAxis) {
										objectChosenForXAxis = false;
					
										smoothVectorGoal = new Vector3 (Mathf.CeilToInt (gameObjectStartPosition.x + direction.x), gameObject.transform.position.y, gameObject.transform.position.z);

										//gameObject.transform.position = smoothVectorGoal;
										StartCoroutine (SetLocation (smoothVectorGoal, returnDirection, _WhichDirection));
								}
							
								break;
						}
				}
		}

		bool DirectionAppoximately (Func<bool> firstQuery, Func<bool> secondQuery, whichDirection _whichDirection)
		{
				if (_whichDirection == whichDirection.UpOrDown) {
			
						return firstQuery ();
				} else if (_whichDirection == whichDirection.LeftOrRight) {
						return secondQuery ();
				} else {
			Debug.Log ("well that happened");
						return true;
				}
		
		}

		//This method is for setting smooth movement after ending the touch:
		IEnumerator SetLocation (Vector3 toReach, int rotateDirection, whichDirection _whichDirection)
		{

				//Let's give a smooth movement while you are moving your object by your touches:
				while (toReach != Vector3.zero &&  
		       DirectionAppoximately( 
		                      	() => !Mathf.Approximately (gameObject.transform.position.z, toReach.z), 
								()=> !Mathf.Approximately (gameObject.transform.position.x, toReach.x)
									,_whichDirection)) {
						gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, toReach, marbleMovementSensitivity);
						if (smoothVectorGoal == Vector3.zero) {
								break;
						}

						if (
				DirectionAppoximately (
				() => !Mathf.Approximately (Mathf.RoundToInt (gameObject.transform.position.z * 10), Mathf.RoundToInt (toReach.z * 10)), 
				() => !Mathf.Approximately (Mathf.RoundToInt (gameObject.transform.position.x * 10), Mathf.RoundToInt (toReach.x * 10)), _whichDirection)) { 
								if (_whichDirection == whichDirection.UpOrDown) {
										gameObject.transform.Rotate (returnDirection * speed * Time.deltaTime, 0, 0, Space.World);
								} else if (_whichDirection == whichDirection.LeftOrRight) {
										gameObject.transform.Rotate (0, 0, returnDirection * speed * Time.deltaTime, Space.World);
					
								}
								
						}
						if (DirectionAppoximately (
				() => Mathf.Approximately (gameObject.transform.position.z, toReach.z),
				() => Mathf.Approximately (gameObject.transform.position.x, toReach.x), _whichDirection)) {
								//At the set the precise position to avoid misalignment:
								gameObject.transform.position = toReach;
								_WhichDirection = whichDirection.None;
						}
						yield return new WaitForSeconds (0.0001f);

				}
				
				if (DirectionAppoximately (
			() => !Mathf.Approximately (gameObject.transform.position.z, toReach.z),
			() => !Mathf.Approximately (gameObject.transform.position.x, toReach.x), _whichDirection)) {
						//At the end set the precise position to avoid misalignment:
						gameObject.transform.position = toReach;
						_WhichDirection = whichDirection.None;
				}

				
		}

	

}
