using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSelection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickSelection(string size) 
    {
        PlayerPrefs.SetString("size",size);
        SceneManager.LoadSceneAsync("Game");
    }
}
