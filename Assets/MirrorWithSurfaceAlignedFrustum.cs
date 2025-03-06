using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Renderer))]
public class MirrorWithSurfaceAlignedFrustum : MonoBehaviour {
    private Camera mainCamera;
    private Camera mirrorCamera;
    private RenderTexture renderTexture;
    private int textureWidth = 1024;
    private int textureHeight = 1024;
    private Transform mirrorTransform;

    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    private void Start() {
        mainCamera = Camera.main;
        mirrorTransform = transform;

        if (!mainCamera) {
            Debug.LogError("No camera with the 'MainCamera' tag found!");
            return;
        }

        // Create a new mirror camera dynamically
        GameObject mirrorCamObj = new GameObject("MirrorCamera");
        mirrorCamera = mirrorCamObj.AddComponent<Camera>();
        mirrorCamera.targetDisplay = 2;

        // Set mirror camera properties
        mirrorCamera.enabled = false;
        mirrorCamera.cullingMask = mainCamera.cullingMask;
        mirrorCamera.clearFlags = CameraClearFlags.Skybox;

        // Create the Render Texture (Simulates an FBO)
        renderTexture = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 2;
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.Create();

        // Assign the render texture to the mirror camera
        mirrorCamera.targetTexture = renderTexture;

        // Assign the render texture to the mirror material
        GetComponent<Renderer>().material.mainTexture = renderTexture;



        // Render first frame
        RenderMirror();
    }
    private void Update() {
        RenderMirror();

    }
    private void LateUpdate() {
    }

    public void RenderMirrorWithCamera(
        Camera mainCamera)
    { }


    private void RenderMirror() {
        if (!mirrorCamera || !mainCamera) return;

        mirrorCamera.transform.position = CalculateMirrorCameraPosition(mirrorTransform, mainCamera.transform.position);

        Matrix4x4 viewMatrix = getCustomViewMatrix(mirrorCamera.transform.position);
        
        float distance = (mirrorTransform.position - mirrorCamera.transform.position).magnitude;
        Matrix4x4 projectionMatrix = getCustomProjectionMatrix(mirrorTransform, distance, distance + 1000, mirrorCamera.transform.position);
        Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(projectionMatrix, false);
        mirrorCamera.projectionMatrix = gpuProj;
        mirrorCamera.Render();
    }

    private Vector3 CalculateMirrorCameraPosition(Transform mirrorTransform, Vector3 mainCameraPosition) {
        Vector3 mirrorNormal = mirrorTransform.forward;
        Debug.DrawRay(mirrorTransform.position, mirrorNormal,Color.cyan);
        Vector3 toCamera = mainCameraPosition - mirrorTransform.position;
        //Vector3 mainForward = mainCamera.transform.forward;
        //Vector3 reflectedForward = mainForward - 2 * Vector3.Dot(mainForward, mirrorNormal) * mirrorNormal;
        //mirrorCamera.transform.rotation = Quaternion.LookRotation(reflectedForward, Vector3.up);
        Vector3 reflectedPos = mainCameraPosition - 2 * Vector3.Dot(toCamera, mirrorNormal) * mirrorNormal;
         Vector3 mainUp = mainCamera.transform.up;
        Vector3 reflectedUp = mainUp - 2 * Vector3.Dot(mainUp, mirrorNormal) * mirrorNormal;
        mirrorCamera.transform.rotation = mirrorTransform.rotation;
        return reflectedPos;
    }
    private bool isCameraFrontOfQuad(Vector3 cameraPosition, Transform quadTransform) {
        Vector3 toCamera = cameraPosition - quadTransform.position;
        float dot = Vector3.Dot(quadTransform.forward, toCamera);
        return dot >= 0;
    }

    private Matrix4x4 getCustomViewMatrix(Vector3 cameraPosition) {
        return Matrix4x4.Translate(cameraPosition);
    }

    private Matrix4x4 getCustomProjectionMatrix(Transform quadTransform,float nearClip,float farClip,Vector3 cameraPosition) {
        // Calculate the quad corners in world space using its transform.
        // Assuming a standard quad mesh with local coordinates:
        // Bottom-Left (-0.5, -0.5, 0)
        // Bottom-Right (0.5, -0.5, 0)
        // Top-Left (-0.5, 0.5, 0)
        Vector3 pa = quadTransform.TransformPoint(new Vector3(-0.5f, -0.5f, 0f)); // Bottom-Left
        Vector3 pb = quadTransform.TransformPoint(new Vector3(0.5f, -0.5f, 0f));  // Bottom-Right
        Vector3 pc = quadTransform.TransformPoint(new Vector3(-0.5f, 0.5f, 0f));  // Top-Left

        Vector3 bottomLeft = quadTransform.TransformPoint(new Vector3(-0.5f, -0.5f, 0f)); // pa
        Vector3 bottomRight = quadTransform.TransformPoint(new Vector3(0.5f, -0.5f, 0f));  // pb
        Vector3 topLeft = quadTransform.TransformPoint(new Vector3(-0.5f, 0.5f, 0f));  // pc
        Vector3 topRight = quadTransform.TransformPoint(new Vector3(0.5f, 0.5f, 0f));   // pd
        

        // Draw the rectangle edges
        Debug.DrawLine(bottomLeft, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, topRight, Color.red);
        Debug.DrawLine(topRight, topLeft, Color.red);
        Debug.DrawLine(topLeft, bottomLeft, Color.red);

        // (Optional) Draw diagonals for clarity
        Debug.DrawLine(bottomLeft, topRight, Color.yellow);
        Debug.DrawLine(bottomRight, topLeft, Color.yellow);

        // Get the eye position from the camera.
        Vector3 pe = cameraPosition;

        // Compute the screen's basis vectors:
        // Right vector (from bottom-left to bottom-right)
        Vector3 vr = (pb - pa).normalized;
        // Up vector (from bottom-left to top-left)
        Vector3 vu = (pc - pa).normalized;
        // The screen normal is the cross product of these two.
        Vector3 vn = Vector3.Cross(vr, vu).normalized;

        Debug.DrawRay(bottomLeft, vr, Color.blue);
        Debug.DrawRay(bottomLeft, vu, Color.blue);
        Debug.DrawRay(bottomLeft, vn,Color.blue);

        // Calculate the distance from the eye to the screen (along the screen normal).
        // This should be positive if the eye is in front of the screen.

        float d = Vector3.Dot(vn,pa - pe);
        if (d < 0) {
            Debug.Log("Negative");
            //d = -d;
        }
        if (d < 0.001f) // Avoid division by zero
        {
            Debug.Log("Avoid division by zero");
        }
        
        Debug.Log("d: " + d + " distance: "+ nearClip+" far clip: " + farClip);

        // Compute the extents of the screen in eye space at the near clip plane.
        // These extents are found by projecting the vectors from the eye to each corner
        // onto the screen basis and then scaling to the near plane.
        float l = Vector3.Dot(vr, pa-pe) * nearClip / d;
        float r = Vector3.Dot(vr, pb-pe) * nearClip / d;
        float b = Vector3.Dot(vu, pa-pe) * nearClip / d;
        float t = Vector3.Dot(vu, pc-pe) * nearClip / d;

        // Create the off-axis projection matrix based on these extents.
        return Matrix4x4.Frustum(-l, -r, -b, -t, -nearClip, -farClip);

    }
}
