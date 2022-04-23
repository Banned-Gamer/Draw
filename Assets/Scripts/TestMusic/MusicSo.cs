using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music", menuName = "Save/New Music")]
public class MusicSo : ScriptableObject
{
    public AudioClip MyAudio;
    public List<float> TimePoints;
}
