using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    public class Selector : MonoBehaviour
    {
        private bool clicked;

        public static Selector instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void LateUpdate()
        {
            /*if (Input.GetMouseButtonUp(0))
            {
                if (!Timeline.instance.IsDraggingEvent())
                {
                    if (clicked == false)
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            print('a');
                            Selections.instance.DeselectAll();
                        }
                    }
                }
            }

            clicked = false;*/
        }

        public void Click(TimelineEventObj eventObj)
        {
            /*if (Input.GetKey(KeyCode.LeftShift))
            {
                Selections.instance.ShiftClickSelect(eventObj);
            }
            else
            {
                Selections.instance.ClickSelect(eventObj);
            }

            clicked = true;*/
        }
    }
}