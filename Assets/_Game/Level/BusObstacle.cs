using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BusObstacle : MonoBehaviour, IObstacle
{
    public int OnCollideGetDamage() => 2;
    public void DisableObstacle()
    {
        this.enabled = false;
        GetComponent<Rigidbody>().detectCollisions = false;
    }

    private void OnValidate()
    {
        // var rb = GetComponent<Rigidbody>();
        //
        // rb.isKinematic = true;
        // rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}