using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using Leap.Unity.PinchUtility;

public class HandsRaycast : MonoBehaviour {

	public GameObject button1;
	Collider button1Collider;
	public GameObject slider1;
	Collider slider1Collider;
	Slider slider1script;
	public GameObject PanelContainer;
	// an object with an array of all buttons should be included 

	float SLIDER_MOVE_SPEED = 0.004f;

	// VIEWPANEL
	float VIEWPANEL_EULER_X_LOWER_THRESHHOLD = 10.0f;
	float VIEWPANEL_EULER_X_UPPER_THRESHHOLD = 100.0f;

	int panelState;
	int PANEL_ON = 0;
	float turnPanelOffTimer = 0.0f;
	int PANEL_OFF = 1;
	float turnPanelOnTimer = 0.0f;

	float PANEL_ON_TIMER_CONSTANT = 0.5f;
	float PANEL_OFF_TIMER_CONSTANT = 2.0f;

	public Camera playerCamera; // aka CenterEyeAnchor

	LineRenderer myLineRenderer;

	public GameObject sceneGod;
	GenerateGraph graphGenerator;

	//Node[] nodes;

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

		graphGenerator = sceneGod.GetComponent<GenerateGraph> ();

		rightPinchDetectorScript = rightPinchDetector.GetComponent<LeapPinchDetector> ();
		leftPinchDetectorScript = leftPinchDetector.GetComponent<LeapPinchDetector> ();

		button1Collider = button1.GetComponent<Collider> ();
		slider1Collider = slider1.GetComponent<Collider> ();
		slider1script = slider1.GetComponent<Slider> ();


	}

	void FixedUpdate () {
		UpdateControlPanel ();

		HandlePinches (leftPinchDetectorScript, LEFT);
		HandlePinches (rightPinchDetectorScript, RIGHT);

		if (stateL == STATE_DRAGGING) { // maybe do this if the user stops moving the node around, don't do it if the node is moving a lot
			graphGenerator.explodeSelectedNode (highlightedObjectL);
		} 

		if (stateR == STATE_DRAGGING) {
			graphGenerator.explodeSelectedNode (highlightedObjectR);
		}

	}

    void NeutralizeButtonState() // neutralize state of all buttons in when you leave
    {
        NeutralizeSliderState(slider1script);
    }

	void UpdateControlPanel () {
		// looking at panel
		// not looking at panel


		if (panelState == PANEL_ON) {
			PanelContainer.SetActive (true);

			if (!(playerCamera.transform.eulerAngles.x >= VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= VIEWPANEL_EULER_X_UPPER_THRESHHOLD)) {
				turnPanelOnTimer = 0.0f;
				turnPanelOffTimer += Time.deltaTime;

				// if you're pinching, don't turn the panel off quite yet
				if (!(rightPinchDetectorScript.IsPinching || leftPinchDetectorScript.IsPinching) && turnPanelOffTimer >= PANEL_OFF_TIMER_CONSTANT) {
					panelState = PANEL_OFF;

                    NeutralizeButtonState();
                }
			}

		} else if (panelState == PANEL_OFF) {

            PanelContainer.SetActive (false);

			if (playerCamera.transform.eulerAngles.x >= VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= VIEWPANEL_EULER_X_UPPER_THRESHHOLD) {
				turnPanelOffTimer = 0.0f;
				turnPanelOnTimer += Time.deltaTime;

				if (turnPanelOnTimer >= PANEL_ON_TIMER_CONSTANT) {
					PanelContainer.transform.eulerAngles = new Vector3( PanelContainer.transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, PanelContainer.transform.eulerAngles.z ); 
					panelState = PANEL_ON;
				}

			} 


		}



	}

    void NeutralizeSliderState(Slider slider)
    {
        slider.state = slider.NORMAL;
        graphGenerator.showNodesOfDegreeGreaterThan(slider.currentValue);
        slider.UnGrab();
    }

    void UpdateSliderState(Collider collider, Slider slider, Ray ray, Vector3 heading, Vector3 p, bool isActive, bool activeThisFrame, int handedness){ // updating for both hands is screwing it up

		RaycastHit hit = new RaycastHit ();

		if (slider.state != slider.DRAGGING && isActive && collider.Raycast (ray, out hit, 200.0f)) { // start a drag
			slider.state = slider.DRAGGING;
			slider.OnGrab ();
			slider.handUsed = handedness;

			// do things related to starting a drag

		}

		if (slider.state == slider.DRAGGING && slider.handUsed == handedness) {

			Vector3 perp = Vector3.Cross (heading, (playerCamera.transform.position - slider.transform.position));
			float dir = Vector3.Dot (perp, playerCamera.transform.up);

			if (dir > 0f) {
				// left?
				//Debug.Log("left");

				slider.transform.localPosition = new Vector3( slider.transform.localPosition.x + SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);



			} else if (dir < 0f) {
				// right?
				//Debug.Log("right");

				slider.transform.localPosition = new Vector3( slider.transform.localPosition.x - SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);


			} else {
				// ontarget?
				Debug.Log("ontarget");
			}

			if (slider.UpdateBarValue ()) { // value changed
				graphGenerator.showNodesOfDegreeGreaterThan (slider.currentValue);
			}
				
			if (!isActive) { // no longer dragging
                NeutralizeSliderState(slider);
            }

		}






	}

	void HandlePinches(LeapPinchDetector detector, int handedness) {
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

        // maybe do something so these don't get registered multiple times at once

		if (Input.GetKeyDown ("d")) {
			print ("graphGenerator.detailingMode = true");
			graphGenerator.detailingMode = true;
		}

		if (Input.GetKeyDown ("f")) {
			print ("graphGenerator.detailingMode = false");
			graphGenerator.detailingMode = false;
		}


        if(Input.GetKeyDown("2"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
        }

        if (Input.GetKeyDown("3"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
        }

        /*
        if (Input.GetKeyDown("2"))
        {
            print("graphGenerator.generateGraphFromCSV as GRAPH_2D");
            graphGenerator.destroyOldGraph();
            graphGenerator.generateGraphFromCSV("b3_node", "b3_edgelist", GenerateGraph.GRAPH_2D);
        }

        if (Input.GetKeyDown("3"))
        {
            print("graphGenerator.generateGraphFromCSV as GenerateGraph.GRAPH_3D");
            graphGenerator.destroyOldGraph();
            graphGenerator.generateGraphFromCSV("b3_node", "b3_edgelist", GenerateGraph.GRAPH_3D);
        }
        */


        if (panelState == PANEL_ON) {

			// do panel actions
			graphGenerator.NodesAreDraggable (false);

			RaycastHit hit = new RaycastHit ();
			Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 100.0f);

			//myLineRenderer.SetVertexCount (2);
			//myLineRenderer.SetPosition (0, p);
			//myLineRenderer.SetPosition (1, endRayPosition);
			//myLineRenderer.enabled = true;

			Ray myRay = new Ray (playerCamera.transform.position, heading);

			if ( button1Collider.Raycast (myRay, out hit, 200.0f)) { // if you hit something

				//Debug.Log("Hit something.");
				if (hit.transform.gameObject.tag == "Clickable") { // if it was a button //don't really need this anymore
					//Debug.Log("Hit Clickable.");

					//hit.transform.gameObject.GetComponent<ButtonActivate> ().OnHit ();
					//graphGenerator.showNodesOfDegreeGreaterThan (22);

					graphGenerator.detailingMode = !graphGenerator.detailingMode;

					if (graphGenerator.detailingMode == false) {
						hit.transform.gameObject.GetComponent<ButtonActivate> ().OnHit ();
					}
					else {
						hit.transform.gameObject.GetComponent<ButtonActivate> ().UnHit ();
					}



				}
			}

    		UpdateSliderState (slider1Collider, slider1script, myRay, heading, p, isActive, activeThisFrame, handedness);



        } else { // not looking at panel

			graphGenerator.NodesAreDraggable (true);

			if (state != STATE_DRAGGING && isActive) { // can start a drag
				state = STATE_DRAGGING;

				for (int i = 0; i < graphGenerator.masterNodeList.Length; i++) {
					if (graphGenerator.masterNodeList[i].nodeForce.degree > graphGenerator.NodeDegree) {
						objectVector = Vector3.Normalize (graphGenerator.masterNodeList[i].gameObject.transform.position - playerCamera.transform.position);
						dotProduct = Vector3.Dot (heading, objectVector);

						if (dotProduct > biggestDotProduct) { // dont select nodes that are not visible
							biggestDotProduct = dotProduct;
							selectedNodeIndex = i;
						}
					}
				}

				GameObject draggedObject = null;
				float distanceOfDraggedObject = 0.0f;
				float originalPinchDistance = 0.0f;

				if (handedness == RIGHT) {
                    graphGenerator.masterNodeList[selectedNodeIndex].nodeForce.Selected ();
					originalPinchDistance = originalPinchDistanceR;
				} else {
                    graphGenerator.masterNodeList[selectedNodeIndex].nodeForce.Selected ();
					originalPinchDistance = originalPinchDistanceL;
				}

				if (handedness == LEFT) {
					highlightedObjectL = graphGenerator.masterNodeList[selectedNodeIndex];
					highlightedObjectL.nodeForce.Selected ();
					//Debug.Log ("start highlightedObjectL.nodeForce.myTextMesh.text: " + highlightedObjectL.nodeForce.myTextMesh.text );
				} else {
					highlightedObjectR = graphGenerator.masterNodeList[selectedNodeIndex];
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

			if (handedness == RIGHT) {
				stateR = state;
			} else {
				stateL = state;
			}
		}
	}






}
