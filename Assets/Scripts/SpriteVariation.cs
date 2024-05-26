
using System.Collections.Generic;
using UnityEngine;

public class SpriteVariation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Sprite> spriteVariations;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteVariations.Count > 0)
        {
            spriteRenderer.sprite = spriteVariations[Random.Range(0, spriteVariations.Count)];
        }
    }
}