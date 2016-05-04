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

	public GameObject sceneGod;

	GenerateRandomGraph myGraph;

	void Start () {
		myGraph = sceneGod.GetComponent<GenerateRandomGraph> ();

		//GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;


	}

	// Attach this script to an orthographic camera.

	void FixedUpdate () {


		var rt = pointer.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), new Vector2(Input.mousePosition.x, Input.mousePosition.y), playerCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
		}


		// this needs work, something is jacked up
		if (Input.GetMouseButton(0)){
			//Debug.Log ("mouse being held");
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("centeredPointerGrey");

			// GET POSITION OF EVENT
			Vector3 p = pointer.transform.position;

			Vector3 heading = p - playerCamera.transform.position;

			RaycastHit hit = new RaycastHit ();

			ButtonActivate hitObject;

			if (Physics.Raycast (playerCamera.transform.position, p, out hit)) { // if you hit something
			//if(Physics.SphereCast(playerCamera.transform.position, 12.0f, heading, out hit, 200.0f)) {

				Debug.Log("Hit something.");
				if (hit.transform.gameObject.tag == "Clickable") { // if it was a draggable object
					Debug.Log("Hit Clickable.");
					hit.transform.gameObject.GetComponent<ButtonActivate> ().OnHit ();
					myGraph.showNodesOfDegreeGreaterThan (22);
				}
			}



		} else {
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("centeredPointer");
		}


	}


}


