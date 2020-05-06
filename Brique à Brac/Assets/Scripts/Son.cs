using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Son
{
    public AudioClip Clip { get; private set; }

    public string Name { get; private set; }

    public bool Loop { get; private set; }

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    public Son(string n, float v, float p, bool l, AudioSource audio)
    {
        source = audio;
        source.name = Name = n;
        source.loop = Loop = l;
        source.volume = volume = v;
        source.pitch = pitch = p;
    }
}
