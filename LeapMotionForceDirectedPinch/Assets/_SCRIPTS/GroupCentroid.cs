using UnityEngine;
using System.Collections;

public class GroupCentroid : Nodelike {

    public GameObject groupContainer;

    void Start()
    {
        myRenderer = this.GetComponent<MeshRenderer>();
        myTextMesh = myTextMeshGameObject.GetComponent<TextMesh>();

        AddColorsToColorList();

        SetColor(color);

        DeactivateText();

    }


    public override void crawlTowardsNewPosition(int dimensionality)
    {
        // centroids only in 3D currently
        if (dimensionality == GenerateGraph.GRAPH_3D)
        {
            this.gameObject.SetActive(true);
            base.crawlTowardsNewPosition(dimensionality);
        }
        else
        {
            this.gameObject.SetActive(false);
            print("centroids don't work in 2D right now, deactivating the centroid");
        }


    }
}
