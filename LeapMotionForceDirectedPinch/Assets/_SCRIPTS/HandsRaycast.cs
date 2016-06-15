using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Leap;
using Leap.Unity.PinchUtility;
using Leap.Unity;

public class HandsRaycast : MonoBehaviour {

	public GameObject toggle3DGameObject;
	Collider toggle3DCollider;
    ToggleLeap toggle3Dscript;

	public GameObject slider1;
	Collider slider1Collider;
	Slider slider1script;

    public GameObject sliderFollowers;
    Collider sliderFollowersCollider;
    Slider sliderFollowersScript;

    public GameObject PanelContainer;
    //todo: an object with an array of all buttons should be included 

    public GameObject infravisionQuad;

    public GameObject rightCapsuleHandObject;
    CapsuleHand rightCapsuleHandScript;
    public GameObject leftCapsuleHandObject;
    CapsuleHand leftCapsuleHandScript;

	int panelState;
	int PANEL_ON = 0;
	float turnPanelOffTimer = 0.0f;
	int PANEL_OFF = 1;
	float turnPanelOnTimer = 0.0f;

	public Camera playerCamera; // aka CenterEyeAnchor

	LineRenderer myLineRenderer;

	public GameObject sceneGod;
	GenerateGraph graphGenerator;

    // handles two-handed action of palm proximity
    int palmState = 0;
    static public int PALM_STATE_NORMAL = 0;
    static public int PALM_STATE_GROUP_SELECTED = 501;
    int selectedKey;

    float palmSelectionTime = 0.0f;
    float palmDeselectionTime = 0.0f;

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

	Vector3 nodeContainerStartPosition;
	Vector3 zoomPinchStartPositionL;
	Vector3 zoomPinchStartPositionR;
	float zoomPinchStartDistance;
	float lastZoomPinchDistance;

    public GameObject testCube;

	void Start () {
		GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
		myLineRenderer.enabled = false;

		graphGenerator = sceneGod.GetComponent<GenerateGraph> ();

		rightPinchDetectorScript = rightPinchDetector.GetComponent<LeapPinchDetector> ();
		leftPinchDetectorScript = leftPinchDetector.GetComponent<LeapPinchDetector> ();

        rightCapsuleHandScript = rightCapsuleHandObject.GetComponent<CapsuleHand>();
        leftCapsuleHandScript = leftCapsuleHandObject.GetComponent<CapsuleHand>();

        toggle3DCollider = toggle3DGameObject.GetComponent<Collider> ();
        toggle3Dscript = toggle3DGameObject.GetComponent<ToggleLeap>();

        slider1Collider = slider1.GetComponent<Collider> ();
		slider1script = slider1.GetComponent<Slider> ();

        sliderFollowersCollider = sliderFollowers.GetComponent<Collider>();
        sliderFollowersScript = sliderFollowers.GetComponent<Slider>();

    }

    bool isHandFist(CapsuleHand handScript, int handedness)
    {
        //print("finger0extended: " + handScript.hand_.Fingers[0].IsExtended); // the thumb

        if(!handScript.hand_.Fingers[1].IsExtended && !handScript.hand_.Fingers[2].IsExtended && !handScript.hand_.Fingers[3].IsExtended && !handScript.hand_.Fingers[4].IsExtended)
        {
            handScript.changeHandColor(ConstantsSpacerock.fistColor);
            if (handedness == ConstantsSpacerock.LEFT) { 
                print("left hand is fist");
            }
            else if (handedness == ConstantsSpacerock.RIGHT)
            {
                print("right hand is fist");
            }
            return true;
        }
        else
        {
            return false;
        }

    }

    void FixedUpdate () {

        if (graphGenerator.interactionReady) {

            CheckDebugKeyboardActions();

            UpdateControlPanel();

            isHandFist(leftCapsuleHandScript, ConstantsSpacerock.LEFT);
            isHandFist(rightCapsuleHandScript, ConstantsSpacerock.RIGHT);

    		HandlePinches (leftCapsuleHandScript, leftPinchDetectorScript, ConstantsSpacerock.LEFT);
            HandlePinches(rightCapsuleHandScript, rightPinchDetectorScript, ConstantsSpacerock.RIGHT);

            //print("palm position: " + leftCapsuleHandScript.GetLeapHand().PalmPosition);
            //print("palm normal: " + leftCapsuleHandScript.GetLeapHand().PalmNormal);
            //print("palm velocity: " + leftCapsuleHandScript.GetLeapHand().PalmVelocity);
            //testCube.GetComponent<Rigidbody>().velocity = leftCapsuleHandScript.GetLeapHand().PalmVelocity.ToVector3;

            /*
            if (rightCapsuleHandScript.thumbTip != null && leftCapsuleHandScript.thumbTip != null) { 
                HandleTwoHandedActions(leftCapsuleHandScript, leftPinchDetectorScript, rightCapsuleHandScript, rightPinchDetectorScript);
            }
            */

            if (stateL == STATE_DRAGGING) { // maybe do this if the user stops moving the node around, don't do it if the node is moving a lot
			    graphGenerator.explodeSelectedNode (highlightedObjectL);
		    } 

		    if (stateR == STATE_DRAGGING) {
			    graphGenerator.explodeSelectedNode (highlightedObjectR);
		    }
        }

    }

    void NeutralizeButtonState() // neutralize state of all buttons in when you leave
    {
        NeutralizeSliderState(slider1script);
        NeutralizeSliderState(sliderFollowersScript);
        //todo neutralize the toggles
    }

    void UpdateControlPanel () {
        // looking at panel
        // not looking at panel

        //print("playerCamera.transform.eulerAngles.x: " + playerCamera.transform.eulerAngles.x);

		if (panelState == PANEL_ON) {
			PanelContainer.SetActive (true);

			if (!(playerCamera.transform.eulerAngles.x >= ConstantsSpacerock.VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= ConstantsSpacerock.VIEWPANEL_EULER_X_UPPER_THRESHHOLD)) {
				turnPanelOnTimer = 0.0f;
				turnPanelOffTimer += Time.deltaTime;

				// if you're pinching, don't turn the panel off quite yet
				if (!(rightPinchDetectorScript.IsPinching || leftPinchDetectorScript.IsPinching) && turnPanelOffTimer >= ConstantsSpacerock.PANEL_OFF_TIMER_CONSTANT) {
					panelState = PANEL_OFF;

                    NeutralizeButtonState();
                }
			}

		} else if (panelState == PANEL_OFF) {

            PanelContainer.SetActive (false);

			if (playerCamera.transform.eulerAngles.x >= ConstantsSpacerock.VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= ConstantsSpacerock.VIEWPANEL_EULER_X_UPPER_THRESHHOLD) {
				turnPanelOffTimer = 0.0f;
				turnPanelOnTimer += Time.deltaTime;

				if (turnPanelOnTimer >= ConstantsSpacerock.PANEL_ON_TIMER_CONSTANT) {
					PanelContainer.transform.eulerAngles = new Vector3( PanelContainer.transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, PanelContainer.transform.eulerAngles.z ); 
					panelState = PANEL_ON;
				}

			} 


		}



	}

    void NeutralizeSliderState(Slider slider)
    {
        slider.state = slider.NORMAL;
        performSliderAction(slider.sliderType, slider.currentValue);
        slider.UnGrab();
    }

    void performSliderAction(string sliderType, int value)
    {
        if(sliderType == "degree") // todo should be a constant string stored elsewhere
        {
            graphGenerator.NodeDegree = value;
            graphGenerator.showLegalNodesBasedOnFilterSettings();
        }
        else if(sliderType == "followers")
        {
            graphGenerator.FollowerCount = value;
            graphGenerator.showLegalNodesBasedOnFilterSettings();
        }

    }

    void performToggleAction(ToggleLeap toggle)
    {
        if(toggle.toggleType == "dimensionality")
        {
            bool newDimensionality = toggle.switchState();
            if (newDimensionality)
            {
                graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
            }
            else
            {
                graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
            }
        }

    }

    void UpdateToggleState(Collider collider, ToggleLeap toggle, Ray ray, Vector3 heading, Vector3 p, bool isActive, bool activeThisFrame, int handedness)
    {
        RaycastHit hit = new RaycastHit();

        if (toggle.state != toggle.DRAGGING && isActive && collider.Raycast(ray, out hit, 200.0f))
        { // start a drag
            toggle.state = toggle.DRAGGING;
            toggle.OnGrab();
            toggle.handUsed = handedness;

            // do things related to starting a drag
        }

        if (toggle.state == toggle.DRAGGING && toggle.handUsed == handedness)
        {

            if (!isActive)
            { // no longer dragging
                toggle.state = toggle.NORMAL;
                performToggleAction(toggle);
            }

        }
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
				slider.transform.localPosition = new Vector3( slider.transform.localPosition.x + ConstantsSpacerock.SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);
			} else if (dir < 0f) {
				slider.transform.localPosition = new Vector3( slider.transform.localPosition.x - ConstantsSpacerock.SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);
			} else {
				//Debug.Log("ontarget");
			}
			if (slider.UpdateBarValue ()) { // value changed
                performSliderAction(slider.sliderType, slider.currentValue);
			}
			if (!isActive) { // no longer dragging
                NeutralizeSliderState(slider);
            }

		}
	}

    void HandleTwoHandedActions(CapsuleHand lHand, LeapPinchDetector lDetector, CapsuleHand rHand, LeapPinchDetector rDetector)
    {
        float palmDistance = lHand.hand_.PalmPosition.DistanceTo(rHand.hand_.PalmPosition);
        if (palmState == PALM_STATE_GROUP_SELECTED)
        {

            if (palmDistance > ConstantsSpacerock.TWO_HAND_PROXIMITY_CONSTANT * 2.0f)
            {
                palmDeselectionTime += Time.deltaTime;
                if(palmDeselectionTime > ConstantsSpacerock.PALM_DESELECTION_TIME_THRESHHOLD)
                {
                    print("palmDeselectionTime > PALM_DESELECTION_TIME_THRESHHOLD");

                    //graphGenerator.hideCentroids();

                    print("infravision toggled off");
                    infravisionQuad.SetActive(false);

                    palmState = PALM_STATE_NORMAL;
                    palmSelectionTime = 0.0f;
                }
            }
        }

        else if(palmState == PALM_STATE_NORMAL)
        {
            if (palmDistance < ConstantsSpacerock.TWO_HAND_PROXIMITY_CONSTANT)
            {
                palmSelectionTime += Time.deltaTime;
                if(palmSelectionTime > ConstantsSpacerock.PALM_SELECTION_TIME_THRESHHOLD)
                {
                    print("palmSelectionTime > PALM_SELECTION_TIME_THRESHHOLD");

                    //graphGenerator.showCentroids();

                    // do a raycast against the centroids
                    // first, find the point between the two palms
                    Vector3 p = Vector3.Lerp(lHand.hand_.PalmPosition.ToVector3(), rHand.hand_.PalmPosition.ToVector3(), 0.5f);

                    Vector3 heading = Vector3.Normalize(p - playerCamera.transform.position);
                    Vector3 objectVector;
                    float dotProduct;
                    float biggestDotProduct = 0.0f;

                    // now raycast versus the face

                    foreach (int key in graphGenerator.nodeGroups.Keys) // todo should check legality once that is implemented
                    {
                        objectVector = Vector3.Normalize(graphGenerator.nodeGroups[key].nodeGroupContainerScript.centroid.gameObject.transform.position - playerCamera.transform.position);

                        dotProduct = Vector3.Dot(heading, objectVector);

                        //print("dotProduct: " + dotProduct);

                        if (dotProduct > biggestDotProduct) // should be "if visible" instead
                        { // dont select nodes that are not visible
                            biggestDotProduct = dotProduct;
                            selectedKey = key;
                        }
                    }

                    //graphGenerator.nodeGroups[selectedKey].nodeGroupContainerScript.centroid.groupCentroidScript.Selected();

                    print("infravision toggled on");
                    infravisionQuad.SetActive(true);

                    palmState = PALM_STATE_GROUP_SELECTED;
                    palmDeselectionTime = 0.0f;
                }
            }
        }
    }

    void HandlePinches(CapsuleHand hand, LeapPinchDetector detector, int handedness) {
		// GET ACTIVITY -- are you pinching, clicking?
		bool isActive = detector.IsPinching;
		bool activeThisFrame = detector.DidStartPinch;

        if (isActive)
        {
            hand.changeHandColor(ConstantsSpacerock.pinchColor);
        }

        // GET POSITION OF EVENT
        //Vector3 p = detector.Position;

        Vector3 p = hand.thumbTip.transform.position;

		// camera to pinch vector
		Vector3 heading = Vector3.Normalize(p - playerCamera.transform.position);

		// camera to object vector
		Vector3 objectVector;
		float biggestDotProduct= 0.0f;
		int selectedNodeIndex = 0;
        int hoveredNodeIndex = 0;
		float dotProduct;

		int state = -1;
		if( handedness == ConstantsSpacerock.RIGHT){
			state = stateR;
		}
		else{
			state = stateL;
		}

        if (panelState == PANEL_ON) {

			// do panel actions
			graphGenerator.NodesAreDraggable (false);

			RaycastHit hit = new RaycastHit ();
			Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 100.0f);

			Ray myRay = new Ray (playerCamera.transform.position, heading);

            UpdateToggleState(toggle3DCollider, toggle3Dscript, myRay, heading, p, isActive, activeThisFrame, handedness);

    		UpdateSliderState (slider1Collider, slider1script, myRay, heading, p, isActive, activeThisFrame, handedness);
            UpdateSliderState(sliderFollowersCollider, sliderFollowersScript, myRay, heading, p, isActive, activeThisFrame, handedness);

        } else { // not looking at panel

            /*
            if(isHandFist(hand, handedness))
            {
                
            }
            */

			graphGenerator.NodesAreDraggable (true);

			if (state != STATE_DRAGGING && isActive) { // can start a drag
				state = STATE_DRAGGING;

				for (int i = 0; i < graphGenerator.masterNodeList.Length; i++) {
					if (graphGenerator.isLegalNode(graphGenerator.masterNodeList[i])) {
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

				if (handedness == ConstantsSpacerock.RIGHT) {
                    graphGenerator.masterNodeList[selectedNodeIndex].nodeForce.Selected ();
					originalPinchDistance = originalPinchDistanceR;
				} else {
                    graphGenerator.masterNodeList[selectedNodeIndex].nodeForce.Selected ();
					originalPinchDistance = originalPinchDistanceL;
				}

				if (handedness == ConstantsSpacerock.LEFT) {
					highlightedObjectL = graphGenerator.masterNodeList[selectedNodeIndex];
					highlightedObjectL.nodeForce.Selected ();
					//Debug.Log ("start highlightedObjectL.nodeForce.myTextMesh.text: " + highlightedObjectL.nodeForce.myTextMesh.text );
				} else {
					highlightedObjectR = graphGenerator.masterNodeList[selectedNodeIndex];
					highlightedObjectR.nodeForce.Selected ();
					//Debug.Log ("start highlightedObjectR.nodeForce.myTextMesh.text: " + highlightedObjectR.nodeForce.myTextMesh.text );
				}
			}
			else if (state == STATE_DRAGGING) { // already dragging

				if (handedness == ConstantsSpacerock.LEFT) {
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

                for (int i = 0; i < graphGenerator.masterNodeList.Length; i++)
                {
                    if (graphGenerator.isLegalNode(graphGenerator.masterNodeList[i]))
                    {
                        objectVector = Vector3.Normalize(graphGenerator.masterNodeList[i].gameObject.transform.position - playerCamera.transform.position);
                        dotProduct = Vector3.Dot(heading, objectVector);

                        if (dotProduct > biggestDotProduct) // should be "if visible" instead
                        { // dont select nodes that are not visible
                            biggestDotProduct = dotProduct;
                            hoveredNodeIndex = i;
                        }
                    }

                }

                //graphGenerator.masterNodeList[hoveredNodeIndex].nodeForce.Hovered();

                if (handedness == ConstantsSpacerock.LEFT) {
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

			if (handedness == ConstantsSpacerock.RIGHT) {
				stateR = state;
			} else {
				stateL = state;
			}
		}
	}


    void CheckDebugKeyboardActions()
    {

        if (Input.GetKeyDown("m"))
        {
            graphGenerator.loadMNistGraph();
        }

        if (Input.GetKeyDown("t"))
        {
            graphGenerator.loadTwitterGraph();
        }

        if (Input.GetKeyDown("space"))
        {
            print("infravision toggled on");
            infravisionQuad.SetActive(true);
        }

        if (Input.GetKeyDown("1"))
        {
            print("infravision toggled off");
            infravisionQuad.SetActive(false);
        }


        if (Input.GetKeyDown("c"))
        {
            print("show centroids");
            graphGenerator.showCentroids();
        }

        if (Input.GetKeyDown("x"))
        {
            print("hide centroids");
            graphGenerator.hideCentroids();
        }

        if (Input.GetKeyDown("d"))
        {
            print("graphGenerator.detailingMode = true");
            graphGenerator.detailingMode = true;
        }

        if (Input.GetKeyDown("f"))
        {
            print("graphGenerator.detailingMode = false");
            graphGenerator.detailingMode = false;
        }


        if (Input.GetKeyDown("2"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
        }

        if (Input.GetKeyDown("3"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
        }

    }



}
