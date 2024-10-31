using UnityEngine;
using UnityEngine.UI;

public class UpgradeImage : MonoBehaviour
{
    public Image[] blankImages;
    public Image[] fillImages;

    public void Start()
    {
        SetAllImagesInactive(fillImages);
        SetAllImagesActive(blankImages);
    }

    public void UpdateImages(int upgradesBought)
    {
        for (int i = 0; i < fillImages.Length; i++)
        {
            fillImages[i].enabled = i < upgradesBought;
        }
    }

    private void SetAllImagesInactive(Image[] images)
    {
        foreach (var image in images)
        {
            image.enabled = false;
        }
    }

    private void SetAllImagesActive(Image[] images)
    {
        foreach (var image in images)
        {
            image.enabled = true;
        }
    }
}