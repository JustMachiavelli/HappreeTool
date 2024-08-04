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
            if (!File.Exists(imagePath))
                return false;

            // 尝试加载图像
            using var stream = File.OpenRead(imagePath);
            using var codec = SKCodec.Create(stream, out var result);
            if (codec == null || result != SKCodecResult.Success)
            {
                // 图像无效
                return false;
            }
            return true;
        }

        /// <summary>
        /// 裁剪图片的右半边
        /// </summary>
        /// <param name="inputImagePath">原图片路径</param>
        /// <param name="outputImagePath">保存新图片路径</param>
        /// <param name="ratio">原图片高比上poster宽的比值</param>
        public static void CropJpgRightWithAspectRatio(string inputImagePath, string outputImagePath, double ratio)
        {
            using var inputStream = File.OpenRead(inputImagePath);
            using var original = SKBitmap.Decode(inputStream) ?? throw new Exception("无法解码输入图像.");

            //设定裁剪区域
            int srcWidth = original.Width;
            int srcHeight = original.Height;
            int cropHeight = srcHeight;
            int cropWidth = (int)(cropHeight / ratio);  // Poster的预期宽度

            //判定是否需要裁剪，还是直接用原图
            if (srcWidth > cropWidth)
            {
                // 长比高大的原图，裁剪右边
                int destWidth = cropWidth;  // Poster的预期宽度
                int startX = srcWidth - destWidth;  // 左上角X坐标

                SKRectI cropRect = new SKRectI(startX, 0, startX + cropWidth, cropHeight);
                // 创建新的裁剪图像
                using var cropped = new SKBitmap(cropWidth, cropHeight);
                using var canvas = new SKCanvas(cropped);

                canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(original, cropRect, new SKRect(0, 0, cropWidth, cropHeight));

                // 保存裁剪后的图像
                using var outputStream = File.OpenWrite(outputImagePath);
                cropped.Encode(outputStream, SKEncodedImageFormat.Jpeg, 95);
            }
            else
            {
                // 原图很“瘦”，直接复制一份原图
                File.Copy(inputImagePath, outputImagePath, true);
            }
        }

        /// <summary>
        /// 获取图片的宽度和高度
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns>图片的宽度和高度</returns>
        public static (int, int) GetImageDimensions(string imagePath)
        {
            using var inputStream = File.OpenRead(imagePath);
            using var codec = SKCodec.Create(inputStream);
            if (codec == null)
            {
                throw new Exception("无法解码输入图像.");
            }

            return (codec.Info.Width, codec.Info.Height);
        }

    }

}
