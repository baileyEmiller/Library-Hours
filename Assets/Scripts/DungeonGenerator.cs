using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [System.Serializable]
    public class DungeonRoom
    {
        public Vector3 location;
        public List<GameObject> doors = new List<GameObject>();
        public GameObject prefab;
        public GameObject inWorldObject;

        public void generate(GameObject room, GameObject door)
        {
            var doorDirection = (room.transform.position - door.transform.position).normalized;
            var offset = (prefab.GetComponent<RoomProperties>().size);
            offset.Scale(doorDirection);
            location = room.transform.position - offset;
            location.y = 0;
            inWorldObject = Instantiate(prefab, location, Quaternion.identity);
            loadDoorsFromPrefab();
            removeFilledDoor(door, doorDirection);
        }

        public void place() {
            location = prefab.transform.position;
            inWorldObject = Instantiate (prefab, location, Quaternion.identity);
            loadDoorsFromPrefab();
        }

        public void removeFilledDoor(GameObject door, Vector3 doorDirection)
        {
            foreach(Transform child in inWorldObject.transform)
            {
                if( doorDirection == -(inWorldObject.transform.position - child.transform.position).normalized)
                {
                    doors.Remove(child.gameObject);
                    Destroy(child.gameObject);
                }
            }
        }

        public void loadDoorsFromPrefab()
        {
            foreach (Transform child in inWorldObject.transform)
            {
                if (child.name.ToLower().Contains("door"))
                {
                    doors.Add(child.gameObject);
                }
            }
            print("Found " + doors.Count + " doors");
        }
    }
    [System.Serializable]
    public struct Connection
    {
        public GameObject door;
        public bool inUse;
    }
    [System.Serializable]
    public class Rank
    {
        public List<DungeonRoom> rooms = new List<DungeonRoom>();
    }

    [SerializeField]
    public List<Rank> ranks = new List<Rank>();
    public Vector3 location;

    private List<Rank> draftPool = new List<Rank>();
    private List<Rank> dungeon = new List<Rank>();

    public void generate()
    {
        draftPool = ranks;
        DungeonRoom root = draftPool[0].rooms[Random.Range(0, draftPool[0].rooms.Count)];
        dungeon.Add(new Rank());
        dungeon[0].rooms.Add(root);
        root.place();
        generateRooms(root, 1);
    }

    public void setRoom(DungeonRoom room, int rank)
    {
        if (ranks.Count >= rank) {
            Rank r = new Rank();
            r.rooms.Add(room);
            ranks.Add(r);
            return;
        }
        ranks[rank].rooms.Add(room);
    }

    private void generateRooms(DungeonRoom room, int rank)
    {
        if (draftPool.Count <= rank) return;
        print("generating rooms for rank" + rank);
        dungeon.Add(new Rank());
        foreach (GameObject door in room.doors) {
            int roomNumber = Random.Range(0, draftPool[rank].rooms.Count);
            var chosenRoom = draftPool[rank].rooms[roomNumber];
            chosenRoom.generate(room.inWorldObject, door);
            dungeon[rank].rooms.Add(chosenRoom);
            //draftPool.RemoveAt(roomNumber);
            generateRooms(chosenRoom, rank + 1);
        }
    }

    public void Start()
    {
        generate();
    }
}
