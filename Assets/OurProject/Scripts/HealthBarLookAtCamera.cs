using UnityEngine;

public class HealthBarLookAtCamera : MonoBehaviour
{
    [SerializeField] private Canvas healthBarCanvas;
    private Transform mainCameraTransform;

    private void Awake()
    {
        if (Camera.main != null)
        {
            // Camera.main: FindGameObjectsWithTag("MainCamera") → 検索コスト
            mainCameraTransform = Camera.main.transform;    // キャッシュ → パフォーマンス向上
        }
    }

    private void LateUpdate()
    {
        if (mainCameraTransform != null && healthBarCanvas != null)
        {
            healthBarCanvas.transform.rotation = mainCameraTransform.rotation;
        }
    }
}
