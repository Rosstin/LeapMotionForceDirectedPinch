using UnityEngine;
using System.Collections;

public class NodeForce : MonoBehaviour { // place this script on the node

	public float charge = 1.0f;
	public float mass = 1.0f;
	private float scale = 0.10f;
	public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	private MeshRenderer myRenderer;

	public GameObject myTextMeshGameObject;
	private TextMesh myTextMesh;

	void Start () {
		myRenderer = this.GetComponent<MeshRenderer> ();
		myTextMesh = myTextMeshGameObject.GetComponent<TextMesh> ();

		RandomText ();

		//float randomScale = Random.value * 2.0f + 1.0f;
		SetScale (scale);

		Color randomColor = new Color (1.0f, Random.value, Random.value, 1.0f);
		SetColor (randomColor);
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

	public void RevertColor () {
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
		myTextMesh.text = newText;
	}

	public void TextFaceCamera(Transform cameraPosition){
		myTextMesh.transform.rotation = cameraPosition.rotation;
	}

}
