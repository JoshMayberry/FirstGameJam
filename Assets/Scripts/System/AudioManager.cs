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

    [Header("WebGLOnly")]
    public bool isWebGl;
    public AudioClip music;
    public AudioClip[] sfxMove;
    public AudioClip[] sfxTwist;
    public AudioClip[] sfxTalk;
    //public AudioClip[] sfxHurt;
    private AudioSource audioSource;
    public GameObject musicVolumeButton;

    // Make this a singleton
    // See: https://youtu.be/rcBHIOjZDpk?t=1007
    public static AudioManager instance { get; private set; }
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }

        instance = this;

#if UNITY_WEBGL
        // WebGL is not playing nice with FMOD right now...
        this.isWebGl = true;
#endif

        this.eventInstances = new List<EventInstance>();
        this.eventEmitters = new List<StudioEventEmitter>();

        this.audioSource = this.GetComponent<AudioSource>();

        if (this.isWebGl) {
            this.audioSource.enabled = true;
            this.musicVolumeButton.SetActive(false);
            return;
        }

        this.audioSource.enabled = false;

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
        if (this.isWebGl) {
            return;
        }
        this.masterBus.setVolume(this.masterSlider.value);
    }

    public void OnMusicVolumeChanged() {
        if (this.isWebGl) {
            return;
        }
        this.musicBus.setVolume(this.musicSlider.value);
    }

    public void OnAmbianceVolumeChanged() {
        if (this.isWebGl) {
            return;
        }
        this.ambianceBus.setVolume(this.ambianceSlider.value);
    }

    public void OnSfxVolumeChanged() {
        if (this.isWebGl) {
            return;
        }
        this.sfxBus.setVolume(this.sfxSlider.value);
    }

    public void OnToggleMusicMenu() {
        if (this.isWebGl) {
            return;
        }
        this.musicMenu.SetActive(!this.musicMenu.activeSelf);
    }

    private void InitializeAmbiance(EventReference ambianceEventReference) {
        this.ambianceEventInstance = RuntimeManager.CreateInstance(ambianceEventReference);
        this.eventInstances.Add(ambianceEventInstance);
        this.ambianceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference) {
        if (this.isWebGl) {
            this.audioSource.clip = this.music;
            this.audioSource.loop = true;
            this.audioSource.Play();
            return;
        }

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
    public void PlayOneShot(string webGlBackupName, EventReference sound, Vector3? worldPosition = null) {
        if (this.isWebGl) {
            AudioClip[] soundArray;

            switch (webGlBackupName) {
                case "moveSound":
                    soundArray = this.sfxMove;
                    break;

                case "twistSound":
                    soundArray = this.sfxTwist;
                    break;

                //case "hurtSound":
                //    //soundArray = this.sfxHurt;
                //    break;

                default:
                    throw new System.Exception("Unknown sound array name '" + webGlBackupName + "'");
            }

            int randomIndex = Random.Range(0, soundArray.Length);
            audioSource.PlayOneShot(soundArray[randomIndex]);
            return;
        }
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
