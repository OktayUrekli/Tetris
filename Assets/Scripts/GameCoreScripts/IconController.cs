using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconController : MonoBehaviour
{
    [SerializeField] Sprite openIcon;
    [SerializeField] Sprite closeIcon;

    Image butonImage;

    bool iconState = true;

    void Start()
    {
        butonImage = GetComponent<Image>();
        butonImage.sprite=(iconState) ? openIcon : closeIcon;
    }

    public void UpdateIconState(bool state)
    {
        if (!openIcon || !closeIcon || !butonImage)
        { return; }
        else { butonImage.sprite = (state) ? openIcon : closeIcon; }
    }
}
