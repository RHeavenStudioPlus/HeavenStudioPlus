using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCursor : MonoBehaviour
{
    [SerializeField] private bool follow = false;
    [SerializeField] private float mouseMoveSpeed;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (follow)
        {
            Vector2 direction = (pos - transform.position).normalized;
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * mouseMoveSpeed, direction.y * mouseMoveSpeed);
        }
        else
        {
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }
}
