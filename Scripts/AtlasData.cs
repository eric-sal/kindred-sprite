using UnityEngine;
using System.Collections;

[System.Serializable]

/// <summary>
/// Atlas data about a frame on a sprite sheet
/// </summary>
public class AtlasData {
    public string name = "";
    public Vector2 position = Vector2.zero;
    public Vector2 offset = Vector2.zero;
    public bool rotated = false;
    public Vector2 size = Vector2.zero;
    public Vector2 frameSize = Vector2.zero;
}
