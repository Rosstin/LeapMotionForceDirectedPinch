using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using Leap.Unity.PinchUtility;

public class HandsRaycast : MonoBehaviour {

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
	Node highlightedObjectR = null;

	int stateL;
	public GameObject leftPinchDetector;
	private LeapPinchDetector leftPinchDetectorScript;
	GameObject draggedObjectL = null;
	float distanceOfDraggedObjectL = 0.0f;
	float originalPinchDistanceL = 0.0f;
	Node highlightedObjectL = null;

	int STATE_NORMAL = 0;
	int STATE_DRAGGING = 1;

	int RIGHT = 0;
	int LEFT = 1;


	Vector3 nodeContainerStartPosition;
	Vector3 zoomPinchStartPositionL;
	Vector3 zoomPinchStartPositionR;
	float zoomPinchStartDistance;
	float lastZoomPinchDistance;

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
		HighlightNearPointFromPinch (leftPinchDetectorScript, LEFT);
		HighlightNearPointFromPinch (rightPinchDetectorScript, RIGHT);

		if (stateL == STATE_DRAGGING) {
			graphGenerator.explodeSelectedNode (highlightedObjectL);
		} 

		if (stateR == STATE_DRAGGING) {
			graphGenerator.explodeSelectedNode (highlightedObjectR);
		}

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

		int state = -1;
		if( handedness == RIGHT){
			state = stateR;
		}
		else{
			state = stateL;
		}

		if(state != STATE_DRAGGING && isActive){ // can start a drag
			state = STATE_DRAGGING;

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
				nodes [selectedNodeIndex].nodeForce.Selected ();
				originalPinchDistance = originalPinchDistanceR;
			} else {
				nodes [selectedNodeIndex].nodeForce.Selected ();
				originalPinchDistance = originalPinchDistanceL;
			}

			if (handedness == LEFT) {
				highlightedObjectL = nodes [selectedNodeIndex];
				highlightedObjectL.nodeForce.Selected ();
				//Debug.Log ("start highlightedObjectL.nodeForce.myTextMesh.text: " + highlightedObjectL.nodeForce.myTextMesh.text );
			}
			else {
				highlightedObjectR = nodes [selectedNodeIndex];
				highlightedObjectR.nodeForce.Selected ();
				//Debug.Log ("start highlightedObjectR.nodeForce.myTextMesh.text: " + highlightedObjectR.nodeForce.myTextMesh.text );
			}
		}

		if (state == STATE_DRAGGING) { // already dragging

			if (handedness == LEFT) {
				if (highlightedObjectL != null) {
					highlightedObjectL.nodeForce.timeSelected += Time.deltaTime;
				}
			} else {
				if (highlightedObjectR != null) {
					highlightedObjectR.nodeForce.timeSelected += Time.deltaTime;
				}
			}



		}

		if (!isActive) { // if you let go you're not dragging
			state = STATE_NORMAL;

			if (handedness == LEFT) {
				if (highlightedObjectL != null) {
					//Debug.Log ("letgo highlightedObjectL.nodeForce.myTextMesh.text: " + highlightedObjectL.nodeForce.myTextMesh.text );
					highlightedObjectL.nodeForce.Unselected ();
					graphGenerator.unselectNode ();
					highlightedObjectL.nodeForce.timeSelected = 0.0f;
					highlightedObjectL = null;
				}
			} else {
				if (highlightedObjectR != null) {
					//Debug.Log ("letgo highlightedObjectR.nodeForce.myTextMesh.text: " + highlightedObjectR.nodeForce.myTextMesh.text );
					highlightedObjectR.nodeForce.Unselected ();
					graphGenerator.unselectNode ();
					highlightedObjectR.nodeForce.timeSelected = 0.0f;
					highlightedObjectR = null;
				}
			}
		}

		if( handedness == RIGHT){
			stateR = state;
		}
		else{
			stateL = state;
		}
	}






}
