using UnityEngine;

namespace NCore
{
    public class FpsOnGUI : MonoBehaviour
    {
        public float fpsMeasuringDelta = 2.0f;
        public int TargetFrame = 30;

        private float mTimePassed;
        private int mFrameCount = 0;
        private float mFPS = 0.0f;

        private GUIStyle style;
        private void Start()
        {
            mTimePassed = 0.0f;
            Application.targetFrameRate = TargetFrame;

            style = new GUIStyle();
            style.normal.background = null;
            style.normal.textColor = new Color(1.0f, 0.5f, 0.0f);
            style.fontSize = 32;
        }

        private void Update()
        {
            ++mFrameCount;
            mTimePassed += Time.deltaTime;

            if (mTimePassed > fpsMeasuringDelta)
            {
                mFPS = mFrameCount / mTimePassed;

                mTimePassed = 0.0f;
                mFrameCount = 0;
            }
        }

        private void OnGUI()
        {
            //居中显示FPS
            GUI.Label(new Rect(0, 0, 200, 200), "FPS: " + mFPS, style);
        }
    }
}