using UnityEngine;
using System.Collections;

public class ToggleLeap : MonoBehaviour {

	private MeshRenderer myRenderer;

    public GameObject toggleGameObject;

    public GameObject label;
	TextMesh textScript;

    public int state = 0;
    public int NORMAL = 0;
    public int DRAGGING = 1;

    // todo give this arbitrary number of states instead of 2?
    public bool on = false;

    public int handUsed;

    public string trueString;
    public string falseString;
    public string toggleType;


    // Use this for initialization
    void Start () {
		myRenderer = this.GetComponent<MeshRenderer>();
		this.myRenderer.material.color = Color.blue;
        on = false;

		textScript = label.GetComponent<TextMesh> ();
        textScript.text = falseString;

    }

    // Update is called once per frame
    void Update () {

	}

    /*
	public void OnHit () {
		this.myRenderer.material.color = Color.red;
		textScript.text = "Detailing Mode Off";
	}

	public void UnHit () {
		this.myRenderer.material.color = Color.blue;
		textScript.text = "Detailing Mode On";
	}
    */

    public bool switchState()
    {
        on = !on;
        bool newState = on;

        if(newState == true)
        {
            this.myRenderer.material.color = Color.red;
            textScript.text = this.trueString;
        }
        else
        {
            this.myRenderer.material.color = Color.blue;
            textScript.text = this.falseString;
        }

        return newState;

    }

    public void OnGrab()
    {
    }

    public void UnGrab()
    {
    }



}
