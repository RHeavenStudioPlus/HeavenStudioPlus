using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jukebox;

using HeavenStudio.Editor.Track;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Timeline;

namespace HeavenStudio.Editor.Commands
{
    // Insert / Delete Space
    public class MoveEntity : ICommand
    {
        private readonly List<Guid> entityIDs = new();
        private List<double> newMoveBeat;
        private List<double> lastMoveBeat;

        public MoveEntity(List<RiqEntity> originalEntities, List<double> newBeat)
        {
            entityIDs = originalEntities.Select(c => c.guid).ToList();
            newMoveBeat = newBeat;
        }

        public void Execute()
        {
            lastMoveBeat = new();
            var beatmap = GameManager.instance.Beatmap;
            var entities = new[] { beatmap.Entities, beatmap.TempoChanges, beatmap.VolumeChanges, beatmap.SectionMarkers }
                            .SelectMany(list => list);

            for (var i = 0; i < entityIDs.Count; i++)
            {
                var movedEntity = entities.FirstOrDefault(c => c.guid == entityIDs[i]);

                lastMoveBeat.Add(movedEntity.beat);
                movedEntity.beat = newMoveBeat[i];
            }
        }

        public void Undo()
        {
            var beatmap = GameManager.instance.Beatmap;
            var entities = new[] { beatmap.Entities, beatmap.TempoChanges, beatmap.VolumeChanges, beatmap.SectionMarkers }
                            .SelectMany(list => list);

            for (var i = 0; i < entityIDs.Count; i++)
            {
                var movedEntity = entities.FirstOrDefault(c => c.guid == entityIDs[i]);

                movedEntity.beat = lastMoveBeat[i];
            }
        }
    }
}