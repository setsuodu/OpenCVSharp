using UnityEngine;
using OpenCvSharp;

public class Utils
{
    /// <summary>
    /// Mat转Texture2D
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    public static Texture2D MatToTexture2D(Mat mat)
    {
        Texture2D t2d = new Texture2D(mat.Width, mat.Height);
        t2d.LoadImage(mat.ToBytes());
        t2d.Apply();
        //赋值完后为什么要Apply
        //因为在贴图更改像素时并不是直接对显存进行更改，而是在另外一个内存空间中更改，这时候GPU还会实时读取旧的贴图位置。
        //当Apply后，CPU会告诉GPU你要换个地方读贴图了。
        return t2d;
    }
}
