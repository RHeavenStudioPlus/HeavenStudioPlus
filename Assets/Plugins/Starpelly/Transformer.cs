using UnityEngine;

namespace Starpelly.Transformer
{
    public class Scale
    {
        public static Vector3 Resize(Vector3 amount, Vector3 direction)
        {
            return new Vector3(direction.x * amount.x - 11f, direction.y * amount.y) / 2;
            // transform.localScale = new Vector3(direction.x * amount.x, direction.y * amount.y);
        }
    }
}