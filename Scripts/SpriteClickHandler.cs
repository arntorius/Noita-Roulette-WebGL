using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteClickHandler : MonoBehaviour
{
    private SpriteManager spriteManager;
    private int spriteIndex;
    private AudioSource audioSource;

    public void Initialize(SpriteManager manager, int index, AudioSource source)
    {
        spriteManager = manager;
        spriteIndex = index;
        audioSource = source;
    }
}
