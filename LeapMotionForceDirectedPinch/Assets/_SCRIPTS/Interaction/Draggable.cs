using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour {

	private MeshRenderer myMeshRenderer = null;

	// Use this for initialization
	void Start () {
		myMeshRenderer = this.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		//this.myMeshRenderer.material.color = Color.black;	
	}

	public void OnHit()
	{
		this.myMeshRenderer.material.color = Color.green;
	}

}
