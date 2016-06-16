using UnityEngine;
using System.Collections;

public class NodeGroup {

    public GameObject gameObject;
    public NodeGroupContainer nodeGroupContainerScript;

    public NodeGroup(GameObject myGameObject)
    {
        gameObject = myGameObject;
        nodeGroupContainerScript = myGameObject.GetComponent<NodeGroupContainer>();
    }

}
