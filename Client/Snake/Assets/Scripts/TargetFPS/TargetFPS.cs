using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    public int SetFps = 60;
    void Start()
    {
        Application.targetFrameRate = SetFps;
    }
}
