using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using Leap.Unity.PinchUtility;

public class MouseRaycast : MonoBehaviour {

	// the screen aspect ratio matters for the vert and horiz, you'll need to recognize that
	// use "force pull" / "force push" to bring objects closer or farther
	// weight it towards objects closer to your face

	public Camera playerCamera;

	public GameObject pointer;

	public Canvas canvas;

	public LineRenderer myLineRenderer;

	public GameObject sceneGod;
	GenerateRandomGraph graphGenerator;

	Node[] nodes;

	int stateR;
	public GameObject rightPinchDetector;
	private LeapPinchDetector rightPinchDetectorScript;
	GameObject draggedObjectR = null;
	float distanceOfDraggedObjectR = 0.0f;
	float originalPinchDistanceR = 0.0f;

	int stateL;
	public GameObject leftPinchDetector;
	private LeapPinchDetector leftPinchDetectorScript;
	GameObject draggedObjectL = null;
	float distanceOfDraggedObjectL = 0.0f;
	float originalPinchDistanceL = 0.0f;

	int STATE_NORMAL = 0;
	int STATE_DRAGGING = 1;

	int RIGHT = 0;
	int LEFT = 1;

	void Start () {
		GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
		myLineRenderer.enabled = false;

		graphGenerator = sceneGod.GetComponent<GenerateRandomGraph> ();
		nodes = graphGenerator.masterNodeList;

		rightPinchDetectorScript = rightPinchDetector.GetComponent<LeapPinchDetector> ();
		leftPinchDetectorScript = leftPinchDetector.GetComponent<LeapPinchDetector> ();

	}

	// Attach this script to an orthographic camera.


	void FixedUpdate () {

		//MousePointerUpdate ();
		//LeapHandsPointerUpdate ();

		for (int i = 0; i < nodes.Length; i++) {
			nodes [i].nodeForce.RevertColor ();
			nodes [i].nodeForce.TextFaceCamera (playerCamera.transform);
		}
  
		ConeCastPointsFromPinch(rightPinchDetectorScript, RIGHT);
		ConeCastPointsFromPinch(leftPinchDetectorScript, LEFT);
	}

	void ConeCastPointsFromPinch(LeapPinchDetector detector, int handedness) {
		// GET ACTIVITY -- are you pinching, clicking?
		bool isActive = detector.IsPinching;
		bool activeThisFrame = detector.DidStartPinch;

		// GET POSITION OF EVENT
		Vector3 p = detector.Position;
		// camera to pinch vector
		Vector3 heading = Vector3.Normalize(p - playerCamera.transform.position);

		// camera to object vector
		Vector3 objectVector;
		float biggestDotProduct= 0.0f;
		int selectedNodeIndex = 0;
		float dotProduct;
		for (int i = 0; i < nodes.Length; i++) {
			objectVector = Vector3.Normalize(nodes [i].gameObject.transform.position - playerCamera.transform.position);
			dotProduct = Vector3.Dot (heading, objectVector);

			if (dotProduct > biggestDotProduct) {
				biggestDotProduct = dotProduct;
				selectedNodeIndex = i;
			}
		}


		GameObject draggedObject = null;
		float distanceOfDraggedObject = 0.0f;
		float originalPinchDistance = 0.0f;

		if (handedness == RIGHT) {
			nodes [selectedNodeIndex].gameObject.GetComponent<MeshRenderer> ().material.color = Color.red; //getting component is slow
			originalPinchDistance = originalPinchDistanceR;
		} else {
			nodes [selectedNodeIndex].gameObject.GetComponent<MeshRenderer> ().material.color = Color.green;
			originalPinchDistance = originalPinchDistanceL;
		}

		int state = -1;
		if( handedness == RIGHT){
			state = stateR;
			distanceOfDraggedObject = distanceOfDraggedObjectR;
			draggedObject = draggedObjectR;
		}
		else{
			state = stateL;
			distanceOfDraggedObject = distanceOfDraggedObjectL;
			draggedObject = draggedObjectL;
		}

		if(state != STATE_DRAGGING && isActive){ // can start a drag
			state = STATE_DRAGGING;
			draggedObject = nodes[selectedNodeIndex].gameObject;
			distanceOfDraggedObject = Vector3.Distance (playerCamera.transform.position, draggedObject.transform.position);
			originalPinchDistance = Vector3.Distance (playerCamera.transform.position, p);
		}

		if (state == STATE_DRAGGING) { // already dragging
			Vector3 direction = heading / distanceOfDraggedObject;

			float currentPinchDistance = Vector3.Distance (playerCamera.transform.position, p);

			if (currentPinchDistance < originalPinchDistance * 0.90f ) {
				// bring the object closer
				distanceOfDraggedObject = distanceOfDraggedObject * 0.99f;
			} else if (currentPinchDistance > originalPinchDistance * 1.10f ) {
				distanceOfDraggedObject = distanceOfDraggedObject * 1.01f;
			} else {
				// do nothing
			}

			Vector3 objectPosition = playerCamera.transform.position + (direction.normalized * distanceOfDraggedObject);
			draggedObject.transform.position = objectPosition;
		}

		if (!isActive) { // if you let go you're not dragging
			state = STATE_NORMAL;
		}

		if( handedness == RIGHT){
			stateR = state;
			distanceOfDraggedObjectR = distanceOfDraggedObject;
			draggedObjectR = draggedObject;
			originalPinchDistanceR = originalPinchDistance;
		}
		else{
			stateL = state;
			distanceOfDraggedObjectL = distanceOfDraggedObject;
			draggedObjectL = draggedObject;
			originalPinchDistanceL = originalPinchDistance;
		}

		Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 100.0f);

		myLineRenderer.SetVertexCount (2);
		myLineRenderer.SetPosition (0, p);
		myLineRenderer.SetPosition (1, endRayPosition);
		myLineRenderer.enabled = true;

	}


}
