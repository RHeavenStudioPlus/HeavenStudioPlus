using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor.Commands
{
    public interface IAction
    {
        void Execute();
        void Undo();
        void Redo();
    }
}