using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music", menuName = "Save/New Music")]
public class MusicSo : ScriptableObject
{
    public AudioClip          MyAudio;
    public List<float>        AttackPoints;
    public List<DefensePoint> DefensePoints;
    public float              MaxTime;

    [System.Serializable]
    public class DefensePoint
    {
        public float TimePoint;
        public float x;
        public float y;

        public DefensePoint(float timePoint, float inX, float inY)
        {
            TimePoint = timePoint;
            x         = inX;
            y         = inY;
        }
    }
}