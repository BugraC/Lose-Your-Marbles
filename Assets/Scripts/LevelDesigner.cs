using UnityEngine;
using System.Collections;

//The way this level designer works is that
//Splits the screen into squares and put each item into
//those squares randomly.
public class LevelDesigner : MonoBehaviour
{
		//Let's create the objects in a static way
		//So we don't have to worry about the reflection and texture setup.
		//There might be a better solution but I believe this is the best for me.
		public GameObject marble1;
		public GameObject marble2;
		public GameObject marble3;
		public GameObject marble4;
		//Here our screen margins for the setup:
		public int topMargin = 40;
		public int bottomMargin = 40;
		public int leftMargin = 60;
		public int rightMargin = 40;
		public int squareSize = 40;
		public float scaleSize = 0.8f;
		//This is the floor Y value itself. If you increase this and the objects will on the air. The opposite will be underground.	
		public float constantFloorYCordinate = 0.1274259f;
		private Random rnd = new Random ();
		// Use this for initialization
		void Start ()
		{
				//Ok let's create our objects:
				GameObject marble = null;
				for (int i = leftMargin; i < Camera.main.pixelWidth - rightMargin; i+= squareSize) {
						for (int j = topMargin; j< Camera.main.pixelHeight - bottomMargin; j+= squareSize) {
								switch (Random.Range (1, 5)) {
								case 1:
										marble = (GameObject)GameObject.Instantiate (marble1);
										break;
								case 2:
										marble = (GameObject)GameObject.Instantiate (marble2);
										break;
								case 3:
										marble = (GameObject)GameObject.Instantiate (marble3);
										break;
								case 4:
										marble = (GameObject)GameObject.Instantiate (marble4);
										break;
								default:
										break;
								}

								if (marble != null) {
										//Let's reduce the size of the elements. They are too big.
										marble.transform.localScale *= scaleSize;
										Vector3 marblePosition = Camera.main.ScreenToWorldPoint (new Vector3 (i, j, 0));
										marble.transform.position = new Vector3 (marblePosition.x, marblePosition.y, Mathf.CeilToInt (marblePosition.z));
										//Let's set the y ordinate in real world:
										marble.transform.position = new Vector3 (marble.transform.position.x, constantFloorYCordinate, marble.transform.position.z);
								}

						}
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
