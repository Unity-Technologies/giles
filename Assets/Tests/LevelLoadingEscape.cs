using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GILES.Example;
using UnityEngine.SceneManagement;
public class LevelLoadingEscape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && pb_SceneLoader.playTest)
        {
            SceneManager.LoadScene("Level Editor");
        }
    }
}
