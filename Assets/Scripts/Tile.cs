using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class Tile : MonoBehaviour
{
    [Header("WFC Neighbours")]
    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] downNeighbours;
    public Tile[] leftNeighbours;

    [Header("Public Data")]
    public float weight = 1;
}


/*
[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Tile tile = (Tile)target;

        if (GUILayout.Button("Match Neighbors"))
        {
            // Match left neighbors
            MatchNeighbors(tile.leftNeighbours, tile, Direction.Left);
            
            // Match right neighbors
            MatchNeighbors(tile.rightNeighbours, tile, Direction.Right);
            
            // Match up neighbors
            MatchNeighbors(tile.upNeighbours, tile, Direction.Up);
            
            // Match down neighbors
            MatchNeighbors(tile.downNeighbours, tile, Direction.Down);

            Debug.Log("Matching neighbors complete.");
        }
    }

    private void MatchNeighbors(Tile[] neighbors, Tile currentTile, Direction direction)
    {
        if (neighbors != null)
        {
            foreach (Tile neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            AddNeighbor(neighbor, currentTile, Direction.Right);
                            break;
                        case Direction.Right:
                            AddNeighbor(neighbor, currentTile, Direction.Left);
                            break;
                        case Direction.Up:
                            AddNeighbor(neighbor, currentTile, Direction.Down);
                            break;
                        case Direction.Down:
                            AddNeighbor(neighbor, currentTile, Direction.Up);
                            break;
                    }
                }
            }
        }
    }

    private void AddNeighbor(Tile neighbor, Tile currentTile, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                if (!ContainsNeighbor(neighbor.leftNeighbours, currentTile))
                {
                    ArrayUtility.Add(ref neighbor.leftNeighbours, currentTile);
                    EditorUtility.SetDirty(neighbor);
                }
                break;
            case Direction.Right:
                if (!ContainsNeighbor(neighbor.rightNeighbours, currentTile))
                {
                    ArrayUtility.Add(ref neighbor.rightNeighbours, currentTile);
                    EditorUtility.SetDirty(neighbor);
                }
                break;
            case Direction.Up:
                if (!ContainsNeighbor(neighbor.upNeighbours, currentTile))
                {
                    ArrayUtility.Add(ref neighbor.upNeighbours, currentTile);
                    EditorUtility.SetDirty(neighbor);
                }
                break;
            case Direction.Down:
                if (!ContainsNeighbor(neighbor.downNeighbours, currentTile))
                {
                    ArrayUtility.Add(ref neighbor.downNeighbours, currentTile);
                    EditorUtility.SetDirty(neighbor);
                }
                break;
        }
    }

    private bool ContainsNeighbor(Tile[] neighbors, Tile tile)
    {
        if (neighbors != null)
        {
            foreach (Tile neighbor in neighbors)
            {
                if (neighbor == tile)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    */

