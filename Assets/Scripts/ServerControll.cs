using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ServerControll : MonoBehaviour, HasPosition
{
    public Tilemap map;

    public void SetPos(Vector3 pos)
    {
       transform.position = map.WorldToCell(pos);
    }
}
