using Org.BouncyCastle.Asn1.Pkcs;
using SkiaSharp;

namespace HappreeTool.Documents
{
    public static class PictureUtils
    {
        /// <summary>
        /// 查看图片是否存在，能否打开，有没有损坏
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <remarks>https://github.com/mono/SkiaSharp/issues/1146</remarks>
        /// <returns>图片是否正常</returns>
        public static bool CheckPicture(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
                return false;

            // 尝试加载图像
            using var stream = System.IO.File.OpenRead(imagePath);
            using var codec = SKCodec.Create(stream, out var result);
            if (codec == null || result != SKCodecResult.Success)
            {
                // 图像无效
                return false;
            }
            return true;
        }

        public static void CropKeepRightWithAspectRatio(string pathFanart, string pathPoster, double ratio)
        {
            using (var stream = File.OpenRead(pathFanart))
            {
                using (var skBitmap = SKBitmap.Decode(stream))
                {
                    int srcWidth = skBitmap.Width;
                    int srcHeight = skBitmap.Height;
                    int wantWidth = (int)(srcHeight / ratio);  // Poster的预期宽度
                    if (srcWidth > wantWidth)
                    {
                        // 正常的fanart，裁剪右边
                        int destWidth = wantWidth;  // Poster的预期宽度
                        int destX = srcWidth - destWidth;  // 左上角X坐标

                        using (var skCanvas = new SKCanvas(skBitmap))
                        {
                            // 裁剪右边
                            skCanvas.ClipRect(new SKRect(destX, 0, srcWidth, srcHeight));

                            // 保存图像为 JPEG 格式
                            SaveAsJpeg(skBitmap, pathPoster);
                        }
                    }
                    else
                    {
                        // fanart很“瘦”，Poster直接复制一份fanart
                        File.Copy(pathFanart, pathPoster, true);
                    }
                }
            }
        }

        private static void SaveAsJpeg(SKBitmap skBitmap, string outputPath)
        {
            using (var image = SKImage.FromBitmap(skBitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
            using (var stream = File.OpenWrite(outputPath))
            {
                data.SaveTo(stream);
            }
        }


    }

}
