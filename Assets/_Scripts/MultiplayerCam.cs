using UnityEngine;
using Mirror;
using Unity.Cinemachine;
using StarterAssets;
public class MultiplayerCam : NetworkBehaviour
{
    [SerializeField]
    private Transform camRoot;    
    [SerializeField]
    private ThirdPersonController cont;    
    [SerializeField]
    private CharacterController charCont;
    private CinemachineCamera freeLookCam;

public override void OnStartLocalPlayer()
{
    base.OnStartLocalPlayer();

    // Find the Cinemachine FreeLook camera in the scene
    freeLookCam = GameObject.FindGameObjectWithTag("FollowPlayer").GetComponent<CinemachineCamera>();

    if (freeLookCam != null)
    {
        freeLookCam.Follow = camRoot;
    }
    else
    {
        Debug.LogError("Cinemachine FreeLook Camera not found in scene!");
    }
    if (isOwned)
    {
        cont.enabled = true;
        charCont.enabled = true;
    }


}
}