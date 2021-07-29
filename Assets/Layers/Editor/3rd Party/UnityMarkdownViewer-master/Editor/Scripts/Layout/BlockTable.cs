using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdig.Extensions.Tables;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Editor.Scripts.Layout
{
    public class BlockTable : Block
    {
        List<List<CellContainer>> tableRows = new List<List<CellContainer>>();
        StringBuilder mWord = new StringBuilder();

        int margin = 5;

        public BlockTable(float indent, Table table, Context context) : base(indent)
        {
            Style style = Style.Default;

            Style boldStyle = Style.Default;
            boldStyle.textAlignment = TextAnchor.UpperCenter;
            boldStyle.Bold = true;

            foreach(TableRow row in table.Descendants<TableRow>())
            {

                Style rowStyle = row.IsHeader ? boldStyle : style;
                if (row == null)
                    continue;
                List<CellContainer> tableRow = new List<CellContainer>();
                foreach (TableCell cell in row.Descendants<TableCell>()){
                    if (cell == null)
                        continue;

                    CellContainer blockContent = new CellContainer(0f);
                    blockContent.isInHeader = row.IsHeader;

                    ParagraphBlock paragraphBlock = cell.Descendants<ParagraphBlock>().First();
                    foreach (Inline inline in paragraphBlock.Inline)
                    {
                        if (inline is LiteralInline)
                        {
                            StringSlice stringSlice = (inline as LiteralInline).Content;
                            string text = stringSlice.Text.Substring(stringSlice.Start, stringSlice.Length);

                            for (var i = 0; i < text.Length; i++)
                            {
                                var ch = text[i];

                                if (ch == '\n')
                                {
                                    AddWord(blockContent, context, rowStyle);
                                    //NewLine();
                                }
                                else if (char.IsWhiteSpace(ch))
                                {
                                    mWord.Append(' ');
                                    AddWord(blockContent, context, rowStyle);
                                }
                                else
                                {
                                    mWord.Append(ch);
                                }
                            }

                            AddWord(blockContent, context, rowStyle);


                        }
                    }
                    tableRow.Add(blockContent);

                }
                tableRows.Add(tableRow);
            }
        }

        void AddWord(CellContainer blockContent, Context context, Style style)
        {
            if (mWord.Length == 0)
            {
                return;
            }

            var payload = new GUIContent(mWord.ToString(), "");
            var content = new ContentText(payload, style, "");
            content.CalcSize(context);
            blockContent.Add(content);
            blockContent.singleLineWidth += content.Location.width;
            mWord.Length = 0;
        }

        public override void Arrange(Context context, Vector2 anchor, float maxWidth)
        {

            //Calculating widths
            int numberOfColumns = CalculateNumberColumns();
            float columnWidth = maxWidth / numberOfColumns;
            float[] widths = new float[numberOfColumns];

            foreach (List<CellContainer> row in tableRows)
            {
                for (int index = 0; index < row.Count; index++)
                {
                    CellContainer cell = row[index];

                    Vector2 position = new Vector2(0,0);
                    if (widths[index] < cell.singleLineWidth + margin*2f)
                        widths[index] = cell.singleLineWidth + margin * 2f;
                }
            }

            float widthOfUnshrunkColumns = maxWidth;
            float numberOfUnshrunkColumns = 0;
            foreach(float width in widths)
            {
                if (width < columnWidth)
                    widthOfUnshrunkColumns -= width;
                else
                    numberOfUnshrunkColumns++;
            }

            float finalColumnWidth = numberOfUnshrunkColumns > 0 ? widthOfUnshrunkColumns / numberOfUnshrunkColumns : columnWidth;

            for (int index = 0; index < widths.Length; index++)
            {
                if (widths[index] > columnWidth)
                {
                    widths[index] = finalColumnWidth;
                }
            }


            //Calculating Heights
            float currentHeight = anchor.y;
            foreach(List<CellContainer> row in tableRows)
            {
                float maxHeight = EditorGUIUtility.singleLineHeight;
                float lastXPosition = 0f;
                for (int index = 0; index < row.Count; index++)
                {
                    CellContainer cell = row[index];

                    Vector2 position = new Vector2(lastXPosition + anchor.x + margin, currentHeight + margin);

                    lastXPosition += widths[index];

                    cell.Arrange(context, position, widths[index] - 2f* margin);
                    if (maxHeight < cell.Rect.height)
                        maxHeight = cell.Rect.height;
                }

                // setting heights
                for (int index = 0; index < row.Count; index++)
                {
                    CellContainer cell = row[index];
                    cell.Rect.height = maxHeight;
                }
                currentHeight += maxHeight + 2f* margin;
            }

            Rect = new Rect(anchor.x, anchor.y, maxWidth, currentHeight- anchor.y);
        }

        private int CalculateNumberColumns()
        {
            int columnCount = 0;
            foreach (List<CellContainer> row in tableRows)
            {
                if (row.Count > columnCount)
                    columnCount = row.Count;
            }
            return columnCount;
        }

        public override void Draw(Context context)
        {
            //foreach (List<CellContainer> row in tableRows)
            bool headerExists = false;
            for (int index = 0; index < tableRows.Count; index++)
            {
                List<CellContainer> row = tableRows[index];
                foreach(CellContainer cell in row)
                {
                    if (cell.isInHeader)
                        headerExists = true;

                    Rect drawRect = new Rect(cell.Rect.x - margin, cell.Rect.y - margin, cell.Rect.width + 2f*margin, cell.Rect.height + 2f * margin);
                    EditorGUI.DrawRect(drawRect, new Color32(223, 226, 229, 255));
                    Rect innerRect = new Rect(drawRect.x + 1, drawRect.y + 1, drawRect.width - 2, drawRect.height - 2);

                    if ((headerExists? index +1 : index) % 2 == 1 && !cell.isInHeader)
                        EditorGUI.DrawRect(innerRect, new Color32(246, 248, 250, 255));
                    else
                        EditorGUI.DrawRect(innerRect, new Color32(255, 255, 255, 255));

                    cell.Draw(context);

                }
            }
        }
    }
}