using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
