using SkiaSharp;

namespace HappreeTool.Utils.Documents
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
        /// 裁剪图片的右半边，并返回 JPEG 字节数组。
        /// </summary>
        /// <param name="imageBytes">原图片</param>
        /// <param name="ratio">原图片高比上 poster 宽的比值</param>
        /// <returns>裁剪后的 JPG 字节流</returns>
        public static async Task<byte[]> CropJpgRightWithAspectRatioAsync(byte[] imageBytes, double ratio)
        {
            await using var inputStream = new MemoryStream(imageBytes);
            using var original = SKBitmap.Decode(inputStream) ?? throw new Exception("无法解码输入图像");

            //设定裁剪区域
            int srcWidth = original.Width;
            int srcHeight = original.Height;
            int cropHeight = srcHeight;
            int cropWidth = (int)(cropHeight / ratio);  // Poster的预期宽度

            using var outputStream = new MemoryStream();

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

                using var image = SKImage.FromBitmap(cropped);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, 95);

                data.SaveTo(outputStream);
            }
            else
            {
                // 原图很“瘦”，直接用原图
                return imageBytes;
            }

            return outputStream.ToArray();
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
