using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private ParticleSystem collectFx;

    private void OnEnable()
    {
        transform.DORotate(new Vector3(0, 180, 0), Random.Range(0.5f, 0.85f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void Collect()
    {
        //todo play some sound
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.AddCoin = 1;
            Collect();
        }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}