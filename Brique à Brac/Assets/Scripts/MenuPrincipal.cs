using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;

public class MenuPrincipal : MonoBehaviour
{
    public void LancerPartie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitterLeJeu()
    {
        Application.Quit();
    }
}
