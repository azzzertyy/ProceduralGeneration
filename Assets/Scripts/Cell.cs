using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public bool visited = false;
    public Tile[] tileOptions;
    public bool isLadder;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color minColor = Color.cyan;
    private Color maxColor = Color.magenta;
    private Color errorColor = Color.yellow;

    private const int minTileOptions = 1;
    private int maxTileOptions = 10;

    private void Update()
    {
        if(collapsed)
        {
            spriteRenderer.enabled = false;
            enabled = false;
        }
        UpdateSpriteColor();
    }

    public void CreateCell(bool collapseState, Tile[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
        maxTileOptions = tiles.Count();
    }

    public void RecreateCell(Tile[] tiles)
    {
        tileOptions = tiles;

        UpdateSpriteColor();
    }

    private void UpdateSpriteColor()
    {
        if(tileOptions.Length < 1)
        {
            spriteRenderer.color = errorColor;
            return;
        }
        float ratio = Mathf.Clamp01((float)(tileOptions.Length - minTileOptions) / (maxTileOptions - minTileOptions));
        spriteRenderer.color = Color.Lerp(minColor, maxColor, ratio);
    }

    public void RemoveOption(Tile tileToRemove)
    {
        List<Tile> newOptions = new List<Tile>(tileOptions);
        newOptions.Remove(tileToRemove);
        tileOptions = newOptions.ToArray();
    }
}
