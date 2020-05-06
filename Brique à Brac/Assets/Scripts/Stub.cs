using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stub
{
    private static readonly IList<Niveau> listeNiveaux = new List<Niveau>()
    {
        new Niveau("Mur",24),
        new Niveau("LeGrandM",12)
    };

    public static IList<Niveau> GetListeNiveaux()
    {
        return listeNiveaux;
    }

    public static Niveau GetNiveau(int i)
    {
        return listeNiveaux[i];
    }
}
