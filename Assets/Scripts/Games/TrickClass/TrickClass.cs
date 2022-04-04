using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games
{
    /**
        mob_Trick
    **/
    public class TrickClass : MonoBehaviour
    {

        public static TrickClass instance;

        private void Awake()
        {
            instance = this;
        }
    }
}