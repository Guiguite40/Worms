using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BananeChildDispersion : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    protected Rigidbody2D rb = null;

    private Vector2 mdir = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AddRandforce", RpcTarget.AllBuffered);
            rb.AddForce(mdir, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void AddRandforce()
    {
        mdir = new Vector2(Random.Range(-3, 3), Random.Range(5, 10));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
