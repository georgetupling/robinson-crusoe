using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die
{
    public DieType type { get; private set; }

    public Sprite face0 { get; private set; }
    public Sprite face1 { get; private set; }
    public Sprite face2 { get; private set; }
    public Sprite face3 { get; private set; }
    public Sprite face4 { get; private set; }
    public Sprite face5 { get; private set; }

    public List<Sprite> faces { get; private set; }

    public Die(DieType type, Sprite face0, Sprite face1, Sprite face2, Sprite face3, Sprite face4, Sprite face5) {
        this.type = type;
        this.face0 = face0;
        this.face1 = face1;
        this.face2 = face2;
        this.face3 = face3;
        this.face4 = face4;
        this.face5 = face5;

        faces = new List<Sprite> { face0, face1, face2, face3, face4, face5 };
    }

    public Sprite GetFaceSprite(int face) {
        Sprite faceSprite = null;
        switch (face) {
            case 0: faceSprite = face0; break;
            case 1: faceSprite = face1; break;
            case 2: faceSprite = face2; break;
            case 3: faceSprite = face3; break;
            case 4: faceSprite = face4; break;
            case 5: faceSprite = face5; break;
            default:
                Debug.LogError("Invalid face number! Face number should be between 0 and 5.");
                break;
        }
        return faceSprite;
    }
}
