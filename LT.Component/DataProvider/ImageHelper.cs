using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace LT.Component.DataProvider
{
    /// <summary>
    /// ImageHelper 的摘要说明。
    /// </summary>
    public class ImageHelper
    {
        public static Bitmap DrawImage(string text, int width, int height, float fontSize, Color fontColor, string fontName, FontStyle fontStyle, Color bgColor)
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(fontName);

            FontFamily family = fonts.Families[0];
            Font font = new Font(family, fontSize, fontStyle, GraphicsUnit.Point);
            SolidBrush brush = new SolidBrush(fontColor);

        TryAgainHere:

            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if (bgColor == Color.Transparent)
            {
                image.MakeTransparent();
            }

            Graphics g = Graphics.FromImage(image);
            SizeF sizef = g.MeasureString(text, font);

            //判断外部传入的宽是否小于字符的实际长度
            var twidth = (int)Math.Ceiling(sizef.Width);
            var theight = (int)Math.Ceiling(sizef.Height);

            if (twidth > width)
            {
                width = twidth;

                image.Dispose();
                g.Dispose();

                goto TryAgainHere;
            }

            if (theight > height)
            {
                height = theight;

                image.Dispose();
                g.Dispose();

                goto TryAgainHere;
            }

            //计算文本在图片中间
            var y = ((float)(height - theight)) / 2;

            //处理透明背景
            if (bgColor != Color.Transparent)
            {
                g.Clear(bgColor);
            }

            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(text, font, brush, 0, y);

            //返回图片
            return image;
        }

        /// <summary>
        /// 直接保存图片
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="fileName">保存目标文件的物理地址</param>
        /// <param name="fileExt">保存目标文件扩展名，含有.</param>
        public static void SaveImage(Image imgPhoto, string fileName, string fileExt)
        {
            imgPhoto.Save(fileName, GetImageFormat(fileExt));
        }

        /// <summary>
        /// 使用图片水印保存
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="fileName">保存目标文件的物理地址</param>
        /// <param name="fileExt">保存目标文件扩展名，含有.</param>
        /// <param name="markPictureSrc">水印图片物理地址</param>
        /// <param name="markPosition">1-9</param>
        /// <param name="markRate">0-100</param>
        public static void SaveImageWithPictureMark(Image imgPhoto, string fileName, string fileExt, string markPictureSrc, int markPosition, int markRate)
        {
            if (!File.Exists(markPictureSrc))
            {
                imgPhoto.Save(fileName, GetImageFormat(fileExt));
                return;
            }

            //define position
            float posX = 5;
            float posY = 5;

            //create a image object containing the photograph to watermark
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto,                               // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0,                                      // x-coordinate of the portion of the source image to draw. 
                0,                                      // y-coordinate of the portion of the source image to draw. 
                phWidth,                                // Width of the portion of the source image to draw. 
                phHeight,                               // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel);                    // Units of measure 

            //create a image object containing the watermark
            Image imgWatermark = new Bitmap(markPictureSrc);
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;

            //to adjust weather to add watermark
            if (phWidth > wmWidth && phHeight > wmHeight)
            {
                //reset position
                GetPosition(ref posX, ref posY, markPosition, phWidth, phHeight, wmWidth, wmHeight);

                //To achieve a transulcent watermark we will apply (2) color 
                //manipulations by defineing a ImageAttributes object and 
                //seting (2) of its properties.
                ImageAttributes imageAttributes = new ImageAttributes();

                //The second color manipulation is used to change the opacity of the 
                //watermark.  This is done by applying a 5x5 matrix that contains the 
                //coordinates for the RGBA space.  By setting the 3rd row and 3rd column 
                //to 0.3f we achive a level of opacity
                float opacity = (float)markRate / 100f;
                float[][] colorMatrixElements = { 
					new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
					new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
					new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
					new float[] {0.0f,  0.0f,  0.0f,  opacity, 0.0f},
					new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                };

                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //draw watermark picture
                grPhoto.DrawImage(imgWatermark,
                    new Rectangle((int)posX, (int)posY, wmWidth, wmHeight),  //Set the detination Position
                    0,                  // x-coordinate of the portion of the source image to draw. 
                    0,                  // y-coordinate of the portion of the source image to draw. 
                    wmWidth,            // Watermark Width
                    wmHeight,		    // Watermark Height
                    GraphicsUnit.Pixel, // Unit of measurment
                    imageAttributes);   //ImageAttributes Object
            }

            //dispose
            grPhoto.Flush();
            grPhoto.Dispose();

            //save new image to file system.
            bmPhoto.Save(fileName, GetImageFormat(fileExt));
            bmPhoto.Dispose();
        }

        /// <summary>
        /// 使用文本水印保存图片
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="fileName">保存目标文件的物理地址</param>
        /// <param name="fileExt">保存目标文件扩展名，含有.</param>
        /// <param name="markText">水印文字</param>
        /// <param name="markPosition">1-9</param>
        public static void SaveImageWithTextMark(Image imgPhoto, string fileName, string fileExt, string markText, int markPosition)
        {
            if (markText == null || markText.Length == 0)
            {
                imgPhoto.Save(fileName, GetImageFormat(fileExt));
                return;
            }

            //define position
            float posX = 5;
            float posY = 5;

            //create a image object containing the photograph to watermark
            //Image imgPhoto = Image.FromStream(HttpFile.InputStream);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto,                               // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0,                                      // x-coordinate of the portion of the source image to draw. 
                0,                                      // y-coordinate of the portion of the source image to draw. 
                phWidth,                                // Width of the portion of the source image to draw. 
                phHeight,                               // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel);                    // Units of measure 


            //-------------------------------------------------------
            //to maximize the size of the markValue message we will 
            //test multiple Font sizes to determine the largest posible 
            //font we can use for the width of the Photograph
            //define an array of point sizes you would like to consider as possiblities
            //-------------------------------------------------------
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };

            Font crFont = null;
            SizeF crSize = new SizeF();

            //Loop through the defined sizes checking the length of the Copyright string
            //If its length in pixles is less then the image width choose this Font size.
            for (int i = 0; i < 7; i++)
            {
                //set a Font object to Arial (i)pt, Bold
                //crFont = new Font("arial", sizes[i], FontStyle.Bold);
                //修正文字的大小与图片的分辨率相关BUG。以72px为标准获取文字的像素宽度。
                crFont = new Font("arial", sizes[i] * (72 / grPhoto.DpiX), FontStyle.Bold);

                //Measure the Copyright string in this Font
                crSize = grPhoto.MeasureString(markText, crFont);

                if ((ushort)crSize.Width < (ushort)phWidth)
                {
                    break;
                }
            }

            //reset position
            GetPosition(ref posX, ref posY, markPosition, phWidth, phHeight, crSize.Width, crSize.Height);

            //define a Brush which is semi trasparent black (Alpha set to 153)
            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            //Draw the markText string
            grPhoto.DrawString(markText, crFont, semiTransBrush2, new PointF(posX + 1, posY + 1));

            //define a Brush which is semi trasparent white (Alpha set to 153)
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            //Draw the markText string a second time to create a shadow effect
            //Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(markText, crFont, semiTransBrush, new PointF(posX, posY));

            //dispose
            grPhoto.Flush();
            grPhoto.Dispose();

            //save new image to file system.
            bmPhoto.Save(fileName, GetImageFormat(fileExt));
            bmPhoto.Dispose();
        }

        /// <summary>
        /// 把图片流保存成缩略图
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="fileName">保存目标文件的物理地址</param>
        /// <param name="fileExt">保存目标文件扩展名，含有.</param>
        /// <param name="maxWidth">最大宽</param>
        /// <param name="maxHeight">最大高</param>
        /// <param name="thumbType">缩略方式：1(等比例缩小),2(自动计算从中间截取),3(使用空白填充法保证大小),4(硬性压缩到指定的大小宽高度)</param>
        public static void SaveThumbImage(Image imgPhoto, string fileName, string fileExt, int maxWidth, int maxHeight, int thumbType)
        {
            //新的缩略图
            Image bitmap = GetThumbImage(imgPhoto, maxWidth, maxHeight, thumbType);

            //保存
            bitmap.Save(fileName, GetImageFormat(fileExt));

            //释放
            bitmap.Dispose();
        }

        /// <summary>
        /// 把图片流保存成缩略图
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="maxWidth">最大宽</param>
        /// <param name="maxHeight">最大高</param>
        /// <param name="thumbType">缩略方式：1(等比例缩小),2(自动计算从中间截取),3(使用空白填充法保证大小),4(硬性压缩到指定的大小宽高度)</param>
        public static Image GetThumbImage(Image imgPhoto, int maxWidth, int maxHeight, int thumbType)
        {
            int nx = 0;
            int ny = 0;
            int nw = maxWidth;
            int nh = maxHeight;

            int ox = 0;
            int oy = 0;
            int ow = imgPhoto.Width;
            int oh = imgPhoto.Height;

            double owh = (double)ow / (double)oh;
            double mwh = (double)maxWidth / (double)maxHeight;

            //选择生成模式
            switch (thumbType)
            {
                case 1:
                    if (owh > mwh)
                    {
                        maxHeight = maxWidth * oh / ow;
                        nh = maxHeight;
                    }
                    else
                    {
                        maxWidth = maxHeight * ow / oh;
                        nw = maxWidth;
                    }
                    break;
                case 2:
                    if (owh > mwh)
                    {
                        int w = oh * maxWidth / maxHeight;
                        ox = (ow - w) / 2;
                        ow = w;
                    }
                    else
                    {
                        int h = ow * maxHeight / maxWidth;
                        oy = (oh - h) / 2;
                        oh = h;
                    }
                    break;
                case 3:
                    if (owh > mwh)
                    {
                        nh = maxWidth * oh / ow;
                        ny = (maxHeight - nh) / 2;
                    }
                    else
                    {
                        nw = maxHeight * ow / oh;
                        nx = (maxWidth - nw) / 2;
                    }
                    break;
            }

            //如果原图比设定的还要小则不缩放处理(按比较处理的情况下)
            if (thumbType == 1 || thumbType == 2)
            {
                if (ow < nw && oh < nh)
                {
                    return (Image)imgPhoto.Clone();
                }
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(maxWidth, maxHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以白背景色填充
            g.Clear(Color.White);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(
                imgPhoto,
                new Rectangle(nx, ny, nw, nh),
                new Rectangle(ox, oy, ow, oh),
                GraphicsUnit.Pixel);

            //释放
            g.Dispose();

            //返回
            return bitmap;
        }

        /// <summary>
        /// 计算水印存放位置
        /// </summary>
        private static void GetPosition(ref float posX, ref float posY, int waterMakrPos, int phWidth, int phHeight, float tgWidth, float tgHeight)
        {
            int offsetX = (int)(phWidth * 0.02);
            int offsetY = (int)(phHeight * .02);

            switch (waterMakrPos)
            {
                case 1: //top left
                    posX = offsetX;
                    posY = offsetY;
                    break;

                case 2: //top middle
                    posX = (phWidth - tgWidth) / 2;
                    posY = offsetY;
                    break;

                case 3: //top right
                    posX = phWidth - tgWidth - offsetX;
                    posY = offsetY;
                    break;

                case 4: //middle left
                    posX = offsetX;
                    posY = (phHeight - tgHeight) / 2;
                    break;

                case 5: //middle middle
                    posX = (phWidth - tgWidth) / 2;
                    posY = (phHeight - tgHeight) / 2;
                    break;

                case 6: //middle right
                    posX = phWidth - tgWidth - offsetX;
                    posY = (phHeight - tgHeight) / 2;
                    break;

                case 7: //bottom left
                    posX = offsetX;
                    posY = phHeight - tgHeight - offsetY;
                    break;

                case 8: //bottom middle
                    posX = (phWidth - tgWidth) / 2;
                    posY = phHeight - tgHeight - offsetY;
                    break;

                case 9: //bottom right
                    posX = phWidth - tgWidth - offsetX;
                    posY = phHeight - tgHeight - offsetY;
                    break;
            }
        }

        /// <summary>
        /// 获取图片类型
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private static ImageFormat GetImageFormat(string ext)
        {
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".png":
                    return ImageFormat.Png;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="bmp">原始图Bitmap</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="bkColor">背景色</param>
        /// <returns>输出Bitmap</returns>
        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            /*临时图像*/
            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            /*画背景的方框*/
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            /*将临时图像移动到背景上*/
            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            //g.DrawImageUnscaled( tmp, 0, 0 );
            g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);

            g.Dispose();
            tmp.Dispose();
            return dst;
        }
        /// <summary>
        /// 图片截剪
        /// </summary>
        /// <param name="bitMap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static Bitmap CutImage(Bitmap bitMap, int x, int y, int width, int height)
        {
            Bitmap bmp1 = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp1);
            g.DrawImage(bitMap, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            g.Dispose();
            return bmp1;
        }

        /// <summary>
        /// 指定区域的图片涂黑
        /// </summary>
        /// <param name="bitMap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap BlackRegion(ref Bitmap bitMap, Color color, int x, int y, int width, int height)
        {
            Graphics g = Graphics.FromImage(bitMap);
            g.FillRectangle(new SolidBrush(color), new Rectangle(x, y, width, height));
            g.Dispose();
            return bitMap;
        }

        /// <summary>
        /// 把图片流保存成缩略图
        /// </summary>
        /// <param name="imgPhoto">图片文件的Image数据流</param>
        /// <param name="maxWidth">最大宽</param>
        /// <param name="maxHeight">最大高</param>
        /// <param name="thumbType">缩略方式：1(等比例缩小),2(自动计算从中间截取),3(使用空白填充法保证大小),4(硬性压缩到指定的大小宽高度)</param>
        public static byte[] SaveThumbImage(Image imgPhoto, int maxWidth, int maxHeight, int thumbType)
        {
            int nx = 0;
            int ny = 0;
            int nw = maxWidth;
            int nh = maxHeight;

            int ox = 0;
            int oy = 0;
            int ow = imgPhoto.Width;
            int oh = imgPhoto.Height;

            double owh = (double)ow / (double)oh;
            double mwh = (double)maxWidth / (double)maxHeight;

            //选择生成模式
            switch (thumbType)
            {
                case 1:
                    if (owh > mwh)
                    {
                        maxHeight = maxWidth * oh / ow;
                        nh = maxHeight;
                    }
                    else
                    {
                        maxWidth = maxHeight * ow / oh;
                        nw = maxWidth;
                    }
                    break;
                case 2:
                    if (owh > mwh)
                    {
                        int w = oh * maxWidth / maxHeight;
                        ox = (ow - w) / 2;
                        ow = w;
                    }
                    else
                    {
                        int h = ow * maxHeight / maxWidth;
                        oy = (oh - h) / 2;
                        oh = h;
                    }
                    break;
                case 3:
                    if (owh > mwh)
                    {
                        nh = maxWidth * oh / ow;
                        ny = (maxHeight - nh) / 2;
                    }
                    else
                    {
                        nw = maxHeight * ow / oh;
                        nx = (maxWidth - nw) / 2;
                    }
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(maxWidth, maxHeight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以白背景色填充
            g.Clear(Color.White);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(
                imgPhoto,
                new Rectangle(nx, ny, nw, nh),
                new Rectangle(ox, oy, ow, oh),
                GraphicsUnit.Pixel);

            MemoryStream currentMs = new MemoryStream();
            //保存
            bitmap.Save(currentMs, imgPhoto.RawFormat);

            byte[] img = currentMs.ToArray();
            //释放
            bitmap.Dispose();
            g.Dispose();
            currentMs.Close();

            return img;

        }
    }
}
