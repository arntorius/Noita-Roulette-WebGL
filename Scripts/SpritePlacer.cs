using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SpritePlacer : MonoBehaviour
{
    public GameObject spritePrefab;
    public LayerMask placementLayer;
    public int maxSprites = 120; // Maximum number of sprites allowed

    private bool allowPlacement = false;
    private List<GameObject> placedSprites = new List<GameObject>();

    void Update()
    {
        // Check if placement is allowed
        if (allowPlacement)
        {
            // Left mouse button to place sprite
            if (Input.GetMouseButtonDown(0))
            {
                if (placedSprites.Count < maxSprites)
                {
                    PlaceSprite();
                }
                else
                {
                    Debug.Log("Maximum number of sprites placed. Cannot place more.");
                }
            }

            // Right mouse button to destroy sprite
            if (Input.GetMouseButtonDown(1))
            {
                DestroySprite();
            }
        }
    }

    void PlaceSprite()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse position

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
        {
            Vector3 spawnPosition = hit.point;
            GameObject newSprite = Instantiate(spritePrefab, spawnPosition, Quaternion.identity);
            placedSprites.Add(newSprite); // Add the newly placed sprite to the list
            Debug.Log("Sprite placed: " + newSprite.name);
        }
    }

    void DestroySprite()
    {
        // Check if there are any placed sprites
        if (placedSprites.Count > 0)
        {
            GameObject lastPlacedSprite = placedSprites[placedSprites.Count - 1];
            placedSprites.Remove(lastPlacedSprite); // Remove the last sprite from the list
            Destroy(lastPlacedSprite); // Destroy the last placed sprite
            Debug.Log("Last placed sprite destroyed: " + lastPlacedSprite.name);
        }
        else
        {
            Debug.Log("No placed sprites to destroy.");
        }
    }

    // Method to allow placing sprites
    public void AllowPlacement()
    {
        allowPlacement = true;
    }

    // Method to reset newly placed sprites and disallow placing sprites
    public void ResetSprites()
    {
        allowPlacement = false;

        // Destroy all newly placed sprites
        foreach (GameObject placedSprite in placedSprites)
        {
            Destroy(placedSprite);
        }

        // Clear the list of placed sprites
        placedSprites.Clear();
    }
}
