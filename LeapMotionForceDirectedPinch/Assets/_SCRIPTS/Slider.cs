using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {

	public int state = 0;
	public int NORMAL = 0;
	public int DRAGGING = 1;

	public int handUsed;

	private MeshRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = this.GetComponent<MeshRenderer>();
		this.myRenderer.material.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnGrab () {
		this.myRenderer.material.color = Color.red;
	}

	public void UnGrab () {
		this.myRenderer.material.color = Color.blue;
	}


}
