using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class RoomTemplates : MonoBehaviour {

    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;

    public List<GameObject> rooms;

    public float waitTime;
    private bool spawnedendTile = false;
    private int maxEnemyCount = 0;
    private int currentEnemyCount = 0;
    [SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private GameObject upStair;
    public GameObject endTile;

    void Update(){

        if(waitTime <= 0 && !spawnedendTile){
            for (int i = 0; i < rooms.Count; i++) {
                if(i == rooms.Count-1)
                {
					ReparentRooms();
                    Instantiate(endTile, rooms[i].transform.position, Quaternion.identity);
                    navSurface.BuildNavMesh();
                    spawnedendTile = true;
                    upStair.SetActive(true);
                }
            }
        } else {
            waitTime -= Time.deltaTime;
        }
    }

    private void ReparentRooms()
    {
        foreach (GameObject room in rooms)
        {
            room.transform.SetParent(transform);
        }
    }
}
