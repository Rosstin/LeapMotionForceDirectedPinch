using UnityEngine;
using System.Collections;

public class GroupCentroidReferences {
    public GameObject gameObject;
    public GroupCentroid groupCentroidScript;

    public GroupCentroidReferences(GameObject myGameObject)
    {
        gameObject = myGameObject;
        groupCentroidScript = myGameObject.GetComponent<GroupCentroid>();
    }
}
