using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateManageVarsContainer")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManageVarsContainer");
    }
    public GameObject[] Loots;
    public GameObject[] Monsters;
    public GameObject[] Treasures;
}
