using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseClickMovement : MonoBehaviour,HasPosition
{
    public Tilemap map;

    public bool isMoving;
    public Vector3Int targetCell;
    public Vector3Int currentCell;
    public Vector3 nextPos;
    public Vector3 lastPos;
    public NetWorkId id;


    void Init()
    {
        isMoving = false;
    }

    
    void Start()
    {
        Init();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        foreach (var o in Object.FindObjectsOfType<MouseClickMovement>())
        {
            o.Init();
        }
    }

    Vector3Int RelativeCell(Vector2 direction)
    {
        return map.WorldToCell(transform.position + (Vector3) direction);
    }
    struct MovementData
    {
        public Vector3 direction;
        public Vector3 current_pos;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = map.WorldToCell(pos);       
    }

    IEnumerator WaitToMove()
    {  
        isMoving= true;
   
        while(map.WorldToCell(transform.position) != targetCell)
        {
            
            yield return new WaitForSeconds(0.3f);

            var currPos = map.WorldToCell(transform.position);

            if (lastPos == currPos && nextPos != targetCell)
            {
                currPos = new Vector3Int(Mathf.FloorToInt(nextPos.x), Mathf.FloorToInt(nextPos.y), Mathf.FloorToInt(nextPos.z));
            }

            var diff = targetCell - currPos;

            Vector3 direction;            
            if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {                
                direction = new Vector3(diff.x, 0, 0).normalized;
            }
            else
            {                
                direction = new Vector3(0, diff.y, 0).normalized;
            }

            var movement = new MovementData();
            movement.direction = direction;
            movement.current_pos = new Vector3(currPos.x, currPos.y, currPos.z);
            nextPos =movement.current_pos + movement.direction;           

            var json = JsonUtility.ToJson(movement);
            
            Fetch.Post("http://127.0.0.1:8125/set-positions/" + id._name, json);

            lastPos = movement.current_pos;
        }

        isMoving = false;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetCell = map.WorldToCell(worldPos);

            if (!isMoving)
            {
                StartCoroutine(WaitToMove());               
            }
        }
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0 * Time.deltaTime);
    }

}
