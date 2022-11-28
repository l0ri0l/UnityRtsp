using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class VideoPath
{
    public static string Server;
    public static string Port;
    public static string Video;
}

public class MenuViewModel : MonoBehaviour
{
    public GameObject MenuButtons;
    public RectTransform ShowHideButtonRect;
    public RectTransform MenuRect;

    public Toggle SoundToggler;
    public Toggle MusicToggler;
    public TMP_InputField Address;
    public TMP_InputField Port;
    public TMP_InputField File;
    public Slider VolumeSlider;

    private bool isMenuVisible
    {
        get => MenuButtons.activeInHierarchy;
    }

    void Start()
    {
        Address.SetTextWithoutNotify(VideoPath.Server);
        Port.SetTextWithoutNotify(VideoPath.Port);
        File.SetTextWithoutNotify(VideoPath.Video);
        
        Debug.Log(Address.text);
        Debug.Log(VideoPath.Server);
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

    public void OnServerAddressSaveClicked()
    {
        VideoPath.Server = Address.text;
        
        Debug.Log(Address.text);
        Debug.Log(VideoPath.Server);
    }
    
    public void OnPortSaveClicked()
    {
        VideoPath.Port = Port.text;
    }

    public void OnFilepathSaveClicked()
    {
        VideoPath.Video = File.text;
    }

    public void OnToggleSound()
    {
        
    }

    public void OnToggleMusic()
    {
        
    }

    public void OnVolumeChanged()
    {
        Debug.Log(VolumeSlider.value);
    }
}
