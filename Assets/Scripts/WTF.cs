using UnityEngine;

// this is a script for testing

namespace RhythmHeavenMania.Tests
{
    public class WTF : MonoBehaviour
    {
        private void Update()
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, Conductor.instance.loopPositionInAnalog));
            print(Conductor.instance.loopPositionInAnalog);
        }
    }
}