using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrassAsset", menuName = "Scriptable objects/Assets/Grass", order = 0)]

public class GrassAsset : ScriptableObject
{
    public Texture2D texture;
    public Vector2 sizeMultiplyer = new Vector2(1, 1);

    [Range(0, 100)]
    public float density = 50f;
    public Color32 healthyColor;
    public Color32 dryColor;
    public Color32 multiplyer;
    
    //public bool aquatic = false
    public Vector2 leavingAltitude = new Vector2(1, 1);
    
    public bool useOriginalColor = false;
}
