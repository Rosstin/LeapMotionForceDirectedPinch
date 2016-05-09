using UnityEngine;
using System.Collections;

public class PlacePanelBasedOnHead : MonoBehaviour {

	public GameObject Panel;
	public GameObject CenterEyeAnchor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Panel.transform.eulerAngles = new Vector3( Panel.transform.eulerAngles.x, CenterEyeAnchor.transform.eulerAngles.y, Panel.transform.eulerAngles.z );
	}
}


