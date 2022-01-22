using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RhythmHeavenMania.Editor.Commands
{
    public class Selection : IAction
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Redo()
        {
            throw new System.NotImplementedException();
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }

    // I spent 7 hours trying to fix this instead of sleeping, which would've probably worked better.
    // I'll go fuck myself later I'm just glad it works
    // I give massive props to people who code undo/redo systems
    // -- Starpelly

    public class Move : IAction
    {
        public List<Pos> pos = new List<Pos>();

        public class Pos
        {
            public TimelineEventObj eventObj;

            public Vector2 lastPos_;
            public Vector3 previousPos;
        }

        public Move(List<TimelineEventObj> eventObjs)
        {
            pos.Clear();

            for (int i = 0; i < eventObjs.Count; i++)
            {
                Pos p = new Pos();
                p.eventObj = eventObjs[i];
                p.lastPos_ = eventObjs[i].lastPos_;
                p.previousPos = eventObjs[i].transform.localPosition;
                this.pos.Add(p);
            }
        }

        public void Execute()
        {
        }

        public void Redo()
        {
            for (int i = 0; i < pos.Count; i++)
            {
                EnsureEventObj(i);
                pos[i].eventObj.transform.localPosition = pos[i].previousPos;
            }
        }

        public void Undo()
        {

            for (int i = 0; i < pos.Count; i++)
            {
                EnsureEventObj(i);
                pos[i].eventObj.transform.localPosition = pos[i].lastPos_;
            }
        }

        private void EnsureEventObj(int id)
        {
            if (pos[id].eventObj == null)
            {
                pos[id].eventObj = GameManager.instance.Beatmap.entities.Find(c => c.eventObj.eventObjID == pos[id].eventObj.eventObjID).eventObj;
            }
        }
    }

    public class Deletion : IAction
    {
        List<TimelineEventObj> eventObjs;

        List<TimelineEventObj> deletedObjs;

        public Deletion(List<TimelineEventObj> eventObjs)
        {
            this.eventObjs = eventObjs;
        }

        public void Execute()
        {
            deletedObjs = eventObjs;
            for (int i = 0; i < eventObjs.Count; i++)
            {
                Selections.instance.Deselect(eventObjs[i]);
                Timeline.instance.DestroyEventObject(eventObjs[i].entity);
            }
        }

        public void Redo()
        {
            deletedObjs = eventObjs;
            for (int i = 0; i < eventObjs.Count; i++)
            {
                Selections.instance.Deselect(eventObjs[i]);
                Timeline.instance.DestroyEventObject(eventObjs[i].entity);
            }
        }

        public void Undo()
        {
            for (int i = 0; i < deletedObjs.Count; i++)
            {
                Beatmap.Entity e = deletedObjs[i].entity;
                eventObjs[i] = Timeline.instance.AddEventObject(e.datamodel, false, new Vector3(e.beat, -e.track * Timeline.instance.LayerHeight()), e, true, e.eventObj.eventObjID);
            }
        }
    }
}