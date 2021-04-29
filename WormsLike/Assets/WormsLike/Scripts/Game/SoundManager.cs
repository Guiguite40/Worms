using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceMusic;


    [SerializeField] AudioClip menuClick;
    [SerializeField] AudioClip menuClickConfirm;
    [SerializeField] AudioClip menuClickExplo;
    [SerializeField] AudioClip menuClickError;
    [SerializeField] AudioClip menuOst;
    [SerializeField] AudioClip lobbyOst;
    [SerializeField] AudioClip gameOst;
    [SerializeField] Slider sliderSound;
    /*[SerializeField]*/ Slider sliderSoundMenu;
    [SerializeField] Slider sliderSoundMusic;
    /*[SerializeField]*/ Slider sliderSoundMenuMusic;
    //[SerializeField] Slider sliderMusic;

    private Dictionary<string, AudioClip> listMusic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> listSound = new Dictionary<string, AudioClip>();

    float savedValue;
    float savedValueM;
    bool hasFirstLoadedMenu = false;
    bool hasFirstLoadedMenuMusic = false;
    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(this);
        }
	}

	void Start()
    {
        listSound.Add("menuClick", menuClick);
        listSound.Add("menuClickConfirm", menuClickConfirm);
        listSound.Add("menuClickError", menuClickError);
        listSound.Add("menuClickExplo", menuClickExplo);

        listMusic.Add("menuOst", menuOst);
        listMusic.Add("lobbyOst", lobbyOst);
        listMusic.Add("gameOst", gameOst);

        if (PlayerPrefs.HasKey("savedValue"))
        {
            SetSliderToSavedValue(false);
            savedValue = PlayerPrefs.GetFloat("savedValue");
            audioSource.volume = savedValue;
        }
        else
        {
            savedValue = 0.5f;
            sliderSound.value = savedValue;
        }

        if (PlayerPrefs.HasKey("savedValueM"))
        {
            SetSliderToSavedValue(false);
            savedValueM = PlayerPrefs.GetFloat("savedValueM");
            audioSourceMusic.volume = savedValueM;
        }
        else
        {
            savedValueM = 0.5f;
            sliderSoundMusic.value = savedValueM;
        }

        if (MenuManager.instance != null)
        {
            sliderSoundMenu = MenuManager.instance.GetMenuSoundSlider();
            sliderSoundMenuMusic = MenuManager.instance.GetMenuSoundSliderM();
        }
    }

    void Update()
    {
        if (sliderSound.value != savedValue)
            SetSoundToSlider(false);

        if (sliderSoundMusic.value != savedValueM)
            SetSoundToSliderMusic(false);

        if (MenuManager.instance != null)
        {
            if (MenuManager.instance.GetMenuSoundSlider().value != savedValue)
                SetSoundToSlider(true);

            if (MenuManager.instance.GetMenuSoundSliderM().value != savedValueM)
                SetSoundToSliderMusic(true);
        }
    }

    public void PlaySound(string _soundName)
	{
        audioSource.clip = listSound[_soundName];
        audioSource.Play();
    }

    public void PlayMusic(string _musicName)
    {
        audioSourceMusic.clip = listMusic[_musicName];
        audioSourceMusic.Play();
    }

    public void StopMusic(string _musicName)
    {
        audioSourceMusic.Stop();
    }

    void SetSoundToSlider(bool _isMenu)
	{
        if (!_isMenu)
        {
            float value = sliderSound.value;
            //listSound["menuClick"].volume = value;
            //listSound["menuClickConfirm"].volume = value;
            //listSound["menuClickError"].volume = value;
            audioSource.volume = value;
            PlaySound("menuClick");
            savedValue = value;

            PlayerPrefs.SetFloat("savedValue", savedValue);
            Debug.Log("cc");
        }
        else
		{
            if (MenuManager.instance.GetMenuSoundSlider() != null)
            {
                if (!hasFirstLoadedMenu)
                {
                    MenuManager.instance.GetMenuSoundSlider().value = savedValue;
                    hasFirstLoadedMenu = true;
                }
                else
                {
                    savedValue = MenuManager.instance.GetMenuSoundSlider().value;
                    sliderSoundMenu = MenuManager.instance.GetMenuSoundSlider();
                    sliderSoundMenu.value = savedValue;
                    sliderSound.value = savedValue;
                    //listSound["menuClick"].volume = savedValue;
                    //listSound["menuClickConfirm"].volume = savedValue;
                    //listSound["menuClickError"].volume = savedValue;
                    audioSource.volume = savedValue;
                    PlaySound("menuClick");

                    PlayerPrefs.SetFloat("savedValue", savedValue);
                }
            }
        }
    }

    void SetSoundToSliderMusic(bool _isMenu)
    {
        if (!_isMenu)
        {
            float value = sliderSoundMusic.value;
            //listMusic["menuOst"].volume = value;
            //listMusic["lobbyOst"].volume = value;
            audioSourceMusic.volume = value;
            PlaySound("menuClick");
            savedValueM = value;

            PlayerPrefs.SetFloat("savedValueM", savedValueM);
            Debug.Log("ccM");
        }
        else
        {
            if (MenuManager.instance.GetMenuSoundSliderM() != null)
            {
                if (!hasFirstLoadedMenuMusic)
                {
                    MenuManager.instance.GetMenuSoundSliderM().value = savedValueM;
                    hasFirstLoadedMenuMusic = true;
                }
                else
                {
                    savedValueM = MenuManager.instance.GetMenuSoundSliderM().value;
                    sliderSoundMenuMusic = MenuManager.instance.GetMenuSoundSliderM();
                    sliderSoundMenuMusic.value = savedValueM;
                    sliderSoundMusic.value = savedValueM;
                    //listMusic["menuOst"].volume = savedValueM;
                    //listMusic["lobbyOst"].volume = savedValueM;
                    audioSourceMusic.volume = savedValueM;
                    PlaySound("menuClick");

                    PlayerPrefs.SetFloat("savedValueM", savedValueM);
                }
            }
        }
    }

    public void SetSliderToSavedValue(bool _inMenu)
	{
        if (!_inMenu)
        {
            sliderSound.value = PlayerPrefs.GetFloat("savedValue");
            sliderSoundMusic.value = PlayerPrefs.GetFloat("savedValueM");
        }
        else
        {
            //sliderSoundMenu.value = PlayerPrefs.GetFloat("savedValue");
            MenuManager.instance.GetMenuSoundSlider().value = PlayerPrefs.GetFloat("savedValue");
            MenuManager.instance.GetMenuSoundSliderM().value = PlayerPrefs.GetFloat("savedValueM");
            //sliderSoundMenuMusic.value = PlayerPrefs.GetFloat("savedValueM");
        }
    }

    public float GetSavedValue()
	{
        return PlayerPrefs.GetFloat("savedValue");
    }

    public float GetSavedValueM()
    {
        return PlayerPrefs.GetFloat("savedValueM");
    }
}
