using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Completed;

public class RoomGenerationManager : MonoBehaviour{
    [SerializeField] BoardManager bm;

    void Start(){
        bm.SetupScene(0);
    }

}
