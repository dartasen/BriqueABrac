using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager
{

    public string Fichier { get; private set; }

    public FileManager() : this("preset")
    {
    }

    public FileManager(string fi)
    {
        Fichier = fi;
    }

    public void SetPreset(string texte)
    {
        SetPreset(texte, Fichier);
    }

    public void SetPreset(string texte, string nomFichier)
    {
        File.WriteAllText(@"Assets/Resources/Data/" + nomFichier + ".txt", texte + "\n");
    }

    public string GetPreset()
    {
        return GetPreset(Fichier);
    }

    public string GetPreset(string nomFichier)
    {
        return File.ReadAllText(@"Assets/Resources/Data/" + nomFichier + ".txt");
    }

    public Niveau getNiveauChoisi()
    {
        Niveau n;
        string preset = GetPreset();

        var param = preset.Split();
        n = new Niveau(param[0], System.Convert.ToInt32(param[1]));

        Debug.LogWarning("Preset : " + preset);
        Debug.LogWarning("Nom : " + n.NomNiveau);
        Debug.LogWarning("NbBriques : " + n.NbBriques);
        return n;
    }
}