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
        RD.SetSeed(int.Parse(inputField.GetComponent<Text>().text));
        SceneManager.LoadScene("MainMap");
    }

}
