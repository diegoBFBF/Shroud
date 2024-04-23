using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerMenuOptions : MonoBehaviour
{
    public void LeaveRoom()
    {
        NetworkOptions.Instance.LeaveRoom();
    }
}
