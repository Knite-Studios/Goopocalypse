using System;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [SerializeField] private Vector2 targetPos;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease = Ease.Linear;

    public float Duration
    {
        get => duration;
        set => duration = value;
    }

    private Tween currentTween;
    private Vector2 startPos;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool isPosChanged;
    private bool isScaleChanged;
    private bool isFadeChanged;

    public UnityEvent OnAnimateStarted;
    public UnityEvent OnAnimateFinished;


    private Queue<bool> growCommands = new ();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPos = rectTransform.anchoredPosition;
        isPosChanged = rectTransform.anchoredPosition != startPos;
        isScaleChanged = rectTransform.localScale == Vector3.one;
        isFadeChanged = canvasGroup.alpha != 0;
    }

    public void MoveAnimate()
    {
        if (currentTween.IsActive())
            return;

        currentTween?.Kill();
       
        OnAnimateStarted.Invoke();

        if (isPosChanged)
            currentTween = rectTransform.DOAnchorPos(startPos, duration).SetEase(ease);
        else
            currentTween = rectTransform.DOAnchorPos(targetPos, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            currentTween.Kill();
            isPosChanged = !isPosChanged;
            OnAnimateFinished.Invoke();
        });
    }

    public void MoveInAnimate(bool check)
    {
        
        if (currentTween.IsActive())
            currentTween.Kill();
    

        OnAnimateStarted.Invoke();

        if (!check)
            currentTween = rectTransform.DOAnchorPos(startPos, duration).SetEase(ease);
        else
            currentTween = rectTransform.DOAnchorPos(targetPos, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            currentTween.Kill();
            isPosChanged = !isPosChanged;
            OnAnimateFinished.Invoke();
        });
    }


    public void GrowAnimate()
    {
        if (currentTween.IsActive())
            return;
        OnAnimateStarted.Invoke();

        if (!isScaleChanged)
            currentTween = rectTransform.DOScale(Vector3.zero, duration).SetEase(ease);
        else
            currentTween = rectTransform.DOScale(Vector3.one, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            isScaleChanged = !isScaleChanged;
            OnAnimateFinished.Invoke();
        });

    }

    public void GrowInAnimate(bool scale)
    {
        if(isScaleChanged == scale)
            return;

        if (currentTween.IsActive() && isScaleChanged != scale)
        {
            currentTween.Kill(); 
        }
        OnAnimateStarted.Invoke();

        isScaleChanged = scale;

        if (!scale)
            currentTween = rectTransform.DOScale(Vector3.zero, duration).SetEase(ease);
        else
            currentTween = rectTransform.DOScale(Vector3.one, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            currentTween.Kill();
            OnAnimateFinished.Invoke();
        });

    }

    public void FadeAnimate()
    {
        if (currentTween.IsActive())
            return;
        currentTween.Kill();

        if (isFadeChanged)
            currentTween = canvasGroup.DOFade(0, duration).SetEase(ease);
        else
            currentTween = canvasGroup.DOFade(1, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            isFadeChanged = !isFadeChanged;
            OnAnimateFinished.Invoke();
        });

    }

    public void FadeInAnimate(bool fade)
    {
        
        currentTween?.Kill();
        
        if (fade)
            currentTween = canvasGroup.DOFade(1, duration).SetEase(ease).SetUpdate(true);
        else
            currentTween = canvasGroup.DOFade(0, duration).SetEase(ease).SetUpdate(true);

        currentTween.OnComplete(() =>
        {
            isFadeChanged = fade;
            OnAnimateFinished.Invoke();
        });

    }


    public void FadeBack()
    {
        if (currentTween.IsActive())
            return;
        
        

        if (isFadeChanged)
            currentTween = canvasGroup.DOFade(0, duration).SetEase(ease);
        else
            currentTween = canvasGroup.DOFade(1, duration).SetEase(ease);

        currentTween.OnComplete(() =>
        {
            isFadeChanged = !isFadeChanged;
        });
    }

    public void RotateAnimate()
    {
        if (currentTween.IsActive())
            return;
        
        currentTween?.Kill();

        currentTween = rectTransform.DORotate(new Vector3(0, 0, -180), duration).SetEase(ease).SetLoops(-1);

    }

    private void OnDestroy()
    {
        currentTween?.Kill();
    }
}
