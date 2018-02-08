using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp; //老版本内存Crash。换最新版成功。。

public class pano : MonoBehaviour
{
    [SerializeField] private Image m_srcImage;
    [SerializeField] private Image m_dstImage;
    Texture2D t2d;
    Mat p1Mat, p2Mat, dstMat;
    bool tryUseGpu = true;
    IEnumerable<Mat> GenerateImages()
    {
        p1Mat = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p1.jpg");
        p2Mat = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p2.jpg");
        yield return p1Mat;
        yield return p2Mat;
    }

    void Start()
    {
        dstMat = new Mat();

        OnOrb();

        OnStitch();
    }

    /// <summary>
    /// 测试函数
    /// </summary>
    void Detect()
    {
        var gray = new Mat(Application.streamingAssetsPath + "/Textures/p1.jpg", ImreadModes.GrayScale);

        KeyPoint[] keyPoints = null;
        using (var orb = ORB.Create(500))
        {
            keyPoints = orb.Detect(gray);
            Debug.Log($"KeyPoint has {keyPoints.Length} items.");
        }
    }

    void DetectAndCompute()
    {
        var gray = new Mat(Application.streamingAssetsPath + "/Textures/p1.jpg", ImreadModes.GrayScale);

        KeyPoint[] keyPoints = null;
        using (var orb = ORB.Create(500))
        using (Mat descriptor = new Mat())
        {
            orb.DetectAndCompute(gray, new Mat(), out keyPoints, descriptor);

            Debug.Log($"keyPoints has {keyPoints.Length} items.");
            Debug.Log($"descriptor has {descriptor.Rows} items.");
        }
    }

    /// <summary>
    /// Orb特征提取
    /// </summary>
    void OnOrb()
    {
        Mat image01 = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p1.jpg");
        Mat image02 = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p2.jpg");

        //灰度图转换
        Mat image1 = new Mat(), image2 = new Mat();
        Cv2.CvtColor(image01, image1, ColorConversionCodes.RGB2GRAY);
        Cv2.CvtColor(image02, image2, ColorConversionCodes.RGB2GRAY);

        KeyPoint[] keyPoint1 = null;
        KeyPoint[] keyPoint2 = null;
        using (ORB orb = ORB.Create(500))
        using (Mat descriptor1 = new Mat())
        using (Mat descriptor2 = new Mat())
        using (var matcher = new BFMatcher())
        {
            //特征点提取并计算
            orb.DetectAndCompute(image1, new Mat(), out keyPoint1, descriptor1);
            orb.DetectAndCompute(image2, new Mat(), out keyPoint2, descriptor2);
            Debug.Log($"keyPoints has {keyPoint1.Length},{keyPoint2.Length} items.");
            Debug.Log($"descriptor has {descriptor1.Rows},{descriptor2.Rows} items.");

            //特征点匹配
            DMatch[] matchePoints = null;
            matchePoints = matcher.Match(descriptor1, descriptor2);

            dstMat = new Mat();
            Cv2.DrawMatches(image01, keyPoint1, image02, keyPoint2, matchePoints, dstMat);
            t2d = Utils.MatToTexture2D(dstMat);
        }

        Sprite dst_sp = Sprite.Create(t2d, new UnityEngine.Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        m_srcImage.sprite = dst_sp;
        m_srcImage.preserveAspect = true;
    }

    /// <summary>
    /// 拼接
    /// </summary>
    void OnStitch()
    {
        IEnumerable<Mat> images = GenerateImages();

        using (var stitcher = Stitcher.Create(tryUseGpu))
        using (var panoMat = new Mat())
        {
            Debug.Log("Stitcher start...");
            var status = stitcher.Stitch(images, panoMat);
            if (status != Stitcher.Status.OK)
            {
                Debug.Log("Can't stitch images, error code = " + (int)status);
                return;
            }
            stitcher.Dispose(); //处理掉
            t2d = Utils.MatToTexture2D(panoMat);
        }

        Sprite sp = Sprite.Create(t2d, new UnityEngine.Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        m_dstImage.sprite = sp;
        m_dstImage.preserveAspect = true;
    }
}
