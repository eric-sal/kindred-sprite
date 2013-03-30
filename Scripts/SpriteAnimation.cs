using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class SpriteAnimation : MonoBehaviour {
    public AnimationFrameset[] framesets;

    public void Update() {
        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        #endif
    }

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
