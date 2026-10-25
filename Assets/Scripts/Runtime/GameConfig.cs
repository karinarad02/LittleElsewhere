using UnityEngine;
namespace LittleElsewhere
{
    [CreateAssetMenu(menuName = "Little Elsewhere/Game Configuration")]
    public sealed class GameConfig : ScriptableObject
    {
        public int seed = 237;
        public Vector2 worldSize = new Vector2(80, 64);
        public float walkSpeed = 3.2f;
        public float runSpeed = 4.8f;
        public float cameraSize = 10.8f;
        [Tooltip("Real seconds per complete 24-hour game day. 86400 = real time; 1440 = 24 minutes.")]
        public float dayLengthSeconds = 86400;
        public int treeCount = 200;
        public int firewoodGoal = 5;
        public int berryGoal = 3;
        public int wildlifeGoal = 4;
        public int fishGoal = 2;
    }
}
