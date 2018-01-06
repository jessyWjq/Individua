using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.Office
{
    public class WordBuilder
    {
        XWPFDocument doc = null;
        private const string DEFAULT_FONTFAMILY = "微软雅黑";
        private const int DEFAULT_FONTSIZE = 12;
        public WordBuilder()
        {
            doc = new XWPFDocument();
        }

        public WordBuilder(string title)
        {
            doc = new XWPFDocument();
            XWPFParagraph p = doc.CreateParagraph();
            p.Alignment = ParagraphAlignment.CENTER;
            XWPFRun r = p.CreateRun();
            r.FontFamily = DEFAULT_FONTFAMILY;
            r.FontSize = 18;
            r.IsBold = true;
            r.SetText(title);
        }
        public WordBuilder(string title, FontStyle fontStyle)
        {
            doc = new XWPFDocument();
            XWPFParagraph p = doc.CreateParagraph();
            p.Alignment = (ParagraphAlignment)fontStyle.ParagraphAlignment;
            XWPFRun r = p.CreateRun();
            r.FontFamily = fontStyle.FontFamily;
            r.FontSize = fontStyle.FontSize;
            r.IsBold = fontStyle.IsBold;
            r.SetText(title);
        }
        /// <summary>
        /// 追加段落
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="paragraphAlignmentEnum">段落对其</param>
        /// <returns></returns>
        public WordBuilder AppendParagraph(string value, string fontFamily, int fontSize, bool isBold, ParagraphAlignmentEnum paragraphAlignmentEnum)
        {
            XWPFParagraph p = doc.CreateParagraph();
            p.Alignment = (ParagraphAlignment)paragraphAlignmentEnum;
            XWPFRun r = p.CreateRun();
            r.FontFamily = fontFamily;
            r.FontSize = fontSize;
            r.IsBold = isBold;
            r.SetText(value);
            return this;
        }
        /// <summary>
        /// 追加段落
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="isBold">是否加粗</param>
        /// <returns></returns>
        public WordBuilder AppendParagraph(string value, string fontFamily, int fontSize, bool isBold)
        {
            return AppendParagraph(value, fontFamily, fontSize, isBold, ParagraphAlignmentEnum.LEFT);
        }
        /// <summary>
        /// 追加段落
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>
        public WordBuilder AppendParagraph(string value, string fontFamily, int fontSize)
        {
            return AppendParagraph(value, fontFamily, fontSize, false);
        }
        public WordBuilder AppendParagraph(string value, string fontFamily)
        {
            return AppendParagraph(value, fontFamily, 12);
        }
        public WordBuilder AppendParagraph(string value, int fontSize)
        {
            return AppendParagraph(value, DEFAULT_FONTFAMILY, fontSize);
        }
        public WordBuilder AppendParagraph(string value, bool isBold)
        {
            return AppendParagraph(value, DEFAULT_FONTFAMILY, DEFAULT_FONTSIZE, isBold);
        }
        /// <summary>
        /// 追加段落
        /// </summary>
        /// <param name="value">内容</param>
        /// <returns></returns>
        public WordBuilder AppendParagraph(string value)
        {
            return AppendParagraph(value, DEFAULT_FONTFAMILY, DEFAULT_FONTSIZE);
        }

        public WordBuilder AppendTable(DataTable dataTable, BorderStyle borderStyle, FontStyle titleColumnFontStyle, FontStyle contentColumnFontStyle)
        {
            string borderColorStr = ColorTranslator.ToHtml(borderStyle.BorderColor);
            XWPFTable table = doc.CreateTable(dataTable.Rows.Count + 1, dataTable.Columns.Count);
            table.SetTopBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);
            table.SetRightBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);
            table.SetBottomBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);
            table.SetLeftBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);
            table.SetInsideHBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);
            table.SetInsideVBorder((XWPFTable.XWPFBorderType)borderStyle.BorderType, borderStyle.BorderSize, 0, borderColorStr);

            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                table.SetColumnWidth(j, (ulong)dataTable.Columns[j].ColumnName.Length * 2 * 256);
                XWPFParagraph pIO = table.GetRow(0).GetCell(j).AddParagraph();
                pIO.Alignment = (ParagraphAlignment)titleColumnFontStyle.ParagraphAlignment;
                XWPFRun rIO = pIO.CreateRun();
                rIO.FontFamily = titleColumnFontStyle.FontFamily;
                rIO.FontSize = titleColumnFontStyle.FontSize;
                rIO.IsBold = titleColumnFontStyle.IsBold;
                rIO.SetText(dataTable.Columns[j].ColumnName);
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    XWPFParagraph pIO = table.GetRow(i + 1).GetCell(j).AddParagraph();
                    pIO.Alignment = (ParagraphAlignment)contentColumnFontStyle.ParagraphAlignment;
                    XWPFRun rIO = pIO.CreateRun();
                    rIO.FontFamily = contentColumnFontStyle.FontFamily;
                    rIO.FontSize = contentColumnFontStyle.FontSize;
                    rIO.IsBold = contentColumnFontStyle.IsBold;
                    rIO.SetText(dataTable.Rows[i][j].ToString());
                }
            }
            return this;
        }

        /// <summary>
        /// 返回Byes
        /// </summary>
        /// <returns></returns>
        public byte[] ToBuffer()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                doc.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        public void SaveFile(string filename)
        {
            byte[] buffer = ToBuffer();
            if (buffer == null || buffer.Length == 0)
            {
                throw new Exception("保存文件出错");
            }

            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                fs.Write(buffer, 0, buffer.Length);
            }
        }
        public void Close()
        {
            doc.Close();
        }
        /// <summary>
        /// 段落对其
        /// </summary>
        public enum ParagraphAlignmentEnum
        {
            /// <summary>
            /// 左对齐
            /// </summary>
            LEFT = 1,
            /// <summary>
            /// 居中
            /// </summary>
            CENTER = 2,
            /// <summary>
            /// 右对齐
            /// </summary>
            RIGHT = 3,
            /// <summary>
            /// 两端对其
            /// </summary>
            BOTH = 4,
            MEDIUM_KASHIDA = 5,
            /// <summary>
            /// 散布
            /// </summary>
            DISTRIBUTE = 6,
            NUM_TAB = 7,
            HIGH_KASHIDA = 8,
            LOW_KASHIDA = 9,
            THAI_DISTRIBUTE = 10
        }
        /// <summary>
        /// 边框类型
        /// </summary>
        public enum BorderTypeEnum
        {
            NIL = 0,
            NONE = 1,
            SINGLE = 2,
            THICK = 3,
            DOUBLE = 4,
            DOTTED = 5,
            DASHED = 6,
            DOT_DASH = 7
        }
    }
    public class BorderStyle
    {
        public WordBuilder.BorderTypeEnum BorderType { get; set; } = WordBuilder.BorderTypeEnum.SINGLE;
        public Color BorderColor { get; set; } = Color.Black;
        public int BorderSize { get; set; } = 1;
    }
    public class FontStyle
    {
        public string FontFamily { get; set; } = "微软雅黑";
        public int FontSize { get; set; } = 12;
        public bool IsBold { get; set; } = false;
        public WordBuilder.ParagraphAlignmentEnum ParagraphAlignment { get; set; } = WordBuilder.ParagraphAlignmentEnum.LEFT;
    }
}
