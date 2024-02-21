using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClickedStateData", menuName = "ScriptableObjects/ClickedStateData", order = 1)]
public class ClickedStateData : ScriptableObject
{
    [System.Serializable]
    public class ClickedStateEntry
    {
        public string spriteName;
        public bool clicked;
    }

    public List<ClickedStateEntry> clickedStates = new List<ClickedStateEntry>();

    // Method to get names of clicked sprites
    public List<string> GetClickedSpriteNames()
    {
        List<string> clickedSpriteNames = new List<string>();
        foreach (ClickedStateEntry entry in clickedStates)
        {
            if (entry.clicked)
            {
                clickedSpriteNames.Add(entry.spriteName);
            }
        }
        return clickedSpriteNames;
    }

    public void SetClickedState(string spriteName, bool clicked)
    {
        // Check if the sprite name exists in the list
        ClickedStateEntry entry = clickedStates.Find(x => x.spriteName == spriteName);
        if (entry != null)
        {
            entry.clicked = clicked;
        }
        else
        {
            // If the sprite name does not exist, add a new entry
            clickedStates.Add(new ClickedStateEntry() { spriteName = spriteName, clicked = clicked });
        }
    }

    public bool GetClickedState(string spriteName)
    {
        // Find the clicked state for the given sprite name
        ClickedStateEntry entry = clickedStates.Find(x => x.spriteName == spriteName);
        if (entry != null)
        {
            return entry.clicked;
        }
        // If the sprite name is not found, return false (not clicked)
        return false;
    }
}
