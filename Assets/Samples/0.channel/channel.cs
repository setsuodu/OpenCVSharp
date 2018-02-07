using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class channel : MonoBehaviour
{
    [SerializeField] private Image m_srcImage;
    [SerializeField] private Image m_dstImage;
    Texture2D t2d;
    Mat srcMat, dstMat;
    Mat[] channels;

    void Start()
    {
        //imread读取
        srcMat = Cv2.ImRead(Application.streamingAssetsPath + "/Textures/0.jpg");

        //通道提取
        channels = Cv2.Split(srcMat);
        Debug.Log(channels.Length);

        //颜色空间转换
        Mat grayMat = new Mat();
        Cv2.CvtColor(srcMat, grayMat, ColorConversionCodes.RGB2GRAY);

        //通道合并
        dstMat = new Mat();
        Cv2.Merge(channels, dstMat);

        //深拷贝
        Mat img1 = dstMat.Clone();
        Mat img2 = new Mat();
        dstMat.CopyTo(img2);
        Cv2.Flip(img2, img2, FlipMode.X);
        //浅拷贝
        //Mat img3 = dstMat;
        //Cv2.Flip(img3, img3, FlipMode.X);

        t2d = Utils.MatToTexture2D(img2);
        Sprite src_sp = Sprite.Create(t2d, new UnityEngine.Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        m_srcImage.sprite = src_sp;
        m_srcImage.preserveAspect = true;

        t2d = Utils.MatToTexture2D(dstMat);
        Sprite dst_sp = Sprite.Create(t2d, new UnityEngine.Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        m_dstImage.sprite = dst_sp;
        m_dstImage.preserveAspect = true;
    }
}
