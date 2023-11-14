
using UnityEngine;
using UnityEngine.UI;

public class FPSManager : MonoBehaviour
{

    public Text fpsText;

    [SerializeField]
    private int targetFrameRate = 0;
    [SerializeField]
    private float interval = 0.5f;
    private int frameCount;
    private float countStartAt;

    public float Fps { get; private set; }

    private void Start()
    {
        //if (targetFrameRate > 0)
        //{
        //    Application.targetFrameRate = targetFrameRate;
        //}

        frameCount = 0;
        countStartAt = 0.0f;
        Fps = 0.0f;
    }

    private void Update()
    {
        frameCount += 1;
        var now = Time.realtimeSinceStartup;
        var elapsed = now - countStartAt;
        if (elapsed >= interval)
        {
            Fps = frameCount / elapsed;
            if (fpsText == null)
            {
               // Log.I("FPS: " + Fps.ToString("f2"));
            }
            else {
                fpsText.text = "FPS: " + Fps.ToString("f2");
            }
            

            frameCount = 0;
            countStartAt = now;
        }
    }
}
