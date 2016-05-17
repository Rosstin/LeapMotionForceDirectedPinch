using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: read in the data
// TODO: improve speed/performance, only update a subset of the nodes each call
// TODO: 

public class GenerateRandomGraph : MonoBehaviour {

	public Camera playerCamera; //aka centereyeanchor

	float CHARGE_CONSTANT = 0.5f;
	float SPRING_CONSTANT = 4.0f;

	float CHANCE_OF_CONNECTION = 0.09f;
	int NUMBER_NODES = 40;

	int NODES_PROCESSED_PER_FRAME = 40; // could also do as a percentage, could have some logic for that, or the max number that can be done

	float NODE_SPREAD_X = 1.0f;
	float NODE_SPREAD_Y = 0.8f;
	float ELEVATION_CONSTANT = 1.0f; // TRY SETTING HEIGHT BY USING PLAYER CAMERA HEIGHT?
	float NODE_SPREAD_Z = 1.0f;
	float DISTANCE_FROM_FACE = 10.0f;

	float GRAPH_SCALE_CONSTANT = 0.005f;

	int NODE_LIMIT = 200;

	int currentIndex = 0;

	//int highestNode = 0;

	public int NodeDegree = 0;

	float EXPLOSION_TIME_1 = 2.0f;
	float EXPLOSION_TIME_2 = 4.0f;
	float EXPLOSION_TIME_3 = 20.0f;

	public bool detailingMode = false;

	Dictionary<string, int> nameToID = new Dictionary<string, int> ();

	public GameObject nodeContainer;
	Leap.Unity.PinchUtility.LeapRTS myLeapRTS;

	public Vector3 nodeContainerOriginalPosition;

	AdjacencyList adjacencyList = new AdjacencyList(0);

	//List<int> tier2Nodes = new List<int> ();

	public Node[] masterNodeList;
	int[] indicesToShowOrExplode;


	// Use this for initialization
	void Start () {

		//nodeContainer = Instantiate (Resources.Load("NodeContainer") as GameObject, new Vector3 (0.0f,0.0f,0.0f),Quaternion.identity) as GameObject;

		//generateGraphFromCSV("node_with_attribures_query_bernie", "edgelist_query_bernie");
		generateGraphFromCSV("node_with_attribures_query_hillary", "edgelist_query_hillary");
		//generateGraphFromCSV("node_with_attribures_query_trump", "edgelist_query_trump");
		//generateGraphRandomly();

		RenderLinesOnce ();
		HideAllLines();

		nodeContainer.transform.position = nodeContainer.transform.position + new Vector3 (0.0f, ELEVATION_CONSTANT, DISTANCE_FROM_FACE);
		nodeContainerOriginalPosition = nodeContainer.transform.position;

		myLeapRTS = nodeContainer.GetComponent<Leap.Unity.PinchUtility.LeapRTS> ();

		showNodesOfDegreeGreaterThan (15);

		//StartCoroutine ("ProcessNodesCoroutine");


	}

	public void explodeSelectedNode(Node highlightedNode) {
		if (highlightedNode != null && detailingMode == true) {
			float time = highlightedNode.nodeForce.timeSelected;

			// show more connections over time // only do these once!

			if (time >= EXPLOSION_TIME_3 && ((time - Time.deltaTime) < EXPLOSION_TIME_3)) {

				List<int> myList = adjacencyList.GetEdgesForVertex (highlightedNode.index);

				foreach (int subIndex in myList) {
					showConnectedNodes (adjacencyList.GetEdgesForVertex (subIndex), subIndex);
				}

			}
			else if (time >= EXPLOSION_TIME_2 && ((time - Time.deltaTime) < EXPLOSION_TIME_2)) {
				// a list of vertices... show every vertex here
				showConnectedNodes (adjacencyList.GetEdgesForVertex (highlightedNode.index), highlightedNode.index);


			} else if (time >= EXPLOSION_TIME_1 && ((time - Time.deltaTime) < EXPLOSION_TIME_1)) {
				// hide all other nodes
				hideNodes ();
				highlightedNode.gameObject.SetActive (true);
				//HideAllLines();
			}


		}
		// then... show lines and nodes based on connections



		// first, hide all other nodes

		// then, draw the edges connecting this node to others
		// then, draw the edges connecting those nodes to others

		// coroutine?

	}

	public void unselectNode(){
		showNodesOfDegreeGreaterThan (NodeDegree);

		hideLabels ();

		HideAllLines (); 
	}


	public void generateGraphFromCSV(string nodeAsset, string edgeAsset){
		//print ("readCSVData");
		TextAsset edgesText = Resources.Load (edgeAsset) as TextAsset;
		string[,] edgesGrid = CSVReader.SplitCsvGrid (edgesText.text);
		int numberOfEdges = edgesGrid.GetUpperBound(1)-1;

		TextAsset positionsText = Resources.Load (nodeAsset) as TextAsset;
		string[,] positionsGrid = CSVReader.SplitCsvGrid (positionsText.text);
		int numberOfNodes = positionsGrid.GetUpperBound(1)-1;

		masterNodeList = new Node[numberOfNodes];
		indicesToShowOrExplode = new int[numberOfNodes];

		print ("masterNodeList.Length: " + masterNodeList.Length);

		// add nodes
		for (int i = 1; (i < numberOfNodes+1) ; i++) { // check your integer stuff so you dont mess it up man
			//print("i in add nodes: " + i);
			// add vertexes
			if (i != 0) { adjacencyList.AddVertex (i);}

			string label = positionsGrid [0, i];
			Vector3 position = 
				new Vector3 (
					float.Parse (positionsGrid [8, i]) * GRAPH_SCALE_CONSTANT,
					float.Parse (positionsGrid [9, i]) * GRAPH_SCALE_CONSTANT,
					float.Parse (positionsGrid [10, i]) * GRAPH_SCALE_CONSTANT
				);

			GameObject myNodeInstance = 
				Instantiate (Resources.Load("Node") as GameObject,
					position,
					Quaternion.identity) as GameObject;

			NodeForce nodeScript = myNodeInstance.GetComponent<NodeForce>();
			nodeScript.SetText (positionsGrid [1, i]);

			nodeScript.degree = int.Parse(positionsGrid [2, i]);

			nodeScript.SetScaleFromDegree (int.Parse(positionsGrid [2, i]));

			myNodeInstance.transform.parent = nodeContainer.transform;

			masterNodeList [i-1] = new Node (myNodeInstance, i-1); 

			masterNodeList [i - 1].nodeForce.SetColorByGroup (int.Parse(positionsGrid [3, i]));

			nameToID.Add (label, i-1);
		}

		// add edges
		for (int i = 1; i < numberOfEdges; i++) {

			//Debug.Log ("outputGrid[0,i]: " + outputGrid[0,i] + "... " + "outputGrid[1,i]: " + outputGrid[1,i]);

			//print ("edgesGrid [0, i]: " + edgesGrid [0, i] + "... edgesGrid [1,i]: " + edgesGrid [1,i] );

			int source = nameToID[(edgesGrid [0,i])]; // source
			int target = nameToID[(edgesGrid [1,i])]; // target

			int s = (int) source;
			int t = (int) target;

			addEdgeToAdjacencyListAfterValidation (s, t);
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

	public void NodesAreDraggable(bool draggable){
		if (draggable) {
			myLeapRTS.enabled = true;
		} else {
			myLeapRTS.enabled = false;
		}
	}

	void hideNodes(){
		for (int i = 0; i < masterNodeList.Length; i++) {
			masterNodeList [i].gameObject.SetActive (false);
		}
	}

	void showNodes(){
		for (int i = 0; i < masterNodeList.Length; i++) {
			masterNodeList [i].gameObject.SetActive (true);
		}
	}

	void hideLabels(){
		for (int i = 0; i < masterNodeList.Length; i++) {
			masterNodeList [i].nodeForce.DeactivateText();
		}
	}

	public void showNodesOfDegreeGreaterThan(int myDegree){

		NodeDegree = myDegree;

		for (int i = 0; i < masterNodeList.Length; i++) {
			if (masterNodeList [i].nodeForce.degree >= myDegree) {
				masterNodeList [i].gameObject.SetActive (true);
			} else {
				masterNodeList [i].gameObject.SetActive (false);
			}
		}
	}


	void showConnectedNodes(List<int> indices, int mainIndex){
		foreach (int index in indices) {

			if (masterNodeList [index].nodeForce.degree > NodeDegree) {

				masterNodeList [index].gameObject.SetActive (true);
				masterNodeList [index].nodeForce.ActivateText ();
				masterNodeList [index].nodeForce.TextFaceCamera (playerCamera.transform);
				showLinesBetween (index, mainIndex);
			}

			//Debug.Log ("index: " + index + "... mainIndex: " + mainIndex + "... adjacencyList.isAdjacent (index, mainIndex): " + adjacencyList.isAdjacent (index, mainIndex));
		}
	}



	void generateGraphRandomly(){
		masterNodeList = new Node[NUMBER_NODES];
		indicesToShowOrExplode = new int[NUMBER_NODES];

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

		//print("applyForcesBetweenTwoNodes(int i, int j).. i: " + i + " j: "+ j );

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

	void renderLinesBetween(int i, int j){ 
		if (adjacencyList.isAdjacent (i, j)) {
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
	}

	void hideLinesBetween(int i, int j){ 
		if (adjacencyList.isAdjacent (i, j)) {
			int smaller = j;
			int bigger = i;
			if (i < j) {
				smaller = i;
				bigger = j;
			}

			string key = "" + smaller + "." + bigger;
			//print ("key: " + key);

			LineRenderer myLineRenderer = adjacencyList._edgesToRender [key];
			myLineRenderer.enabled = false;
		} else {
			//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
		}
	}

	void showLinesBetween(int i, int j){ 
		if (adjacencyList.isAdjacent (i, j)) {
			int smaller = j;
			int bigger = i;
			if (i < j) {
				smaller = i;
				bigger = j;
			}

			string key = "" + smaller + "." + bigger;
			//Debug.Log ("key: " + key);

			LineRenderer myLineRenderer = adjacencyList._edgesToRender [key];
			myLineRenderer.enabled = true;
		} else {
			//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
		}
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

	void RenderLinesOnce () { 
		for(int i = 0; i < masterNodeList.Length-1; i++){
			for (int j = 0; j < masterNodeList.Length-1; j++) {
				if (i != j) {
					renderLinesBetween (i, j);
				}
			}
		}
	}

	void HideAllLines () { 
		foreach (KeyValuePair<string, LineRenderer> item in adjacencyList._edgesToRender) {
			item.Value.enabled = false;
		}
	}



}
