using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class SpriteAnimation : MonoBehaviour {
    public AnimationFrameset[] framesets;

    public AnimationFrameset GetFrameset(string framesetName) {
        AnimationFrameset frameset = null;

        if (framesetName == "") {
            return null;
        }

        for (int f = 0; f < framesets.Length; f++) {
            if (framesets[f].name.ToLower() == framesetName.ToLower()) {
                frameset = framesets[f];
                break;
            }
        }

        return frameset;
    }
}
