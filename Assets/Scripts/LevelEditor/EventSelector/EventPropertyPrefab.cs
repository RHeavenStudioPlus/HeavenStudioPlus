using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

using Jukebox;

namespace HeavenStudio.Editor
{
    public class EventPropertyPrefab : MonoBehaviour
    {
        public TMP_Text caption;
        protected string _captionText;
        public EventParameterManager parameterManager;
        public RiqEntity entity;
        public string propertyName;
        public List<PropertyCollapse> propertyCollapses = new List<PropertyCollapse>();

        public virtual void SetProperties(string propertyName, object type, string caption)
        {
            this.parameterManager = EventParameterManager.instance;

            entity = parameterManager.entity;
            this.propertyName = propertyName;
            this.caption.text = _captionText = caption;
        }
        public virtual void SetCollapses(object type) { }
        public virtual void PostLoadProperties(object type) { }

        public void UpdateCollapse(object type)
        {
            List<EventPropertyPrefab> recursedCollapse = new() { this };
            foreach (var p in propertyCollapses)
            {
                if (p.collapseables.Count > 0)
                {
                    foreach (var c in p.collapseables)
                    {
                        if (c != null)
                        {
                            c.gameObject.SetActive(p.collapseOn(type, p.entity) && gameObject.activeSelf);
                            c.RecursiveUpdateCollapse(recursedCollapse);
                        }
                    }
                }
                else
                {
                    _ = p.collapseOn(type, p.entity);
                }
            }
        }

        public void RecursiveUpdateCollapse(List<EventPropertyPrefab> updated)
        {
            if (updated == null)
            {
                updated = new();
            }
            if (updated.Contains(this))
            {
                return;
            }
            updated.Add(this);
            foreach (var p in propertyCollapses)
            {
                if (p.collapseables.Count > 0)
                {
                    foreach (var c in p.collapseables)
                    {
                        if (c != null)
                        {
                            c.gameObject.SetActive(p.collapseOn(entity[propertyName], p.entity) && gameObject.activeSelf);
                            c.RecursiveUpdateCollapse(updated);
                        }
                    }
                }
                else
                {
                    _ = p.collapseOn(entity[propertyName], p.entity);
                }
            }
        }

        public class PropertyCollapse
        {
            public List<EventPropertyPrefab> collapseables;
            public Func<object, RiqEntity, bool> collapseOn;
            public RiqEntity entity;

            public PropertyCollapse(List<EventPropertyPrefab> collapseables, Func<object, RiqEntity, bool> collapseOn, RiqEntity entity)
            {
                this.collapseables = collapseables;
                this.collapseOn = collapseOn;
                this.entity = entity;
            }
        }
    }
}