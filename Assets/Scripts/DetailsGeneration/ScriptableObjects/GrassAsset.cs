using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrassAsset", menuName = "Scriptable objects/Assets/Grass", order = 0)]

public class GrassAsset : ScriptableObject
{
    public Texture2D texture;
    public Vector2 sizeMultiplyer = new Vector2(1, 1);

    public Color healthyColor;
    public Color dryColor;
    
    //public bool aquatic = false
    public Vector2 leavingAltitude = new Vector2(1, 1);
    
    public bool useOriginalColor = false;
}
