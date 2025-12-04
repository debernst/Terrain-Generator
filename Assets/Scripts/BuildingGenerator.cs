using System.CodeDom.Compiler;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject roofPrefab;
    public GameObject windowPrefab;
    public GameObject doorPrefab;

    public bool includeRoof = true;
    public int width = 1;
    public int height = 1;
    public float cellUnitSize = 1;
    public int numberOfFloors = 1;
    public bool doorsOnGroundFloorOnly = true;
    [Range(0.0f, 1.0f)] public float doorPercentChance = 0.3f;
    [Range(0.0f, 1.0f)] public float windowPercentChance = 0.5f;
    public Floor[] floors;

    public class Wall
    {
        public enum WallType
        {
            Plain,
            Door,
            Window
        }
        public WallType WallTypeSelected { get; private set; } = WallType.Plain;

        public Wall(WallType wallType = WallType.Plain)
        {
            this.WallTypeSelected = wallType;
        }

        public void SetWallType(WallType wallType)
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
            
            // creates a list of each side (wall) of the building
            Walls = new Wall[4];

            for (int i = 0; i < 4; i++)
            {
                Walls[i] = new Wall(Wall.WallType.Plain);
            }
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
                    AssignWallFeatures(rooms[w, h], floorCount, w, h);
                }
            }
            floors[floorCount] = new Floor(floorCount++, rooms);
        }
    }

    // Logic to apply windows & doors on any side
    void AssignWallFeatures(Room room, int floorNumber, int roomX, int roomY)
    {
        bool isNorthEdge = roomY == height - 1;
        bool isEastEdge = roomX == width - 1;
        bool isSouthEdge = roomY == 0;
        bool isWestEdge = roomX == 0;

        bool[] isExterior = { isNorthEdge, isEastEdge, isSouthEdge, isWestEdge };
        bool isGroundFloor = floorNumber == 0;

        for (int i = 0; i < 4; i++)
        {
            if (!isExterior[i])
                continue; 

            bool canPlaceDoor = (!doorsOnGroundFloorOnly || isGroundFloor) && isGroundFloor; 

            if (canPlaceDoor && Random.value < doorPercentChance)
            {
                room.Walls[i].SetWallType(Wall.WallType.Door);
                continue;
            }

            if (Random.value < windowPercentChance)
            {
                room.Walls[i].SetWallType(Wall.WallType.Window);
            }
        }
    }

    // builds the building using the prefab & applies rotation
    void RenderBuilding()
    {
        foreach(Floor floor in floors)
        {
            for(int w = 0; w < width; w++)
            {
                for(int h = 0; h < height; h++)
                {
                    Room room = floor.rooms[w, h];
                    Vector3 basePosition = new Vector3(room.RoomPosition.x, floor.FloorNumber, room.RoomPosition.y);
                    
                    Quaternion[] rotations = {
                        Quaternion.Euler(0, 0, 0),
                        Quaternion.Euler(0, 90, 0),
                        Quaternion.Euler(0, 180, 0),
                        Quaternion.Euler(0, -90, 0)
                    };

                    // Render each wall with its features
                    for (int i = 0; i < 4; i++)
                    {
                        var wall = Instantiate(wallPrefab, basePosition, rotations[i]);
                        wall.transform.parent = transform;

                        // Add door or window based on wall type
                        if (room.Walls[i].WallTypeSelected == Wall.WallType.Door)
                        {
                            var door = Instantiate(doorPrefab, basePosition, rotations[i]);
                            door.transform.parent = wall.transform;
                        }
                        else if (room.Walls[i].WallTypeSelected == Wall.WallType.Window)
                        {
                            var window = Instantiate(windowPrefab, basePosition, rotations[i]);
                            window.transform.parent = wall.transform;
                        }
                    }

                    // Applies roof if we want a roof
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
