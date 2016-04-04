using UnityEngine;
using System.Collections;

public class NodeForce : MonoBehaviour { // place this script on the node

	public float charge = 1.0f;
	public float mass = 1.0f;
	public float scale = 1.0f;
	public Color color;


	private MeshRenderer myRenderer;

	void Start () {
		myRenderer = this.GetComponent<MeshRenderer> ();

		float randomScale = Random.value * 3.0f;
		UpdateScale (randomScale);

		Color randomColor = new Color (Random.value, Random.value, Random.value, 1.0f);
		UpdateColor (randomColor);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
	}

	void UpdateScale (float newScale) {
		scale = newScale;
		this.transform.localScale = new Vector3(scale, scale, scale);
	}

	void UpdateColor (Color newColor) {
		color = newColor;
		this.myRenderer.material.color = newColor;
	}

	public void RevertColor () {
		this.myRenderer.material.color = color;
	}

}
