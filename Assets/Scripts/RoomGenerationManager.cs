using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Completed;
namespace Game.LevelManager.DungeonLoader{
    public class RoomGenerationManager : MonoBehaviour{
        [SerializeField] BoardManager bm = default;
        [SerializeField] Shapes shapeGenerator = default;

        public RoomData rd;

        void Start(){
            //bm.SetupScene(0);
            //bm.SetupScene(shapeGenerator.CreateFeedDescription(), 1);
            rd = shapeGenerator.CreateFeed(10,10);
            bm.SetupScene(rd, 10, 10);
        }

    }
}
