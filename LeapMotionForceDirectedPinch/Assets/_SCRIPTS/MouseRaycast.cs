using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using Leap.Unity.PinchUtility;

public class MouseRaycast : MonoBehaviour {

	// the screen aspect ratio matters for the vert and horiz, you'll need to recognize that
	// use "force pull" / "force push" to bring objects closer or farther

	public Camera playerCamera;

	public GameObject pointer;

	public Canvas canvas;

	public LineRenderer myLineRenderer;

	//public GameObject rightHandObject;
	//public CapsuleHand rightHandScript;
	//private Transform pointerFingerTip;

	public GameObject rightPinchDetector;
	private LeapPinchDetector rightPinchDetectorScript;

	int state;
	int STATE_NORMAL = 0;
	int STATE_DRAGGING = 1;

	GameObject draggedObject = null;
	float distanceOfDraggedObject = 0.0f;

	//private Transform objectToMove;     // The object we will move.
	//private Vector3 offSet;       // The object's position relative to the mouse position.
	//private float dist;

	void Start () {
		GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
		myLineRenderer.enabled = false;
		//rightHandScript = rightHandObject.GetComponent<CapsuleHand> ();
		//GameObject prefabNode = Resources.Load ("Node") as GameObject;

		rightPinchDetectorScript = rightPinchDetector.GetComponent<LeapPinchDetector> ();

	}

	// Attach this script to an orthographic camera.


	void FixedUpdate () {

		//MousePointerUpdate ();
		LeapHandsPointerUpdate ();

	}

	void LeapHandsPointerUpdate () {

		// GET ACTIVITY -- are you pinching, clicking?
		bool isActive = rightPinchDetectorScript.IsPinching;
		bool activeThisFrame = rightPinchDetectorScript.DidStartPinch;

		// GET POSITION OF EVENT
		Vector3 p = rightPinchDetectorScript.Position;
		//Vector3 p = pointerFingerTip.position;
		//Vector3 p = globalMousePos;

		Vector3 heading = p - playerCamera.transform.position;

		// RAY STUFF

		RaycastHit hit = new RaycastHit ();


		//if (Physics.Raycast (playerCamera.transform.position, p, out hit)) { // if you hit something
		if (Physics.SphereCast (playerCamera.transform.position, 12.0f, heading, out hit, 200.0f)) {
			if (hit.transform.gameObject.tag == "Draggable") { // if it was a draggable object
				hit.transform.gameObject.GetComponent<Draggable> ().OnHit ();
				//if (Input.GetMouseButtonDown (0)) {
				if(activeThisFrame){ // if you're pinching/clicking
					state = STATE_DRAGGING;
					draggedObject = hit.transform.gameObject;
					distanceOfDraggedObject = Vector3.Distance (playerCamera.transform.position, hit.transform.gameObject.transform.position);
				}
			}
		}


		if (state == STATE_DRAGGING) {
			Vector3 direction = heading / distanceOfDraggedObject;
			Vector3 objectPosition = playerCamera.transform.position + (direction.normalized * distanceOfDraggedObject);
			draggedObject.transform.position = objectPosition;
		}

		if (!isActive) {
			state = STATE_NORMAL;
		}

		Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 100.0f);

		myLineRenderer.SetVertexCount (2);
		myLineRenderer.SetPosition (0, p);
		myLineRenderer.SetPosition (1, endRayPosition);
		myLineRenderer.enabled = true;
	
	}

	void MousePointerUpdate () {
		// MOUSE POINTER STUFF
		/*
		var rt = pointer.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), new Vector2(Input.mousePosition.x, Input.mousePosition.y), playerCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
		}

		if (Input.GetMouseButton(0)){
			//Debug.Log ("mouse being held");
			pointer.GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("pointerSpriteRed");
		} else {
			pointer.GetComponent<UnityEngine.UI.Image> ().sprite = Resources.Load<Sprite> ("pointerSprite");
		}
		*/
	}


}
