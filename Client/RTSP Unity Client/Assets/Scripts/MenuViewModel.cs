using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MenuViewModel : MonoBehaviour
{
    public GameObject MenuButtons;
    public RectTransform ShowHideButtonRect;
    public RectTransform MenuRect;

    private bool isMenuVisible
    {
        get => MenuButtons.activeInHierarchy;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShowHideButtonClicked()
    {
        Debug.Log("Clicked Menu");
        if (!isMenuVisible)
        {
            MenuButtons.SetActive(true);
            MenuRect.DOLocalMoveX(MenuRect.rect.width, 1f);
            ShowHideButtonRect.DORotate(new Vector3(0, 0, 0), 1f);
        }
        else
        {
            ShowHideButtonRect.DORotate(new Vector3(0, 180, 0), 1f);
            MenuRect.DOLocalMoveX(0, 1f).OnComplete(() =>
            {
                MenuButtons.SetActive(false);
            });
        }
    }
}
