using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPostProcessing", menuName = "Scriptable objects/Terrain/PostProcessing", order = 0)]
public class PostProcessing : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField, Range(0,10)] public float Sigma;
    [SerializeField, Range(0, 10)] public int KernelRadius;
    [SerializeField] public bool ApplyWeight;
    [SerializeField, Range(0, 1)] public float Weight;
}
