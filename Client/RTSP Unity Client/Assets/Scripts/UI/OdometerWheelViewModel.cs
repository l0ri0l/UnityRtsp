using DG.Tweening;
using UnityEngine;

public class OdometerWheelViewModel : MonoBehaviour
{
    public OdometerWheelValueViewModel NextElementTemplate;
    public OdometerWheelValueViewModel CurrentElement;
    public bool isChangeable;
    
    [HideInInspector] public string nextSymbol ="0";
    
    
    private string _currentSymbol = "0";
    
    private void ChangeElement(string digit)
    {
        if (digit != CurrentElement.DigitValue.text)
        {
            var nextElement = Instantiate(NextElementTemplate, transform);
            
            var nextElementRect = nextElement.GetComponent<RectTransform>();
            
            nextElementRect.anchoredPosition = new Vector2(0, 25f);
            nextElement.DigitValue.SetText(digit);

            var currenRect = CurrentElement.transform.GetComponent<RectTransform>();


            currenRect.DOLocalMoveY(-25f, 0.5f);
            nextElementRect.DOLocalMoveY(0f, 0.5f)
              .OnComplete(() =>
              {
                  Destroy(CurrentElement.gameObject);

                  CurrentElement = nextElement;
              });
        }
    }

    void Update()
    {
        // we got web socket message not from main thread and need to return to the main to instantiate new objects
        if ((_currentSymbol != nextSymbol) && isChangeable)
        {
            _currentSymbol = nextSymbol;
            ChangeElement(nextSymbol);
        }
    }
}
