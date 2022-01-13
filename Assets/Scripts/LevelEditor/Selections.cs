using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    public class Selections : MonoBehaviour
    {
        public List<TimelineEventObj> eventsSelected = new List<TimelineEventObj>();

        public static Selections instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        public void ClickSelect(TimelineEventObj eventToAdd)
        {
            DeselectAll();
            eventsSelected.Add(eventToAdd);
            eventToAdd.Select();
        }

        public void ShiftClickSelect(TimelineEventObj eventToAdd)
        {
            if (!eventsSelected.Contains(eventToAdd))
            {
                eventsSelected.Add(eventToAdd);
                eventToAdd.Select();
            }
            /*else
            {
                eventsSelected.Remove(eventToAdd);
                eventToAdd.DeSelect();
            }*/
        }

        public void DragSelect(TimelineEventObj eventToAdd)
        {
            if (!eventsSelected.Contains(eventToAdd))
            {
                eventsSelected.Add(eventToAdd);
                eventToAdd.Select();
            }
        }

        public void DeselectAll()
        {
            for (int i = 0; i < eventsSelected.Count; i++)
            {
                eventsSelected[i].DeSelect();
            }

            eventsSelected.Clear();
        }

        public void Deselect(TimelineEventObj eventToDeselect)
        {
            if (eventsSelected.Contains(eventToDeselect))
            {
                eventsSelected.Remove(eventToDeselect);
                eventToDeselect.DeSelect();
            }
        }
    }
}