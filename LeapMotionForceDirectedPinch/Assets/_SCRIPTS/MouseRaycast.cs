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
	int STATE_NODESELECTED = 2;

	int RIGHT = 0;
	int LEFT = 1;

	int stateZoom;
	int STATE_ZOOM_NEUTRAL = 0;
	int STATE_ZOOM_BEGIN = 1;

	// STATE FOR NODE SELECTION THING

	int nstate;
	int NSTATE_NORMAL = 0;
	int NSTATE_NODESELECTED = 1;

	GameObject highlightedObject = null;


	Vector3 nodeContainerStartPosition;
	Vector3 zoomPinchStartPositionL;
	Vector3 zoomPinchStartPositionR;
	float zoomPinchStartDistance;
	float lastZoomPinchDistance;

	float ZOOM_CONSTANT = 10.0f;

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
			//nodes [i].nodeForce.TextFaceCamera (playerCamera.transform);
		}
  
		HighlightNearPointFromPinch (rightPinchDetectorScript, RIGHT);
		HighlightNearPointFromPinch (leftPinchDetectorScript, LEFT);
	
		//pinchActions ();

		//ConeCastPointsFromPinch(rightPinchDetectorScript, RIGHT);
		//ConeCastPointsFromPinch(leftPinchDetectorScript, LEFT);
	}

	void pinchActions() {

		// if you're pinching but state_zoom_begin hasn't happened, put yourself in STATE_ZOOM_BEGIN
		if (stateZoom != STATE_ZOOM_BEGIN && rightPinchDetectorScript.IsPinching && leftPinchDetectorScript.IsPinching) {
			stateZoom = STATE_ZOOM_BEGIN;
			nodeContainerStartPosition = graphGenerator.nodeContainer.transform.position;
			zoomPinchStartPositionL = leftPinchDetectorScript.Position;
			zoomPinchStartPositionR = rightPinchDetectorScript.Position;
			zoomPinchStartDistance = Vector3.Distance(zoomPinchStartPositionL, zoomPinchStartPositionR);
		}

		// exiting STATE_ZOOM_BEGIN because you stopped pinching... record the position of the graph, that's the new neutral
		if (stateZoom == STATE_ZOOM_BEGIN && (!rightPinchDetectorScript.IsPinching || !leftPinchDetectorScript.IsPinching)) {
			stateZoom = STATE_ZOOM_NEUTRAL;
		}

		// we might want to lock the player into zooming or rotating

		if (stateZoom == STATE_ZOOM_BEGIN) {
			rotateAndZoomGraph ();
		}


	}

	void rotateAndZoomGraph() {

		Vector3 pr = rightPinchDetectorScript.Position;
		Vector3 pl = leftPinchDetectorScript.Position;

		float currentPinchDistance = Vector3.Distance (pr, pl);

		//print ("currentPinchDistance: " + currentPinchDistance + "... zoomPinchStartDistance: " + zoomPinchStartDistance);

		// make a position based on starting and current pinch positions
		graphGenerator.nodeContainer.transform.position = 
			new Vector3(
				nodeContainerStartPosition.x, 
				nodeContainerStartPosition.y, 
				nodeContainerStartPosition.z - ZOOM_CONSTANT*(currentPinchDistance-zoomPinchStartDistance)
			);

		//lastZoomPinchDistance = currentPinchDistance;

		//print ("graphGenerator.nodeContainer.transform.position: " + graphGenerator.nodeContainer.transform.position + "... nodeContainerStartPosition: " + nodeContainerStartPosition);

		// if both are pinching and the distance between pinches is increasing or decreasing, zoom in or out




		// if both are pinching and one is rotating around the other, rotate the dude




	}

	void HighlightNearPointFromPinch(LeapPinchDetector detector, int handedness) {
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

		GameObject highlightedObject = null;
		float distanceOfDraggedObject = 0.0f;
		float originalPinchDistance = 0.0f;

		if (handedness == RIGHT) {
			nodes [selectedNodeIndex].nodeForce.SetColor (Color.red);
			originalPinchDistance = originalPinchDistanceR;
		} else {
			nodes [selectedNodeIndex].nodeForce.SetColor (Color.green);
			originalPinchDistance = originalPinchDistanceL;
		}

		int state = -1;
		if( handedness == RIGHT){
			state = stateR;
			distanceOfDraggedObject = distanceOfDraggedObjectR;
		}
		else{
			state = stateL;
			distanceOfDraggedObject = distanceOfDraggedObjectL;
		}


		//STATE STUFF
		if (nstate != NSTATE_NODESELECTED && isActive) { // select the node
			nstate = STATE_NODESELECTED;
			highlightedObject = nodes [selectedNodeIndex].gameObject;
		}

		if (state == NSTATE_NODESELECTED) { // already selected
			//highlightedObject; //increase time selected
		}

		if (!isActive) { // if you let go you're not dragging
			nstate = NSTATE_NORMAL;
		}

		if( handedness == RIGHT){
			stateR = state;
			distanceOfDraggedObjectR = distanceOfDraggedObject;
			originalPinchDistanceR = originalPinchDistance;
		}
		else{
			stateL = state;
			distanceOfDraggedObjectL = distanceOfDraggedObject;
			originalPinchDistanceL = originalPinchDistance;
		}

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
