//https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#
using UnityEngine;
using System;
//Allows us to use Lists.
using System.Collections.Generic;
//Tells Random to use the Unity Engine random number generator.
using Random = UnityEngine.Random;

using Game.LevelManager.DungeonLoader;

namespace Completed{
    public class BoardManager : MonoBehaviour{
        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count{
            public int minimum;             //Minimum value for our Count class.
            public int maximum;             //Maximum value for our Count class.

            //Assignment constructor.
            public Count (int min, int max){
                minimum = min;
                maximum = max;
            }
        }

        [Serializable]
        public class TilePrefab{
            [SerializeField] List<GameObject> pref;

            public List<GameObject> GetAllGameObjects(){
                return this.pref;
            }

            public int GetPrefCount(){
                return pref.Count;
            }
        }

        [SerializeField] List<TilePrefab> prefabsSpawn = default;
        [SerializeField] GameObject exit;             //Prefab to spawn for exit.

        [SerializeField] int columns = 8;             //Number of columns in our game board.
        [SerializeField] int rows = 8;                //Number of rows in our game board.

        //A variable to store a reference to the transform of our Board object.
        private List <Vector3> gridPositions = new List <Vector3>();
        private Transform boardHolder;
        //A list of possible locations to place tiles.

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList (){
            //Clear our list gridPositions.
            gridPositions.Clear();

            //Loop through x axis (columns).
            for(int x = 1; x < columns-1; x++){
                //Within each column, loop through y axis (rows).
                for(int y = 1; y < rows-1; y++){
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    gridPositions.Add (new Vector3(x, y, 0f));
                }
            }
        }


        //Sets up the outer walls and floor (background) of the game board.
        void BoardSetup(){
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject ("Board").transform;
            TilePrefab prefHold; 
            GameObject toInstantiate;
            GameObject instance;

            //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for(int x = -1; x < columns + 1; x++){
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for(int y = -1; y < rows + 1; y++){
                    //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                    prefHold = prefabsSpawn[(int) TileTypes.Floor];
                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    if(x == -1 || x == columns || y == -1 || y == rows){
                        prefHold = prefabsSpawn[(int) TileTypes.Wall];
                    }

                    toInstantiate = prefHold.GetAllGameObjects()
                        [Random.Range(0, prefHold.GetPrefCount() - 1)];

                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), 
                                Quaternion.identity) as GameObject;

                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent(boardHolder);
                }
            }
        }


        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition(){
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }

        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutObjectAtRandom (List<GameObject> tileArray, int minimum, int maximum){
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = Random.Range(minimum, maximum+1);

            //Instantiate objects until the randomly chosen limit objectCount is reached
            for(int i = 0; i < objectCount; i++){
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                Vector3 randomPosition = RandomPosition();

                //Choose a random tile from tileArray and assign it to tileChoice
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Count)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        void LayoutObjectsByPosition(List<GameObject> tileArray, List<Position> tilePos, int sideSpaces){
            int i = 0;
            GameObject tileChoice;
            Position posAux;

            for(i = 0; i < tilePos.Count; i++){
                posAux = tilePos[i];
                tileChoice = tileArray[Random.Range(0, tileArray.Count)];
                
                LayoutObjectByPosition(tileChoice, posAux, sideSpaces);
            }
        }

        void LayoutObjectByPosition(GameObject tileChoice, Position pos, int sideSpaces){
            Instantiate(tileChoice, 
                new Vector3(pos.x + sideSpaces, pos.y + sideSpaces, 0),
                Quaternion.identity);
        }

        public void SetupScene(RoomData roomData, int maxWidth, int maxHeight){
            int i, j;
            List<GameObject> tileArray;


            columns = maxWidth;
            rows = maxHeight;

            BoardSetup();

            for(i = 0; i < maxHeight - 1; i++){
                for(j = 0; j < maxWidth - 1; j++){
                    //
                    if(roomData.Room[i][j].TileType != TileTypes.Floor){
                        //print(roomData.Room[j][i] + " " + i + " " + j);
                        tileArray = prefabsSpawn[(int) roomData.Room[i][j].TileType].GetAllGameObjects();
                        LayoutObjectByPosition(tileArray[Random.Range(0, tileArray.Count)],
                                                new Position(j, i), 0);
                    }
                }
            }
        }

        //Side Spaces is the distance from walls, both in right or left
        public void SetupScene(ShapeDescription roomData, int sideSpaces){
            columns = roomData.width + ((sideSpaces) * 2); //Spaces
            rows = roomData.height + ((sideSpaces) * 2);

            BoardSetup();

            LayoutObjectsByPosition(prefabsSpawn[(int) TileTypes.Block].GetAllGameObjects(),
                roomData.blockPoints, sideSpaces);
        }
    }
}        

/*
        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene(int level){
            //Creates the outer walls and floor.
            BoardSetup();

            //Reset our list of gridpositions.
            InitialiseList();

            //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
            LayoutObjectAtRandom (prefabsSpawn[(int) TileTypes.Block].GetAllGameObjects(),
                wallCount.minimum, wallCount.maximum);

            //Instantiate the exit tile in the upper right hand corner of our game board
            Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
        }

        
            for(int x = 1; x < columns-1; x++){
                //Within each column, loop through y axis (rows).
            for(int y = 1; y < rows-1; y++){

            Go from 1 to LIMIT - 1
*/