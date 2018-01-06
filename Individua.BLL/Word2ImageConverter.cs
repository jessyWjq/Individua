using Aspose.Words;
using Aspose.Words.Saving;
using ESBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Individua.BLL
{
    public  class Word2ImageConverter
    {

        private bool cancelled = false;
        public event CbGeneric<int, int> ProgressChanged;
        public event CbGeneric ConvertSucceed;
        public event CbGeneric<string> ConvertFailed;
        /// <summary>
        /// 将Word文档转换为图片      
        /// </summary>
        /// <param name="wordInputPath">Word文件路径</param>
        /// <param name="imageOutputDirPath">图片输出路径，如果为空，默认值为Word所在路径</param>      
        /// <param name="startPageNum">从PDF文档的第几页开始转换，如果为0，默认值为1</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换，如果为0，默认值为Word总页数</param>
        /// <param name="imageFormat">设置所需图片格式，如果为null，默认格式为PNG</param>
        /// <param name="resolution">设置图片的像素，数字越大越清晰，如果为0，默认值为128，建议最大值不要超过1024</param>
        public void ConvertToImage(string wordInputPath, string imageOutputDirPath, int startPageNum, int endPageNum, ImageFormat imageFormat, int resolution, string zskguid, string zskname)
        {

        //    ModifyInMemory.ActivateMemoryPatching();
            try
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(wordInputPath);

                if (doc == null)
                {
                    throw new Exception("Word文件无效或者Word文件被加密！");
                }
                if (imageOutputDirPath.Trim().Length == 0)
                {
                    imageOutputDirPath = Path.GetDirectoryName(wordInputPath);
                }
                if (!Directory.Exists(imageOutputDirPath))
                {
                    Directory.CreateDirectory(imageOutputDirPath);
                }
                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }
                if (endPageNum > doc.PageCount || endPageNum <= 0)
                {
                    endPageNum = doc.PageCount;
                }
                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum; startPageNum = endPageNum; endPageNum = startPageNum;
                }
                if (imageFormat == null)
                {
                    imageFormat = ImageFormat.Png;
                }
                if (resolution <= 0)
                {
                    resolution = 128;
                }
                string imageName = Path.GetFileNameWithoutExtension(wordInputPath);
                ImageSaveOptions imageSaveOptions = new ImageSaveOptions(SaveFormat.Png)
                {
                    Resolution = resolution
                };
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    if (this.cancelled)
                    {
                        break;
                    }
                    MemoryStream stream = new MemoryStream();
                    imageSaveOptions.PageIndex = i - 1;
                    string imgPath = Path.Combine(imageOutputDirPath, imageName) + "_" + i.ToString("000") + "." + imageFormat.ToString();
                    doc.Save(stream, imageSaveOptions);
                    Image img = Image.FromStream(stream);
                    Bitmap bm = ESBasic.Helpers.ImageHelper.Zoom(img, 0.6f);
                    bm.Save(imgPath, imageFormat);
                    img.Dispose();
                    stream.Dispose();
                    bm.Dispose();

                    System.Threading.Thread.Sleep(200);
                    this.ProgressChanged?.Invoke(i - 1, endPageNum);

                    string imgname = zskname + "_" + i.ToString("000");

                    string[] sArray = System.Text.RegularExpressions.Regex.Split(imgPath, "ZSKWJ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    string imgpatch = "ZSKWJ" + sArray[1].ToString();

                }
                if (this.cancelled)
                {
                    return;
                }
                this.ConvertSucceed?.Invoke();
            }
            catch (Exception ex)
            {
                this.ConvertFailed?.Invoke(ex.Message);
            }
        }
    }
}
