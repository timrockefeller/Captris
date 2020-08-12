using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EnterGameManager : MonoBehaviour
{

    public GameObject inputField;
    public GameObject startButton;

    public void Enter()
    {
        RD.SetSeedS(int.Parse(inputField.GetComponent<Text>().text));
        SceneManager.LoadScene("MainMap");
    }

    public void EnterRandom(){
        RD.SetSeedS(new System.Random().Next());
        SceneManager.LoadScene("MainMap");
    }
}
