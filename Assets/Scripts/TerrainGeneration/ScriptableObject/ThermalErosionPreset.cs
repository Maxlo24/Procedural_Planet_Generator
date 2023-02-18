using UnityEngine;

[CreateAssetMenu(fileName = "NewThermalErosion", menuName = "Scriptable objects/Terrain/ThermalErosion", order = 0)]
public class ThermalErosionPreset : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField, Range(1, 10)] public int RepetitionCount = 1;
    [SerializeField, Range(1, 20)] public int BorderSize = 0;
    [SerializeField, Range(0, 800)] public int LifeTime = 500;
    [SerializeField, Range(0, 0.5f)] public float Strength = 0.1f;
    [SerializeField, Range(0, 89.99f)] public float MinTanAngle = 0f;
    [SerializeField, Range(0, 89.99f)] public float MaxTanAngle = 89.99f;
}
