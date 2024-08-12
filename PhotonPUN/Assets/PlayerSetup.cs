using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Movement movement;

    public GameObject camera;

    public string nickname;

    public TextMeshPro nicknameText;

    public Transform TPweaponHolder;

    private int currentWeaponIndex = 0; 


    private void Start()
    {
        if (photonView.IsMine)
        {
            IsLocalPlayer();
        }
        else
        {
            movement.enabled = false;
            camera.SetActive(false);
        }

        photonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        photonView.RPC("SetTPWeapon", RpcTarget.AllBuffered, currentWeaponIndex);
    }
    public void IsLocalPlayer()
    {
        TPweaponHolder.gameObject.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetTPWeapon(int _weaponIndex)
    {
        foreach (Transform _weapon in TPweaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }

        // TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);

        if (_weaponIndex >= 0 && _weaponIndex < TPweaponHolder.childCount)
        {
            TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
            currentWeaponIndex = _weaponIndex; 
        }
    }
    /// <summary>
    /// For weapon syncing
    /// </summary>
    /// <param name="_name"></param>
    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
        if (photonView.IsMine)
        {
            photonView.RPC("SetTPWeapon", newPlayer, currentWeaponIndex);
        }
    }
    public void ChangeWeapon(int newWeaponIndex)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetTPWeapon", RpcTarget.AllBuffered, newWeaponIndex);
        }
    }
    /////////////////////////////////////////////////////////////
    
}
