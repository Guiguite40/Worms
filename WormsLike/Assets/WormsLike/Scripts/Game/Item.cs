using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Type type = 0;

    enum Type
    {
        Weapon,
        Utility,
    }

    ItemsList itemsList = 0;
    enum ItemsList
    {
        RoquetLauncher,
        Grenade,
        SaintGrenade,
        Banana,
        AirStrike,
        Teleportation,
        IDK1,
        JetPack,
        IDK2,
        Shield,
        IronBar,
        Touret,
        NotAvailable1,
        NotAvailable2,
        SkipTurn
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
