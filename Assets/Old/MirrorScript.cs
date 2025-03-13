using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MirrorScript : MonoBehaviour {
    private Camera mainCamera;    // The player's main camera
    public Camera mirrorCamera;  // The mirror's reflection camera
    private RenderTexture renderTexture; // Assigned in the Inspector

    private void Start() {
        if(renderTexture == null) { 
            renderTexture = new RenderTexture(256, 256,24);
            renderTexture.antiAliasing = 2;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.Create();
        }
        if (!mainCamera)
            mainCamera = Camera.main;

        if (!mirrorCamera || !renderTexture) {
            Debug.LogError("Mirror Camera or Render Texture not assigned!");
            return;
        }

        // Assign the render texture to the mirror camera
        mirrorCamera.targetTexture = renderTexture;

        // Assign the texture to the material
        GetComponent<Renderer>().material.mainTexture = renderTexture;
    }

    private void LateUpdate() {
        if (!mirrorCamera || !mainCamera) return;

        // Reflect main camera's position relative to the mirror
        Vector3 mirrorNormal = transform.up; // Plane's normal direction
        Vector3 toCamera = mainCamera.transform.position - transform.position;
        Vector3 reflectedPos = mainCamera.transform.position - 2 * Vector3.Dot(toCamera, mirrorNormal) * mirrorNormal;

        mirrorCamera.transform.position = reflectedPos;

        // Reflect the camera's forward direction
        Vector3 reflectedForward = Vector3.Reflect(mainCamera.transform.forward, mirrorNormal);
        mirrorCamera.transform.forward = reflectedForward;

        // Match camera FOV and other settings
        mirrorCamera.fieldOfView = mainCamera.fieldOfView;
        mirrorCamera.nearClipPlane = mainCamera.nearClipPlane;
        mirrorCamera.farClipPlane = mainCamera.farClipPlane;
    }
}
