using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Gerar parte visual

//https://docs.microsoft.com/pt-br/dotnet/csharp/programming-guide/delegates/how-to-combine-delegates-multicast-delegates

namespace Game.LevelManager.DungeonLoader
{
    public class Shapes : MonoBehaviour{
        
        delegate void FeedWay(RoomData rd, int x, int y, TileTypes TT);
        delegate void FeedType(RoomData rd, int posBeg,int posEnd,int constant,TileTypes TT);
        //delegate void FeedType(RoomData rd, int posBeg,int posEnd,int maxX, int maxY,TileTypes TT);

        //private void HorizontalFeed(RoomData roomData, int xBeg, int xEnd, int Y,TileTypes TT)


        void Start(){
            //XShape(10, 10);
            StarShape(10,10);
        }

        //Both function only work with X = Y
        //Made to test, not to use

        //X e * X | \ / (X 9 Quadrado) 
        private RoomData XShape(int x, int y){
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

        private void Feed(RoomData roomData, int x, int y, TileTypes TT){
            roomData[x, y] = new Tile(TT);
        }

        private void EntireRoomFeed(RoomData roomData, int x, int y, TileTypes TT){
            int i;

            for(i = 0 ; i < y ; i++){
                HorizontalFeed(roomData, 0, x, i, TileTypes.Floor);
            }
        }

        private void HorizontalFeed(RoomData roomData, int xBeg, int xEnd, int Y,TileTypes TT){
            if(xBeg > xEnd) return;
            int i;
            
            for(i = xBeg; i < xEnd; i++){
                Feed(roomData, i, Y, TT);
            }
        }

        private void VerticalFeed(RoomData roomData, int yBeg, int yEnd, int X,TileTypes TT){
            if(yBeg > yEnd) return;
            int i;

            for(i = yBeg; i < yEnd; i++){
                Feed(roomData, X, i, TT);
            }
        }

        private void DiagonalFeed(RoomData roomData, int xBeg, int yBeg, int X, int Y,TileTypes TT){
            int i;
            int xMark = xBeg;
            int yMark = yBeg;
            
            for(i = 0; xMark < X && yMark < Y; i++){
                xMark += i;
                yMark += i;
                Feed(roomData, xMark, yMark, TT);
            }
        }

        private void ReverseDiagonalFeed(RoomData roomData, int xBeg, int yBeg, int X, int Y,TileTypes TT){
            int i;
            int xMark = X;
            int yMark = Y;
            
            for(i = 0; xMark > xBeg && yMark > yBeg; i++){
                xMark -= i;
                yMark -= i;
                Feed(roomData, xMark, yMark, TT);
            }
        }

        private void FeedCorner(FeedType feed,RoomData roomData, 
            int positionBeg, int positionEnd, int last, TileTypes TT){
            
            feed(roomData, positionBeg, positionEnd, 0, TT);
            feed(roomData, positionBeg, positionEnd, last, TT);
        }


        private void JumpObstacleFeed(FeedType feed,
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
