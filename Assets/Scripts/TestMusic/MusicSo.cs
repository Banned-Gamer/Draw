using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music", menuName = "Save/New Music")]
public class MusicSo : ScriptableObject
{
    public AudioClip MyAudio;
    public List<float> AttackPoints;
    public List<DefencePoint> DefencePoints;
    public float MaxTime;

    [System.Serializable]
    public class DefencePoint
    {
        public float TimePoint;
        public float x;
        public float y;

        public DefencePoint(float timePoint, float inX, float inY)
        {
            TimePoint = timePoint;
            x = inX;
            y = inY;
        }
    }
}