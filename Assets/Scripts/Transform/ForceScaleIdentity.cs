using UnityEngine;

public class ForceScaleIdentity : MonoBehaviour
{
    void Update()
    {
        transform.localScale = new Vector3(1f / transform.parent.localScale.x, 1f / transform.parent.localScale.y);
    }
}
