using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private ParticleSystem collectFx;
    public float rotationSpeed = 90f;

    private void OnEnable()
    {
        rotationSpeed = Random.Range(-1, 1) > 0 ? Random.Range(-180, -90) : Random.Range(90, 180);

        StartCoroutine(RotationAnimation());
    }


    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }

    IEnumerator RotationAnimation()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

            for (int i = 0; i < 3; i++)
                yield return null;
        }
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
        this.StopAllCoroutines();
    }
}