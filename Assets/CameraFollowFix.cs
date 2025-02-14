using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollowFix : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

        // Find Trunks using the Player tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            vCam.Follow = player.transform; // Assign Trunks as Follow target
            Debug.Log("Camera is now following: " + player.name);
        }
        else
        {
            Debug.LogError("ERROR: No Player found in scene! Make sure Trunks has the 'Player' tag.");
        }
    }
}
