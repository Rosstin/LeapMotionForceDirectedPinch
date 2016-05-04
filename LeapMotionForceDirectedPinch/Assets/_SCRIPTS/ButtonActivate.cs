using UnityEngine;
using System.Collections;

public class ButtonActivate : MonoBehaviour {

	private MeshRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = this.GetComponent<MeshRenderer>();
		this.myRenderer.material.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnHit () {
		this.myRenderer.material.color = Color.red;
	}
}
