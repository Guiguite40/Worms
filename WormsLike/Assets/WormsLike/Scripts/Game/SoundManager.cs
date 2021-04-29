using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource menuClick;
    [SerializeField] AudioSource menuClickConfirm;
    [SerializeField] AudioSource menuClickError;

    private Dictionary<string, AudioSource> listMusic = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> listSound = new Dictionary<string, AudioSource>();
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
    }

    void Update()
    {
        
    }

    public void PlaySound(string _soundName)
	{
        listSound[_soundName].Play();
    }

    public void PlayMusic(string _musicName)
    {
        listMusic[_musicName].Play();
    }
}
