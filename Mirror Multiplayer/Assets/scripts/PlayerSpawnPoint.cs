using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    //https://www.youtube.com/watch?v=Fx8efi2MNz0&t=539s
   [SerializeField] private int team = 0;
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoint(transform,team);
    }
    void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(transform,team);
    }
}
