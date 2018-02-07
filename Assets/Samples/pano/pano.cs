using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp; //老版本内存Crash。换最新版，坚持就是胜利。。

public class pano : MonoBehaviour
{
    [SerializeField] private Image m_srcImage;
    [SerializeField] private Image m_dstImage;
    Texture2D t2d;
    Mat p1Mat, p2Mat, dstMat;
    bool tryUseGpu = true;
    IEnumerable<Mat> GenerateImages()
    {
        yield return p1Mat;
        yield return p2Mat;
    }

    void Start()
    {
        p1Mat = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p1.jpg");
        p2Mat = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/p2.jpg");
        dstMat = new Mat();

        IEnumerable<Mat> images = GenerateImages();
        using (var stitcher = Stitcher.Create(tryUseGpu))
        using (var pano = new Mat())
        {
            Debug.Log("Stitcher start...");
            var status = stitcher.Stitch(images, pano);
            if (status != Stitcher.Status.OK)
            {
                Debug.Log("Can't stitch images, error code = " + (int)status);
                return;
            }
            stitcher.Dispose(); //处理掉
            t2d = Utils.MatToTexture2D(pano);
        }

        Sprite dst_sp = Sprite.Create(t2d, new UnityEngine.Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        m_dstImage.sprite = dst_sp;
        m_dstImage.preserveAspect = true;
    }
}
