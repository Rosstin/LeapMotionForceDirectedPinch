using UnityEngine;
using System.Collections;

public class NodeForce : MonoBehaviour { // place this script on the node

	public float charge = 1.0f;
	public float mass = 1.0f;
	private float scale = 0.10f;
	public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Color selectedColor = Color.green;

	public int degree;

	public float timeSelected = 0.0f;

	bool selected = false;

	private MeshRenderer myRenderer;

	public GameObject myTextMeshGameObject;
	public TextMesh myTextMesh;

	void Start () {
		myRenderer = this.GetComponent<MeshRenderer> ();
		myTextMesh = myTextMeshGameObject.GetComponent<TextMesh> ();

		//RandomText ();

		//float randomScale = Random.value * 2.0f + 1.0f;
		SetScale (scale);

		//Color randomColor = new Color (Random.value, Random.value, 1.0f, 1.0f);
		//SetColor (randomColor);

		SetColor (color);

		DeactivateText ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
	}

	public void SetScale (float newScale) {
		scale = newScale;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}

	public void SetColor (Color newColor) {
		color = newColor;
		this.myRenderer.material.color = newColor;
	}

	public void SetColorByGroup(int group) {

		//Debug.Log ("color group: " + group);

		float r = Mathf.PerlinNoise (group*1.0f, 1.0f);
		float g = Mathf.PerlinNoise (group*1.0f, 2.0f);
		float b = Mathf.PerlinNoise (group*1.0f, 3.0f);

		Color newColor = new Color(r, g, b, 1.0f);

		if (group == 0) {
			newColor = Color.blue;
		} else if (group == 1) {
			newColor = Color.magenta;
		} else if (group == 2) {
			newColor = Color.yellow;
		} else if (group == 3) {
			newColor = Color.red;
		} else if (group == 4) {
			newColor = Color.white;
		} else if (group == 5) {
			newColor = Color.gray;
		} else {
			newColor = Color.black;
		}



		color = newColor;
		//this.myRenderer.material.color = newColor;
	}

	public void RevertColor () {
		this.myRenderer.material.color = color;
	}

	public void Selected() {
		this.myRenderer.material.color = selectedColor;
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

}
