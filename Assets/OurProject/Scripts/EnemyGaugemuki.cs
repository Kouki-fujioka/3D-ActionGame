using UnityEngine;

public class EnemyGaugemuki : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    void Update()
    {
        //EnemyGaugeをMain Cameraへ向かせる
        canvas.transform.rotation = Camera.main.transform.rotation;
    }
}