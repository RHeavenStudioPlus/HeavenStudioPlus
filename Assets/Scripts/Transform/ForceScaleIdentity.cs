using UnityEngine;

public class ForceScaleIdentity : MonoBehaviour
{
    public GameObject parent;

    void Update()
    {
        if (parent == null)
            transform.localScale = new Vector3(1f / transform.parent.localScale.x, 1f / transform.parent.localScale.y);
        else
            transform.localScale = new Vector3(1f / parent.transform.localScale.x, 1f / parent.transform.localScale.y);
    }
}
