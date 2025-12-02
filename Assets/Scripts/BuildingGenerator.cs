using System.CodeDom.Compiler;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject roofPrefab;

    public bool includeRoof = true;
    public int width = 1;
    public int height = 1;
    public float cellUnitSize = 1;
    public int numberOfFloors = 1;

    [SerializeField] public Floor[] floors;

    public class Wall
    {
        public enum WallType
        {
            Plain
        }
        public WallType WallTypeSelected { get; private set; } = WallType.Plain;

        public Wall(WallType wallType = WallType.Plain)
        {
            this.WallTypeSelected = wallType;
        }
    }

    public class Floor
    {
        public int FloorNumber { get; private set; }
        public Room[,] rooms;

        public Floor(int floorNumber, Room[,] rooms)
        {
            FloorNumber = floorNumber;
            this.rooms = rooms;
        }
    }


    public class Room
    {
        public Wall[] Walls;

        private Vector2 position;

        public bool HasRoof { get; private set; }

        public Room(Vector2 position, bool hasRoof = false)
        {
            this.position = position;
            this.HasRoof = hasRoof;
        }

        public Vector2 RoomPosition
        {
            get
            {
                return this.position;
            }
        }
    }


    private void Awake()
    {
        GenerateBuilding();
        RenderBuilding();
    }

    // Creates the data for the structure
    void GenerateBuilding()
    {
        floors = new Floor[numberOfFloors];

        int floorCount = 0;

        foreach(Floor floor in floors)
        {
            Room[,] rooms = new Room[width,height];
            for(int w = 0; w < width; w++)
            {
                for(int h = 0; h < height; h++)
                {
                    rooms[w, h] = new Room(new Vector2(w * cellUnitSize, h * cellUnitSize), includeRoof ? (floorCount == floors.Length - 1) : false);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    // builds sthe building using the prefab & applies rotation
    void RenderBuilding()
    {
        foreach(Floor floor in floors)
        {
            for(int w = 0; w < width; w++)
            {
                for(int h = 0; h < height; h++)
                {
                    Room room = floor.rooms[w, h];
                    var wall1 = Instantiate(wallPrefab, new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y), Quaternion.Euler(0,0,0));
                    wall1.transform.parent = transform;
                    var wall2 = Instantiate(wallPrefab, new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y), Quaternion.Euler(0, 90, 0));
                    wall2.transform.parent = transform;
                    var wall3 = Instantiate(wallPrefab, new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y), Quaternion.Euler(0, 180, 0));
                    wall3.transform.parent = transform;
                    var wall4 = Instantiate(wallPrefab, new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y), Quaternion.Euler(0, -90, 0));
                    wall4.transform.parent = transform;

                    if (room.HasRoof)
                    {
                        var roof = Instantiate(roofPrefab, new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y), Quaternion.identity);
                        roof.transform.parent = transform;
                    }
                }
            }
        }
    }

}
