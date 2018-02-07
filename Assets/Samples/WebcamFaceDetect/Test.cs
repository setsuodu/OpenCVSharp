using System.Collections;
using UnityEngine;
using OpenCvSharp;

public class Test : MonoBehaviour
{
    Texture2D t = null;

    void Start()
    {
        t = new Texture2D(200, 200, TextureFormat.RGB565, false);

        // Load the cascades
        var haarCascade = new CascadeClassifier(Application.streamingAssetsPath + "/haarcascades/haarcascade_frontalface_alt2.xml");
        var lbpCascade = new CascadeClassifier(Application.streamingAssetsPath + "/lbpcascades/lbpcascade_frontalface.xml");

        // Detect faces
        Mat haarResult = DetectFace(haarCascade);
        Mat lbpResult = DetectFace(lbpCascade);

        byte[] bs = haarResult.ToBytes(".png");
        t.LoadImage(bs);
        t.Apply();

    }

    void OnGUI()
    {
        if (t != null)
        {
            GUI.DrawTexture(new UnityEngine.Rect(0, 0, t.width, t.height), t);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cascade"></param>
    /// <returns></returns>
    private Mat DetectFace(CascadeClassifier cascade)
    {
        Mat result;

        using (var src = new Mat(Application.streamingAssetsPath + "/3.png", ImreadModes.Color))
        using (var gray = new Mat())
        {
            result = src.Clone();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Detect faces
            OpenCvSharp.Rect[] faces = cascade.DetectMultiScale(gray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(30, 30));

            // Render all detected faces
            foreach (OpenCvSharp.Rect face in faces)
            {
                var center = new Point
                {
                    X = (int)(face.X + face.Width * 0.5),
                    Y = (int)(face.Y + face.Height * 0.5)
                };
                var axes = new Size
                {
                    Width = (int)(face.Width * 0.5),
                    Height = (int)(face.Height * 0.5)
                };
                Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
            }
        }
        return result;
    }
}
