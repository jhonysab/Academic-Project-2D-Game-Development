using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [System.Serializable]
    public class SpawnRoute
    {
        public Transform spawnPoint;
        public Transform[] path;
    }

    public SpawnRoute[] spawnRoutes;

    private void Awake()
    {
        main = this;
    }
}