using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour {
    [field: Header("Music")]
    [field: SerializeField] public EventReference gameMusic { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField] public EventReference moveSound { get; private set; }
    [field: SerializeField] public EventReference twistSound { get; private set; }
    [field: SerializeField] public EventReference hurtSound { get; private set; }

    [field: Header("Ambiance")]


    public static FmodEvents instance { get; private set; }
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one FMOD Events in the scene.");
        }

        instance = this;
    }
}
