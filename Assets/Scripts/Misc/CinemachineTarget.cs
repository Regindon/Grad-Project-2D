using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region Tooltip
    [Tooltip("Populate with the CursorTarget gameobject")]
    #endregion Tooltip
    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
       
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

  
    void Start()
    {
        SetCinemachineTargetGroup();
    }

    //the target group
    private void SetCinemachineTargetGroup()
    {
        //create target group for cinemachine for the cinemachine camera to follow
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 4f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target { weight = 0.4f, radius =0.6f, target = cursorTarget };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;

    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

}