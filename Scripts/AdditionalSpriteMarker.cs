using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdditionalSpriteMarker : MonoBehaviour
{
    public GameObject newSpritePrefab; // Reference to the new sprite prefab to be instantiated

    // Method to set the newSpritePrefab variable
    public void SetNewSpritePrefab(GameObject spritePrefab)
    {
        newSpritePrefab = spritePrefab;
    }


    private void OnMouseDown()
    {
        Debug.Log("Mouse Clicked on Additional Sprite Marker");
        SpawnNewSprite();
    }
    private bool HasSpritePrefab2Child(GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.CompareTag("SpritePrefab2"))
            {
                return true;
            }

            // If the child has more children, check recursively
            if (child.childCount > 0)
            {
                if (HasSpritePrefab2Child(child.gameObject))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SpawnNewSprite()
    {
        if (newSpritePrefab != null)
        {
            WandManager wandManager = FindObjectOfType<WandManager>();
            if (wandManager != null && wandManager.GetSpritePrefab2Count() < wandManager.GetSpritePrefab2Limit())
            {
                GameObject newSpriteObject = Instantiate(newSpritePrefab, transform.position, Quaternion.identity);
                // Set the parent of the new sprite object to the same parent as the additional sprite
                newSpriteObject.transform.SetParent(transform.parent);

                // Increment the count of spritePrefab2 instances
                wandManager.IncrementSpritePrefab2Count();
            }
            else
            {
                Debug.Log("Cannot generate more spritePrefab2 instances. Limit reached.");
            }
        }
        else
        {
            Debug.LogWarning("New sprite prefab not assigned in the inspector!");
        }
    }

}
