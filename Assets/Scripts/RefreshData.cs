using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RefreshData : MonoBehaviour
{
    public bool isRefreshing;
    public NetWorkId id;
    
    public HasPosition pos;

    private void Init()
    {
        Application.runInBackground= true;
        id = GetComponent<NetWorkId>();
        isRefreshing = false;
        
        pos = GetComponent<HasPosition>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }      

    [UnityEditor.Callbacks.DidReloadScripts]
    static void OnScriptsReload()
    {
        foreach(var o in Object.FindObjectsOfType<RefreshData>())
        {
            o.Init();
        }
    }   

    public void GotData(string jsonData)
    {
        var newPos = JsonUtility.FromJson<Vector3>(jsonData);
            
        pos.SetPos(newPos);          
    }

    IEnumerator Refresh()
    {
        isRefreshing = true;

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Debug.Log("asking position for" + id.name);

            Fetch.Get("http://127.0.0.1:8125/positions/" + id._name, this);           // ahmmm when did he change this   
        }

        isRefreshing = false;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {        
        if (!isRefreshing)
        {
            StartCoroutine(Refresh());            
        }   
    }        
}
