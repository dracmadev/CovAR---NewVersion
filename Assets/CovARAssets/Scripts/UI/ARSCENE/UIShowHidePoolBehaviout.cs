using UnityEngine;
using UnityEngine.UI;

public class UIShowHidePoolBehaviout : MonoBehaviour
{


    [Header("SHOW & HIDE POOL:")]
    [SerializeField] private Sprite _PoolIsActiveSprite;
    [SerializeField] private Sprite _PoolIsNotActiveSprite;

    Button myButton;

    ARObjectManager _ARObjectManager;
    void Start()
    {
        InitShowAndHidePool();
    }

    // Update is called once per frame
    void Update()
    {
        OnShowAndHidePoolBehaviour();
    }

    /////////////////////////////////////////////////////////////////// INIT ////////////////////////////////////////////////////////////////////////////////////////

    void InitShowAndHidePool()
    {
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }
        if(this.gameObject.GetComponent<Button>())
        {
            myButton = this.gameObject.GetComponent<Button>();
        }
      
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////// BEHAVIOUR //////////////////////////////////////////////////////////////////////////////////

    void OnShowAndHidePoolBehaviour()
    {
        if (_ARObjectManager != null && myButton != null)
        {
            if (_ARObjectManager.GetPoolActiveState())
            {
               myButton.image.sprite = _PoolIsActiveSprite;
            }
            else
            {
                myButton.image.sprite = _PoolIsNotActiveSprite;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
