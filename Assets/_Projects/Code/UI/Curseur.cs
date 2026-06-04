using UnityEngine;
using DG.Tweening;

public class Curseur : MonoBehaviour
{
    public Transform target;
    public float duration;
    public Ease ease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOMove(target.position, duration).SetLoops(-1,LoopType.Yoyo).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
