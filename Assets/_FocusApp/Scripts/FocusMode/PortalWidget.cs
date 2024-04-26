using DG.Tweening;
using UnityEngine;

public class PortalWidget : FocusWidget {

    public GameObject portal;
    Vector3 size;
    [SerializeField]
    Ease easeBig;
    [SerializeField]
    Ease easeSmall;

    private void Awake()
    {
        size = portal.transform.localScale;
        portal.transform.localScale = Vector3.zero;

    }

    override
    public void HandleonFadeInStart(float time){

        // portal.SetActive(true);
        portal.transform.DOScale(size, time).SetEase(easeBig);
    }

    override
    public void HandleonFadeOutStart(float time){
        portal.transform.DOScale(Vector3.zero, time).SetEase(easeSmall);
    }


}
