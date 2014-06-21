using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
		private Vector3 startPos;
		private Vector3 direction;
		private bool directionChosen;
		private Vector3 gameObjectStartPosition;
		public int speed = 100;
		// This value calibrates the touch range of the object. If you give this range a big value then one touch can choose two marbles at one touch began action which we don't want that.
		public int touchSensitivity = 20;
		// Use this for initialization
		void Start ()
		{
	
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
								startPos = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 1.273157f));
								
								gameObjectStartPosition = gameObject.transform.position;
								//Debug.Log (Camera.main.WorldToScreenPoint (gameObject.transform.position).x  + ":" +  touch.position.x);
								
								directionChosen = (touch.position.x >= Camera.main.WorldToScreenPoint (gameObject.transform.position).x -touchSensitivity
				                   && touch.position.x <= Camera.main.WorldToScreenPoint (gameObject.transform.position).x+touchSensitivity);
								
								break;
						// Determine direction by comparing the current touch
						// position with the initial one.
						case TouchPhase.Moved:
								if (directionChosen) {
										direction = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 1.273157f)) - startPos;
										gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObjectStartPosition.z + direction.z);
										gameObject.transform.Rotate (touch.deltaPosition.y * speed * Time.deltaTime, 0, 0, Space.World);
										//Debug.Log (touch.deltaPosition.y);
								}
								break;
						// Report that a direction has been chosen when the finger is lifted.
						case TouchPhase.Ended:
								if (directionChosen) {
									directionChosen = true;
									gameObjectStartPosition = gameObject.transform.position;
								}
								break;
						}
				}
		}
}
