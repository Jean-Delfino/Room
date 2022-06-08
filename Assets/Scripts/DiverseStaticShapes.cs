using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Game.LevelManager.DungeonLoader{
    public class DiverseStaticShapes : Shapes{
        public override RoomData CreateFeed(int x, int y){
            return StarShape(x, y);
        }

        public override ShapeDescription CreateFeedDescription(){ return null; }

        //Asterisk
        private RoomData StarShape(int x, int y){
            int i,auxPos,startPosition,min,max, width = (y % 2); //0 for even and 1 for odd

            auxPos = x-2; //x - i (1) - 1
            width = Convert.ToInt32(!Convert.ToBoolean(width)); //1 for even and 0 for odd
            startPosition = (y / 2) - width; //2

            RoomData roomData = ScriptableObject.CreateInstance<RoomData>();
            roomData.Init(x, y);

            FeedType horizontalFeed = HorizontalFeed;
            FeedCorner(horizontalFeed, roomData, 0, x, y-1, TileTypes.Floor);

            for(i = 1; i < x-1; i++){
                min = i;
                max = auxPos;

                if(i > auxPos){
                    min = auxPos;
                    max = i;
                }

                //print("Block--------------");
                Feed(roomData, i, i, TileTypes.Block); //Same as the X

                if(i != auxPos){ //Odd number X
                    Feed(roomData,auxPos, i,TileTypes.Block);
                }

                //print("Floor1------------");
                JumpObstacleFeed(horizontalFeed, roomData, 0, startPosition , min, 1, i, TileTypes.Floor);
                //print("Floor-1.2------------");
                JumpObstacleFeed(horizontalFeed, roomData, (startPosition + width + 1), x , max , 1, i, TileTypes.Floor);

                auxPos--;

                if(i == startPosition || i == (startPosition + width)) continue;
                
                //print("Block2--------------");
                horizontalFeed(roomData, (startPosition), (startPosition + width + 1), i, TileTypes.Floor);
            }
            return roomData;
        }  

        private RoomData CuttedAsterisk(int x, int y){
            int i , startPosition, width = (y % 2); //0 for even and 1 for odd

            width = Convert.ToInt32(!Convert.ToBoolean(width)); //1 for even and 0 for odd
            RoomData roomData = StarShape(x,y);
            startPosition = (y / 2) - width; //2

            for(i = 0; i < width; i++){
                HorizontalFeed(roomData, 1, x-1, startPosition,TileTypes.Block);
            }
            
            HorizontalFeed(roomData, 1, x-1, startPosition+i,TileTypes.Block);

            return roomData;
        }

        private RoomData AllDiagonal(int x, int y){
            int i; //0 for even and 1 for odd

            RoomData roomData = ScriptableObject.CreateInstance<RoomData>();
            roomData.Init(x, y);

            EntireRoomFeed(roomData, x, y, TileTypes.Floor);

            //First in the vertical
            for(i = 0; i < y; i+=2){
                DiagonalFeed(roomData, 0, i, x, y, TileTypes.Block);
            }

            //Horizontal
            for(i = 2; i < x; i+=2){
                DiagonalFeed(roomData, i, 0, x, y, TileTypes.Block);
            }

            return roomData;
        }  

    }
}
