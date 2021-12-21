using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class CommonAnimEvents : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(this.gameObject);
        }
    }

}