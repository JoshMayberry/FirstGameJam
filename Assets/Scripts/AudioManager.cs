using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public enum MusicArea {
    Dungeon = 0,
    Water = 1,
    Forest = 2
}

public enum MusicState {
    Normal = 0,
    Death = 1,
    Victory = 2,
    Pause = 3,
    Menu = 4
}

public class AudioManager : MonoBehaviour {

    [Header("Volume")]

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambianceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambianceEventInstance;
    private EventInstance musicEventInstance;

    [SerializeField] private GameObject musicMenu;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambianceSlider;
    [SerializeField] private Slider sfxSlider;

    // Make this a singleton
    // See: https://youtu.be/rcBHIOjZDpk?t=1007
    public static AudioManager instance { get; private set; }
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }

        instance = this;

        this.eventInstances = new List<EventInstance>();
        this.eventEmitters = new List<StudioEventEmitter>();

        this.masterBus = RuntimeManager.GetBus("bus:/");
        this.musicBus = RuntimeManager.GetBus("bus:/Music");
        this.ambianceBus = RuntimeManager.GetBus("bus:/Ambiance");
        this.sfxBus = RuntimeManager.GetBus("bus:/SFX");

        this.OnMasterVolumeChanged();
        this.OnMusicVolumeChanged();
        this.OnAmbianceVolumeChanged();
        this.OnSfxVolumeChanged();
    }

    public void Start() {
        AudioManager.instance.InitializeMusic(FmodEvents.instance.gameMusic);
        //AudioManager.instance.InitializeAmbiance(FmodEvents.instance.gameAmbiance);
    }

    public void OnMasterVolumeChanged() {
        this.masterBus.setVolume(this.masterSlider.value);
    }

    public void OnMusicVolumeChanged() {
        this.musicBus.setVolume(this.musicSlider.value);
    }

    public void OnAmbianceVolumeChanged() {
        this.ambianceBus.setVolume(this.ambianceSlider.value);
    }

    public void OnSfxVolumeChanged() {
        this.sfxBus.setVolume(this.sfxSlider.value);
    }

    public void OnToggleMusicMenu() {
        this.musicMenu.SetActive(!this.musicMenu.activeSelf);
    }

    private void InitializeAmbiance(EventReference ambianceEventReference) {
        this.ambianceEventInstance = RuntimeManager.CreateInstance(ambianceEventReference);
        this.eventInstances.Add(ambianceEventInstance);
        this.ambianceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference) {
        this.musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        this.eventInstances.Add(musicEventInstance);
        this.musicEventInstance.start();
    }

    private void CleanUp() {
        foreach (EventInstance eventInstance in this.eventInstances) {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter eventEmitter in this.eventEmitters) {
            eventEmitter.Stop();
        }
    }

    private void OnDestroy() {
        this.CleanUp();
    }

    // Functions
    public void PlayOneShot(EventReference sound, Vector3? worldPosition = null) {
        RuntimeManager.PlayOneShot(sound, (worldPosition.HasValue ? worldPosition.Value : this.gameObject.transform.position));
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject) {
        StudioEventEmitter eventEmitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        eventEmitter.EventReference = eventReference;
        this.eventEmitters.Add(eventEmitter);
        return eventEmitter;
    }

    public EventInstance CreateEventInstance(EventReference eventReference) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        this.eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void SetAmbianceParameter(string parameterName, float parameterValue) {
        this.ambianceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicArea(MusicArea area) {
        this.musicEventInstance.setParameterByName("area", (float)area);
    }
    public void SetMusicState(MusicState state) {
        this.musicEventInstance.setParameterByName("musicState", (float)state);
    }
}
