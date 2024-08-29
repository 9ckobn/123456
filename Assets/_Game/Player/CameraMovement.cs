using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform LookAt;
    public Player player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float initialFov = 60f;
    public float speedToFovFactor = 0.5f;
    private Camera cam;

    // Variables for camera shake
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;
    private Vector3 originalPos;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = initialFov;
        originalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        float targetFov = initialFov + (1) * speedToFovFactor;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, smoothSpeed);

        transform.LookAt(LookAt);

        // If shaking, apply shake effect
        if (isShaking)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
        }
    }

    // Method to start shaking the camera
    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        originalPos = transform.localPosition;
        StartCoroutine(Shake());
    }

    // Coroutine to handle the shaking
    IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
        transform.localPosition = originalPos;
    }
}