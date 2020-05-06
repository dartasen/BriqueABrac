using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Niveau : MonoBehaviour
{
    public int NbBriques { get; private set; }
    public string NomNiveau { get; private set; }

    public Niveau(string nom,int nb)
    {
        NbBriques = nb;
        NomNiveau = nom;
    }

    public string GetNom()
    {
        return NomNiveau;
    }
}
