using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.IO;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private static Son musique;
    private float volumeMusiqueDefaut;
    private float pitchMusiqueDefaut;

    private float volumeSFXDefaut;

    private ArrayList listeMusiques;
    private readonly ArrayList listeSons;    

    void Start()
    {

        listeMusiques = ChargerDossier("Musiques/");

        if (listeMusiques == null)
            Debug.LogWarning("Aucune musique trouvé dans Assets/Resources/Musiques/");
        else
            PlayMusiqueAleatoire();

    }

    private void Awake()
    {

        if (instance == null)   //évite la duplication d'AudioManager
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); //Si on détruit l'objet à chaque changement de scene, la musique ne serait pas continue

        volumeMusiqueDefaut = 0.5F;
        pitchMusiqueDefaut = 1F;

        volumeSFXDefaut = 0.5F;

    }
    
    public void PlayMusiqueAleatoire()
    {

        System.Random rand = new System.Random();

        if (listeMusiques == null)
            Debug.Log("Pb chargement musiques");

        int index = rand.Next(listeMusiques.Count);   //on choisit aléatoirement la musique à jouer

        string musiqueAcharger = "Musiques/" + Path.GetFileNameWithoutExtension(((FileInfo)listeMusiques[index]).Name); //le chemin relatif de la musique à jouer

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>(musiqueAcharger); //charge une musique dans le dossier Resources grâce à un chemin

        PlayMusique(Path.GetFileNameWithoutExtension(((FileInfo)listeMusiques[index]).Name), source);
    }

    public void PlayMusique(string nom, AudioSource source)
    {
        Son s = new Son(nom, volumeMusiqueDefaut, pitchMusiqueDefaut, true, source);
        PlayMusique(s);
    }

    public void ChangerMusique()
    {
        PlayMusiqueAleatoire();
    }

    public void PlayMusique(Son nouvelleMusique)
    {
        if (musique == null)
        {
            musique = nouvelleMusique;
            musique.source.Play();

        }
        else
            if (musique.Name != nouvelleMusique.Name)
            {
            musique.source.Stop(); ;
            musique = nouvelleMusique;
            musique.source.loop = true;
            musique.source.Play();
            }
    }

    private Son FindSon(string name, string type)
    {

        ArrayList dossier = null;

        switch (type)
        {
            case "musique":
                dossier = listeMusiques;
                break;

            case "son":
                dossier = ChargerDossier("Sons/");
                break;
        }

        if (dossier == null)
            Debug.LogWarning("dossier null");

        foreach (FileInfo f in dossier)
        {
            if (Path.GetFileNameWithoutExtension(f.Name) == name)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(dossier + Path.GetFileNameWithoutExtension(f.Name));

                return new Son(Path.GetFileNameWithoutExtension(f.Name), volumeMusiqueDefaut, pitchMusiqueDefaut, true, audioSource);
            }
        }
        return null;
    }

    private ArrayList ChargerDossier(string dossier)
    {
        ArrayList liste = new ArrayList();
        var info = new DirectoryInfo(Application.dataPath + "/Resources/" + dossier);
        var fileInfo = info.GetFiles();


        foreach (var file in fileInfo)
        {
            if (!file.FullName.EndsWith(".meta"))
            {
                liste.Add(file); //Chaque musique à un .meta associé qui est crée par Unity, il contient juste des infos sur le fichier
            }
        }

        return liste;
    }

    public void PlayAvecNom(string name, string type)
    {

        Son s = FindSon(name, type);

        if (s == null || s.source == null)
        {
            Debug.LogWarning("Erreur son " + name + " introuvable");
            return;
        }
        s.source.Play();
    }

    public void PlayMusiqueAvecNom(string name)
    {
        Son s = FindSon(name, "musique");
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Erreur musique " + name + " introuvable");
        }
        PlayMusique(s);
    }

    public void ChangerVolumeMusique(float volume)
    {
        volumeMusiqueDefaut = volume;
        musique.source.volume = volume;
    }

    public void ChangerVolumeSFX(float volume)
    {
        volumeSFXDefaut = volume;        
    }
}

