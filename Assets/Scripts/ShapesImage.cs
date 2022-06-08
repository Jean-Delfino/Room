using System.Collections;
using System.Collections.Generic;
//using System.Drawing;

using UnityEngine;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=EYf3Teh0nqM&ab_channel=YusufShakeel
//https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/working-with-images-bitmaps-icons-and-metafiles?view=netframeworkdesktop-4.8
//https://docs.microsoft.com/pt-br/dotnet/api/system.drawing.bitmap?view=dotnet-plat-ext-6.0

//https://answers.unity.com/questions/1610197/how-to-use-systemdrawing-in-unity2018.html
//https://gamedev.stackexchange.com/questions/133372/system-drawing-dll-not-found

//https://forum.unity.com/threads/using-bitmaps-in-unity.899168/

//https://www.youtube.com/watch?v=ae6mW74FZSU&ab_channel=c00pala

//[SerializeField] Bitmap imageConvert = default;

//https://docs.unity3d.com/ScriptReference/Texture2D.GetPixels.html
//https://docs.unity3d.com/ScriptReference/Texture2D.GetPixel.html

namespace Game.LevelManager.DungeonLoader{
    public class ShapeDescription{
        public int width;
        public int height;
        public List<Position> blockPoints;

        public ShapeDescription(int width, int height, List<Position> blockPoints){
            this.width = width;
            this.height = height;
            this.blockPoints = new List<Position>(blockPoints);
        }

        public ShapeDescription(int width, int height){
            this.width = width;
            this.height = height;
        }
    }

    public class Position{
        public int x;
        public int y;

        public Position(int x, int y){
            this.x = x;
            this.y = y;
        }
    }

    public class ShapesImage : Shapes{
        [SerializeField] Texture2D imageConvert = default;

        [SerializeField] Color blockColor = default;

        private void Start() {
            //GenerateMap(imageConvert);
        }

        public override RoomData CreateFeed(int x, int y){return null;}

        public override ShapeDescription CreateFeedDescription(){
            return GenerateMap(imageConvert);
        }

        private ShapeDescription GenerateMap(Texture2D image){
            List<Position> blockPoints = new List<Position>();
            int i, j, block;
            int maxHeight = 0, maxWidth = 0;
            int minHeight = image.height, minWidth = image.width;

            bool atLeastOneBlock = false;

            //print(image.width + " " + image.height);

            //print(image.GetPixel(image.width/2, image.height/2));

            for(i = 0; i < image.height; i++){
                block = -1;

                for(j = 0; j < image.width; j++){
                    if(image.GetPixel(j, i) == blockColor){
                        SetRef(minWidth > j, ref minWidth, j);
                        SetRef(minHeight > i, ref minHeight, i);
                        
                        blockPoints.Add(new Position(j, i));
                        block = j;
                        atLeastOneBlock = true;
                    }
                }


                if(block != -1){
                    maxHeight = i;
                    SetRef(block > maxWidth, ref maxWidth, block);
                }  

            }   

            maxWidth -= minWidth;
            maxHeight -= minHeight;

            if(atLeastOneBlock){
                maxWidth++;
                maxHeight++;
            }

            SubtractAllPosition(blockPoints, minWidth, minHeight);
            //PrintPositionList(blockPoints);
            return new ShapeDescription(maxWidth, maxHeight, blockPoints);
        }

        private void SetRef(bool condition, ref int valueMin, int newValue){
            if(!condition) return;

            valueMin = newValue;
        }

        private void PrintPositionList(List<Position> posPrint){
            int i;

            for(i = 0; i < posPrint.Count; i++){
                print(posPrint[i].x + ", " + posPrint[i].y);
            }
        }

        private void SubtractAllPosition(List<Position> posHold, int minWidth, int minHeight){
            int i;

            for(i = 0; i < posHold.Count; i++){
                posHold[i].x -= minWidth;
                posHold[i].y -= minHeight;
            }
        }
    }
}
