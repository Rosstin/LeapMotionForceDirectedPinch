using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeForce : MonoBehaviour { // place this script on the node

	public float charge = 1.0f;
	public float mass = 1.0f;
	private float scale = 0.10f;
	public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Color selectedColor = new Color(0.0f, 1.0f, 0.3f, 1.0f);
    public Color hoveredColor = Color.green;

	public int degree;

	public float timeSelected = 0.0f;

	bool selected = false;

	private MeshRenderer myRenderer;

	public GameObject myTextMeshGameObject;
	public TextMesh myTextMesh;

    public float x_3d;
    public float y_3d;
    public float z_3d;
    public Vector3 endPosition_3d;

    public float x_2d;
    public float y_2d;
    public Vector3 endPosition_2d;

    int crawlstate = 0;
    static int NORMAL = 0;
    static int CRAWL_TOWARDS_3D = 3;
    static int CRAWL_TOWARDS_2D = 2;

    static float CRAWL_SPEED = 4.0f;

    List<Color> colors;

	void Start () {
		myRenderer = this.GetComponent<MeshRenderer> ();
		myTextMesh = myTextMeshGameObject.GetComponent<TextMesh> ();

		AddColorsToColorList ();

		//RandomText ();

		//float randomScale = Random.value * 2.0f + 1.0f;
		SetScale (scale);

		//Color randomColor = new Color (Random.value, Random.value, 1.0f, 1.0f);
		//SetColor (randomColor);

		SetColor (color);

		DeactivateText ();


	}

    // Update is called once per frame
    void Update() {
        if (crawlstate != NORMAL)
        {
            Crawl();
        }
	}

    void Crawl()
    {
        if (crawlstate == CRAWL_TOWARDS_3D)
        {
            Vector3 local_endPosition_3d = this.transform.parent.transform.position - endPosition_3d;
            float step = CRAWL_SPEED * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(transform.position, local_endPosition_3d, step);

            if (local_endPosition_3d == gameObject.transform.position)
            {
                crawlstate = NORMAL;
            }

        }
        else if (crawlstate == CRAWL_TOWARDS_2D)
        {
            Vector3 local_endPosition_2d = this.transform.parent.transform.position - endPosition_2d;
            float step = CRAWL_SPEED * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(transform.position, local_endPosition_2d, step);
            if (local_endPosition_2d == gameObject.transform.position)
            {
                crawlstate = NORMAL;
            }

        }

    }

    void FixedUpdate () {
	}


    /*
    public void assumeNewDimensionalPosition(int dimensionality)
    {
        if(dimensionality == GenerateGraph.GRAPH_3D)
        {
            gameObject.transform.position =
                new Vector3(
                    x_3d,
                    y_3d,
                    z_3d + GenerateGraph.DISTANCE_FROM_FACE // i dont think this is correct, just a stopgap
                );
        }
        else
        {
            gameObject.transform.position =
                new Vector3(
                    x_2d,
                    y_2d,
                    0.0f + GenerateGraph.DISTANCE_FROM_FACE // i dont think this is correct, just a stopgap
                );
        }
    }
    */

    public void crawlTowardsNewPosition(int dimensionality)
    {

        // recalculate edges
        // initial position and final position are calculated the same way
        // wizardry with parents will need to be observed at some point

        if (dimensionality == GenerateGraph.GRAPH_3D)
        {
            endPosition_3d = new Vector3(
                    x_3d,
                    y_3d,
                    z_3d
                );
            crawlstate = CRAWL_TOWARDS_3D;

        }
        else
        {
            endPosition_2d = new Vector3(
                    x_2d,
                    y_2d,
                    0.0f 
                );
            crawlstate = CRAWL_TOWARDS_2D;
        }
    }

	public void SetScale (float newScale) {
		scale = newScale;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}

	public void SetScaleFromDegree (int degree) {
		SetScale (Mathf.Log (degree)/10.0f);
	}

	public void SetColor (Color newColor) {
		color = newColor;
		this.myRenderer.material.color = newColor;
	}

	public void SetColorByGroup(int group) {

		//Debug.Log ("color group: " + group);

		//Random.seed = group;

		if (colors == null || colors.Count == 0) {
			AddColorsToColorList ();
		}

		//float r = Mathf.PerlinNoise (group*1.0f, 1.0f);
		//float g = Mathf.PerlinNoise (group*2.0f, 2.0f);
		//unityfloat b = Mathf.PerlinNoise (group*3.0f, 3.0f);

		Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);

		//Debug.Log ("group: " + group + "... r: " + r + "... g: " + g + "... b: " + b);

		newColor = colors[group%colors.Count];

		color = newColor;
	}

	public void RevertColor () {
		this.myRenderer.material.color = color;
	}

	public void Selected() {
		this.myRenderer.material.color = selectedColor;
	}

    public void Hovered()
    {
        this.myRenderer.material.color = hoveredColor;
    }

    public void Unselected() {
		this.myRenderer.material.color = color;
	}


	public void RandomText() {
		float r = Random.value;
		if (r < 0.20f) {
			myTextMesh.text = "synergy";
		} else if (r < 0.40f) {
			myTextMesh.text = "robots";
		} else if (r < 0.60f) {
			myTextMesh.text = "cats";
		} else if (r < 0.80f) {
			myTextMesh.text = "love";
		} else {
			myTextMesh.text = "value";
		}
	}

	public void SetText(string newText) {
		myTextMesh = myTextMeshGameObject.GetComponent<TextMesh> ();
		myTextMesh.text = newText;
	}
		
	public void DeactivateText() {
		myTextMeshGameObject.SetActive (false);
	}

	public void ActivateText() {
		myTextMeshGameObject.SetActive (true);
	}

	public void TextFaceCamera(Transform cameraPosition){
		myTextMesh.transform.rotation = cameraPosition.rotation;
	}

	private void AddColorsToColorList(){
		colors = new List <Color>();

		colors.Add (new Color(1.0f, 0.0f, 0.0f, 1.0f));
		colors.Add (new Color(1.0f, 128.0f/255.0f, 0.0f, 1.0f));
		colors.Add (new Color(1.0f, 1.0f, 0.0f, 1.0f));
		//colors.Add (new Color(128.0f/255.0f, 1.0f, 0.0f, 1.0f));
		//colors.Add (new Color(0.0f, 1.0f, 0.0f, 1.0f));
		//colors.Add (new Color(0.0f, 1.0f, 128.0f/255.0f, 1.0f));

		//colors.Add (new Color(0.0f, 1.0f, 1.0f, 1.0f));
		//colors.Add (new Color(0.0f, 0.5f, 1.0f, 1.0f));
		colors.Add (new Color(0.0f, 0.0f, 1.0f, 1.0f));
		colors.Add (new Color(0.5f, 0.0f, 1.0f, 1.0f));
		colors.Add (new Color(1.0f, 0.0f, 1.0f, 1.0f));
		colors.Add (new Color(1.0f, 0.0f, 0.5f, 1.0f));


	}

}
