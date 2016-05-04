using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseRaycast : MonoBehaviour {

	// the screen aspect ratio matters for the vert and horiz, you'll need to recognize that

	public Camera playerCamera;

	public GameObject pointer;

	//public GameObject marker;
	//public GameObject MRScreen;

	public Canvas canvas;

	private Transform objectToMove;     // The object we will move.
	private Vector3 offSet;       // The object's position relative to the mouse position.
	private float dist;


	void Start () {
	}

	// Attach this script to an orthographic camera.

	void FixedUpdate () {

		var rt = pointer.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), new Vector2(Input.mousePosition.x, Input.mousePosition.y), playerCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
		}

		if (Input.GetMouseButton(0)){
			Debug.Log ("mouse being held");
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("pointerSpriteRed");
		} else {
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("pointerSprite");
		}


	}
}
