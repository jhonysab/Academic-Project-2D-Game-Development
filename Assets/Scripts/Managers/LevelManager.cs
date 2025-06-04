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
        public Transform[] path; // O caminho específico desta rota
    }

    public SpawnRoute[] spawnRoutes; // Array de todas as rotas

    private void Awake()
    {
        main = this;
    }
}