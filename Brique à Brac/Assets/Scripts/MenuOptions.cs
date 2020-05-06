using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{
    private FileManager fm;

    void Start()
    {
        fm = new FileManager();
        fm.SetPreset("Mur 24"); //La carte par défaut
    }

    public void SetCarteMur()
    {
        fm.SetPreset("Mur 24");
    }

    public void SetCarteGrandM()
    {
        fm.SetPreset("LeGrandM 12");
    }
}
