using UnityEngine;
using System.Collections;

public class GenerateVoxelCanvas : MonoBehaviour {

    public static int CANVAS_WIDTH = 28;
    public static int CANVAS_HEIGHT = 28;

    // Use this for initialization
    void Start () {
        GameObject model = Instantiate(Resources.Load("Voxel") as GameObject, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        float edgewidth = model.GetComponent<Renderer>().bounds.size.x;
        model.SetActive(false);

        for (int i = 0; i < CANVAS_WIDTH; i++)
        {
            for (int j = 0; j < CANVAS_HEIGHT; j++)
            {
                GameObject currentVoxel = Instantiate(Resources.Load("Voxel") as GameObject, 
                    new Vector3(
                        0.0f + i * edgewidth, 
                        0.0f + j * edgewidth,
                        0.0f),
                    Quaternion.identity) as GameObject;

                currentVoxel.GetComponent<Voxel>().setArbitraryBWColor(Random.Range(0.0f, 1.0f));

                /*
                if ( Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    currentVoxel.GetComponent<Voxel>().setBlackColor();
                }
                else
                {
                    currentVoxel.GetComponent<Voxel>().setWhiteColor();
                }
                */

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
