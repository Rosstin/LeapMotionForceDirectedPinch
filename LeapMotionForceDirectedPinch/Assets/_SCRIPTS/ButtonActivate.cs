using UnityEngine;
using System.Collections;

public class ButtonActivate : MonoBehaviour {

	private MeshRenderer myRenderer;

	public GameObject label;
	TextMesh textScript;

	// Use this for initialization
	void Start () {
		myRenderer = this.GetComponent<MeshRenderer>();
		this.myRenderer.material.color = Color.blue;

		textScript = label.GetComponent<TextMesh> ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnHit () {
		this.myRenderer.material.color = Color.red;
		textScript.text = "Detailing Mode Off";
	}

	public void UnHit () {
		this.myRenderer.material.color = Color.blue;
		textScript.text = "Detailing Mode On";
	}


}
