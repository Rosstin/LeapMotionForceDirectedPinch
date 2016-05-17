using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {

	public GameObject slider;
	public GameObject min;
	public GameObject max;

	public GameObject label;
	TextMesh textScript;

	public int state = 0;
	public int NORMAL = 0;
	public int DRAGGING = 1;

	public int handUsed;

	public int maxValue = 30;
	public int minValue = 0;
	public int currentValue;

	private MeshRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = slider.GetComponent<MeshRenderer>();
		this.myRenderer.material.color = Color.blue;

		textScript = label.GetComponent<TextMesh> ();
		slider.transform.localPosition = new Vector3 ((min.transform.localPosition.x+max.transform.localPosition.x)/2, slider.transform.localPosition.y, slider.transform.localPosition.z );

		UpdateBarValue ();

	}
	
	// Update is called once per frame
	void Update () {

	}

	public bool UpdateBarValue() { // returns whether the value changed

		int lastValue = currentValue;

		float ratioThruSlider = (slider.transform.localPosition.x - min.transform.localPosition.x)/(max.transform.localPosition.x - min.transform.localPosition.x);

		float floatValue = (ratioThruSlider * (maxValue - minValue)) + minValue;

		currentValue = (int) floatValue;

		textScript.text = "Show degree > " + currentValue;

		if (currentValue != lastValue) {
			return true; // value changed

		} else { 
			return false; // value didnt change
		}

	}


	public void OnGrab () {
		this.myRenderer.material.color = Color.red;
	}

	public void UnGrab () {
		this.myRenderer.material.color = Color.blue;
	}


}
