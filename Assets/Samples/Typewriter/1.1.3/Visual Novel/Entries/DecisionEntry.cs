using Aarthificial.Typewriter.Attributes;
using Aarthificial.Typewriter.Entries;
using Aarthificial.Typewriter.References;
using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// An event specifically for simplifying decision trees
/// </summary>
/// <remarks>
/// There might be a better way to do this
/// </remarks>
[Serializable]
public class DecisionEntry : EventEntry {
    public bool cancelWhenLeavesArea;
    public EntryReference areaFlag;
}
