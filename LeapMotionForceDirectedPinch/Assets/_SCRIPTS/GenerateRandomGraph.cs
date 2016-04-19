using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: read in the data
// TODO: improve speed/performance, only update a subset of the nodes each call
// TODO: 

public class GenerateRandomGraph : MonoBehaviour {

	float CHARGE_CONSTANT = 0.5f;
	float SPRING_CONSTANT = 4.0f;

	float CHANCE_OF_CONNECTION = 0.09f;
	int NUMBER_NODES = 40;

	int NODES_PROCESSED_PER_FRAME = 40; // could also do as a percentage, could have some logic for that, or the max number that can be done

	float NODE_SPREAD_X = 1.0f;
	float NODE_SPREAD_Y = 0.8f;
	float ELEVATION_CONSTANT = 2.0f;
	float NODE_SPREAD_Z = 1.0f;
	float DISTANCE_FROM_FACE = 5.0f;

	int currentIndex = 0;

	int highestNode = 0;

	GameObject nodeContainer;

	AdjacencyList adjacencyList = new AdjacencyList(0);

	public Node[] masterNodeList;

	//public GameObject nodeToClone;

	// Use this for initialization
	void Start () {

		nodeContainer = Instantiate (Resources.Load("NodeContainer") as GameObject,
			new Vector3 (0.0f,0.0f,0.0f),
			Quaternion.identity) as GameObject;

		//generateGraphFromCSV ();
		generateGraphRandomly();

		nodeContainer.transform.position = nodeContainer.transform.position + new Vector3 (0.0f, ELEVATION_CONSTANT, DISTANCE_FROM_FACE);

		StartCoroutine ("ProcessNodesCoroutine");


	}
		
	void generateGraphFromCSV(){
		print ("readCSVData");
		TextAsset text = Resources.Load ("L") as TextAsset;
		string[,] outputGrid = CSVReader.SplitCsvGrid (text.text);

		int numberOfEdges = outputGrid.GetUpperBound(1);

		// find the highest node
		for (int i = 1; i < numberOfEdges; i++) {
			float source = float.Parse(outputGrid [0,i]); // source
			float target = float.Parse(outputGrid [1,i]); // target
			// change it so that it ignores the last newline in a file

			int s = (int) source;
			int t = (int) target;

			if (s > highestNode) {
				highestNode = s;
			}

			if (t > highestNode) {
				highestNode = t;
			}

			//Debug.Log ("outputGrid[0,i]: " + outputGrid[0,i] + "... " + "outputGrid[1,i]: " + outputGrid[1,i]);
		}

		masterNodeList = new Node[highestNode];

		// add nodes
		randomlyPlaceNodes ();

		// add edges
		for (int i = 1; i < numberOfEdges; i++) {

			//Debug.Log ("outputGrid[0,i]: " + outputGrid[0,i] + "... " + "outputGrid[1,i]: " + outputGrid[1,i]);

			float source = float.Parse(outputGrid [0,i]); // source
			float target = float.Parse(outputGrid [1,i]); // target

			int s = (int) source;
			int t = (int) target;

			addEdgeToAdjacencyListAfterValidation (s, t);
		}

	}

	void generateGraphRandomly(){
		masterNodeList = new Node[NUMBER_NODES];

		// add nodes
		randomlyPlaceNodes();

		// populate adjacency
		for (int i = 0; i < NUMBER_NODES; i++) {
			for (int j = 0; j < NUMBER_NODES; j++) {
				if (Random.Range (0.00f, 1.00f) < CHANCE_OF_CONNECTION) {
					addEdgeToAdjacencyListAfterValidation (i, j);
				}
			}
		}
	}

	void randomlyPlaceNodes(){ //also adds vertexes to adjacencylist
		int numNodes = masterNodeList.Length;
		// add nodes
		for (int i = 0; i < numNodes; i++) {

			// add vertexes
			if (i != 0) { adjacencyList.AddVertex (i);}

			GameObject myNodeInstance = 
				Instantiate (Resources.Load("Node") as GameObject,
					new Vector3 (Random.Range (-NODE_SPREAD_X, NODE_SPREAD_X), Random.Range (-NODE_SPREAD_Y, NODE_SPREAD_Y), Random.Range (-NODE_SPREAD_Z, NODE_SPREAD_Z)),
					Quaternion.identity) as GameObject;

			myNodeInstance.transform.parent = nodeContainer.transform;

			masterNodeList [i] = new Node (myNodeInstance, i); 


		}
	}

	void addEdgeToAdjacencyListAfterValidation(int source, int target){
		int smaller = source;
		int bigger = target;
		if (target < source) {
			smaller = target;
			bigger = source;
		}

		if (adjacencyList.isAdjacent (smaller, bigger) == false) {
			adjacencyList.AddEdge (smaller, bigger, nodeContainer);
		}
	}

	void applyForcesBetweenTwoNodes(int i, int j){ // and render the lines
		// apply force
		// there should only be one interaction for each
		// force = constant * absolute(myNodes[i].charge * myNodes[j].charge)/square(distance(myNodes[i], myNodes[j]))

		// CALC REPULSIVE FORCE
		float distance = Vector3.Distance (masterNodeList [i].gameObject.transform.localPosition, masterNodeList [j].gameObject.transform.localPosition); 

		float chargeForce = (CHARGE_CONSTANT) * ((masterNodeList [i].nodeForce.charge * masterNodeList [j].nodeForce.charge) / (distance * distance));

		float springForce = 0;
		if (adjacencyList.isAdjacent (i, j)) {
			// print ("Number " + i + " and number " + j + " are adjacent.");
			springForce = (SPRING_CONSTANT) * (distance);
			// draw a line between the points if it exists

			int smaller = j;
			int bigger = i;
			if (i < j) {
				smaller = i;
				bigger = j;
			}

			string key = "" + smaller + "." + bigger;
			//print ("key: " + key);

			LineRenderer myLineRenderer = adjacencyList._edgesToRender [key];
			myLineRenderer.SetVertexCount (2);
			myLineRenderer.SetPosition (0, masterNodeList [smaller].gameObject.transform.position);
			myLineRenderer.SetPosition (1, masterNodeList [bigger].gameObject.transform.position);
			myLineRenderer.enabled = true;
		} else {
			//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
		}

		float totalForce = chargeForce - springForce; //only if they're in the same direction

		float accel = totalForce / masterNodeList [i].nodeForce.mass;
		float distanceChange = /* v0*t */ 0.5f * (accel) * (Time.deltaTime) * (Time.deltaTime);

		Vector3 direction = masterNodeList [i].gameObject.transform.localPosition - masterNodeList [j].gameObject.transform.localPosition;

		// apply it

		Vector3 newPositionForI = masterNodeList [i].gameObject.transform.localPosition + direction.normalized * distanceChange;

		//if (i == 0) {Debug.Log ("new position for I before constraint: " + newPositionForI);}

		//TODO have to redo the math for these if we're going to move the thing around

		// now it's a local position so this should work again
		if (newPositionForI.x > NODE_SPREAD_X) {
			newPositionForI.x = NODE_SPREAD_X;
		}

		if (newPositionForI.x < -NODE_SPREAD_X) {
			newPositionForI.x = -NODE_SPREAD_X;
		}

		if (newPositionForI.y > NODE_SPREAD_Y) {
			newPositionForI.y = NODE_SPREAD_Y;
		}

		if (newPositionForI.y < -NODE_SPREAD_Y ) {
			newPositionForI.y = -NODE_SPREAD_Y ;
		}

		if (newPositionForI.z > NODE_SPREAD_Z ) {
			newPositionForI.z = NODE_SPREAD_Z ;
		}

		if (newPositionForI.z < -NODE_SPREAD_Z ) {
			newPositionForI.z = -NODE_SPREAD_Z ;
		}

		//if (i == 0) {Debug.Log ("new position for I after constraint: " + newPositionForI);}


		masterNodeList [i].gameObject.transform.localPosition = newPositionForI;

		// put in something to dampen it and stop calculations after it settles down
		// TODO
	}

	// Update is called once per frame
	void Update () {


		//nodeContainer.transform.Rotate (0, Time.deltaTime*10, 0);

		// update only one per frame? don't update every node every frame
		// render lines

		//ProcessNodes ();
	}


	IEnumerator ProcessNodesCoroutine() {
		while (true) {
			for (int j = 0; j < 2; j++) {
				ProcessNodes ();
			}
			yield return null;
		}
	}

	void ProcessNodes () {

		int nodesProcessedThisFrame = 0;
		while( nodesProcessedThisFrame < NODES_PROCESSED_PER_FRAME){
			nodesProcessedThisFrame += 1;
			currentIndex += 1;
			if (currentIndex >= masterNodeList.Length) {
				currentIndex = 0;
			}
			int i = currentIndex;
			for (int j = 0; j < masterNodeList.Length; j++) {
				if (i != j) {
					applyForcesBetweenTwoNodes (i, j);
				}
			}
		}

	}

}
