using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
namespace HeavenStudio.Games.Scripts_BuiltToScaleDS
{
    public class BTSPiece : MonoBehaviour
    {
        public Animator anim;

        void LateUpdate()
        {
            if (anim.IsAnimationNotPlaying())
                Destroy(gameObject);
        }
    }
}
