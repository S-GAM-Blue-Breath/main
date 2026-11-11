using System;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectionHud : MonoBehaviour
{
    [Serializable]
    private class FishSlot
    {
        public string fishName;
        public Image slotImage;
        public Sprite collectedSprite;
    }

    [SerializeField] private FishSlot[] slots;

    public void RevealFish(string fishName)
    {
        foreach (FishSlot slot in slots)
        {
            if (slot.fishName != fishName)
            {
                continue;
            }

            if (slot.slotImage.sprite != slot.collectedSprite)
            {
                slot.slotImage.sprite = slot.collectedSprite;
            }

            return;
        }
    }
}