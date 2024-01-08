using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraHandler : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    public Camera Camera => sceneCamera;
}
