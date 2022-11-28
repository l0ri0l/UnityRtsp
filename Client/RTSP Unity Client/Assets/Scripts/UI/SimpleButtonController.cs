using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioSource onExitEnterSource;
    public AudioSource onClickSource;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onExitEnterSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExitEnterSource.Play();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClickSource.Play();
    }
}
