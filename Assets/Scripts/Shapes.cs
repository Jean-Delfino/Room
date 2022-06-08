using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


//Gerar parte visual

//https://docs.microsoft.com/pt-br/dotnet/csharp/programming-guide/delegates/how-to-combine-delegates-multicast-delegates

namespace Game.LevelManager.DungeonLoader{
    public abstract class Shapes : MonoBehaviour{
        
        protected delegate void FeedWay(RoomData rd, int x, int y, TileTypes TT);
        protected delegate void FeedType(RoomData rd, int posBeg,int posEnd,int constant,TileTypes TT);
        //delegate void FeedType(RoomData rd, int posBeg,int posEnd,int maxX, int maxY,TileTypes TT);

        //private void HorizontalFeed(RoomData roomData, int xBeg, int xEnd, int Y,TileTypes TT)

        //Both function only work with X = Y
        //Made to test, not to use
        public abstract RoomData CreateFeed(int x, int y);
        public abstract ShapeDescription CreateFeedDescription();

        //X e * X | \ / (X 9 Quadrado) 
        protected RoomData XShape(int x, int y){
            int i, auxPos, min, max;

            auxPos = x-2; //x - i (1) - 1

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
                Feed(roomData, i, i, TileTypes.Block); 
                
                if(i != (auxPos)){
                    Feed(roomData,(auxPos), i,TileTypes.Block);
                } 
                
                //print("Floor1------------" + auxPos);
                JumpObstacleFeed(horizontalFeed, roomData, 0, max, min, 1, i, TileTypes.Floor);
                //print("Floor3------------");
                horizontalFeed(roomData, max+1, x, i, TileTypes.Floor);

                //print("Block--------------");
                auxPos--; //Same as doing x-i-1 every time

            }
            //var uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/RoomGenerator/XShape.asset");
            //AssetDatabase.CreateAsset(roomData, uniquePath);
            return roomData;
        }

        protected void Feed(RoomData roomData, int x, int y, TileTypes TT){
            roomData[x, y] = new Tile(TT);
        }

        protected void EntireRoomFeed(RoomData roomData, int x, int y, TileTypes TT){
            int i;

            for(i = 0 ; i < y ; i++){
                HorizontalFeed(roomData, 0, x, i, TileTypes.Floor);
            }
        }

        protected void HorizontalFeed(RoomData roomData, int xBeg, int xEnd, int Y,TileTypes TT){
            if(xBeg > xEnd) return;
            int i;
            
            for(i = xBeg; i < xEnd; i++){
                Feed(roomData, i, Y, TT);
            }
        }

        protected void VerticalFeed(RoomData roomData, int yBeg, int yEnd, int X,TileTypes TT){
            if(yBeg > yEnd) return;
            int i;

            for(i = yBeg; i < yEnd; i++){
                Feed(roomData, X, i, TT);
            }
        }

        protected void DiagonalFeed(RoomData roomData, int xBeg, int yBeg, int X, int Y,TileTypes TT){
            int i;
            int xMark = xBeg;
            int yMark = yBeg;
            
            for(i = 0; xMark < X && yMark < Y; i++){
                xMark += i;
                yMark += i;
                Feed(roomData, xMark, yMark, TT);
            }
        }

        protected void ReverseDiagonalFeed(RoomData roomData, int xBeg, int yBeg, int X, int Y,TileTypes TT){
            int i;
            int xMark = X;
            int yMark = Y;
            
            for(i = 0; xMark > xBeg && yMark > yBeg; i++){
                xMark -= i;
                yMark -= i;
                Feed(roomData, xMark, yMark, TT);
            }
        }

        protected void FeedCorner(FeedType feed,RoomData roomData, 
            int positionBeg, int positionEnd, int last, TileTypes TT){
            
            feed(roomData, positionBeg, positionEnd, 0, TT);
            feed(roomData, positionBeg, positionEnd, last, TT);
        }


        protected void JumpObstacleFeed(FeedType feed,
            RoomData roomData, int positionBeg, int positionEnd, 
                int startJump, int qtdJump, int constant, TileTypes TT){
            
            if(startJump > positionEnd){
                //print("Entered");
                return;
            }
            feed(roomData, positionBeg, startJump, constant, TT);
            //print("Floor2------------");
            feed(roomData, startJump+qtdJump, positionEnd, constant, TT);

        }
    }  
}

        /*
        void Start(){
            //XShape(10, 10);
            StarShape(10,10);
        }*/
