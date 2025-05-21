using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    private readonly string player = "Player";
    private readonly string questScene = "QuestScene";
    private readonly string skeletonScene = "SkeletonScene";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(player))
        {   
            string currentScene = SceneManager.GetActiveScene().name;

            if(currentScene == questScene)
            {
                SceneManager.LoadScene(skeletonScene);
            }
            else if(currentScene == skeletonScene)
            {
                SceneManager.LoadScene(questScene);
            }
            
        }

        
    }
}
