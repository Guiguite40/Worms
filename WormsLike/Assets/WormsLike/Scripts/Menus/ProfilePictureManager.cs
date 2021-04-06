using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilePictureManager : MonoBehaviour
{
    public static ProfilePictureManager instance;

    [SerializeField] private Sprite spSlime1;
    [SerializeField] private Sprite spSlime2;
    [SerializeField] private Sprite spSlime3;
    [SerializeField] private Sprite spSlime4;
    [SerializeField] private Sprite spSlime5;
    [SerializeField] private Sprite spSlime6;


    private List<Sprite> listSpPicture = new List<Sprite>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        listSpPicture.Add(spSlime1);
        listSpPicture.Add(spSlime2);
        listSpPicture.Add(spSlime3);
        listSpPicture.Add(spSlime4);
        listSpPicture.Add(spSlime5);
        listSpPicture.Add(spSlime6);
    }

    public Sprite GetPicture(int _index)
	{
        return listSpPicture[_index];
	}
}
