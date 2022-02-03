using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    public class EventParameterManager : MonoBehaviour
    {
        [Header("General References")]
        [SerializeField] private GameObject eventSelector;

        [Header("Property Prefabs")]
        [SerializeField] private GameObject IntegerP;

        public Beatmap.Entity entity;

        public static EventParameterManager instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        public void StartParams(Beatmap.Entity entity)
        {
            AddParams(entity);
        }

        private void AddParams(Beatmap.Entity entity)
        {
            var minigame = EventCaller.instance.GetMinigame(entity.datamodel.Split(0));
            int actionIndex = minigame.actions.IndexOf(minigame.actions.Find(c => c.actionName == entity.datamodel.Split(1)));
            Minigames.GameAction action = minigame.actions[actionIndex];

            if (action.parameters != null)
            {
                eventSelector.SetActive(false);
                this.entity = entity;

                Editor.instance.SetGameEventTitle($"Properties for {entity.datamodel}");

                for (int i = 1; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < action.parameters.Count; i++)
                {
                    object param = action.parameters[i].parameter;
                    string caption = action.parameters[i].propertyCaption;
                    string propertyName = action.parameters[i].propertyName;

                    AddParam(propertyName, param, caption);
                }
            }
        }

        private void AddParam(string propertyName, object type, string caption)
        {
            GameObject prefab = IntegerP;

            if (type.GetType() == typeof(EntityTypes.Integer))
            {
                prefab = IntegerP;
            }

            GameObject input = Instantiate(prefab);
            input.transform.SetParent(this.gameObject.transform);
            input.SetActive(true);
            input.transform.localScale = Vector2.one;

            var property = input.GetComponent<EventPropertyPrefab>();
            property.SetProperties(propertyName, type, caption);
        }
    }
}