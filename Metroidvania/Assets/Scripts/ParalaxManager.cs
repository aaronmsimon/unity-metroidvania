using UnityEngine;

public class ParalaxManager : MonoBehaviour
{
    [SerializeField] private ParallaxLayer[] layers;
    [SerializeField] private Transform cam;

    private Vector3 lastCameraPosition;

    private void Start() {
        lastCameraPosition = cam.position;
    }

    private void LateUpdate() {
        Vector3 cameraDelta = cam.position - lastCameraPosition;

        foreach (ParallaxLayer layer in layers) {
            float moveX = cameraDelta.x * layer.ParallaxFactor;
            float moveY = cameraDelta.y * layer.ParallaxFactor;

            layer.Layer.position += new Vector3(moveX, moveY, 0);

            lastCameraPosition = cam.position;
        }
    }

    [System.Serializable]
    private class ParallaxLayer
    {
        [SerializeField] private Transform layer;
        [SerializeField][Range(0,1)] private float parallaxFactor;

        public Transform Layer => layer;
        public float ParallaxFactor => parallaxFactor;
    }
}
