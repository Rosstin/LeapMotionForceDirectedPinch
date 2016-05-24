using UnityEngine;
using System.Collections;

public class Voxel : MonoBehaviour {

    public MeshRenderer myRenderer;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void assignRendererIfNull()
    {
        if (myRenderer == null)
        {
            myRenderer = this.GetComponent<MeshRenderer>();
        }
    }

    public void setBlackColor()
    {
        assignRendererIfNull();
        this.myRenderer.material.color = Color.black;
    }


    public void setWhiteColor()
    {
        assignRendererIfNull();
        this.myRenderer.material.color = Color.white;
    }

    public void setArbitraryBWColor(float value)
    {
        assignRendererIfNull();
        this.myRenderer.material.color = new Color(value, value, value, 1.0f);
    }

}
