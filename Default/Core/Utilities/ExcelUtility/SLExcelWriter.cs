using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LCW.Core.Utilities.ExcelUtility
{
    public static class SLExcelWriter
    {
        private static string ColumnLetter(int intCol)
        {
            try
            {
                var intFirstLetter = ((intCol) / 676) + 64;
                var intSecondLetter = ((intCol % 676) / 26) + 64;
                var intThirdLetter = (intCol % 26) + 65;

                var firstLetter = (intFirstLetter > 64)
                    ? (char)intFirstLetter : ' ';
                var secondLetter = (intSecondLetter > 64)
                    ? (char)intSecondLetter : ' ';
                var thirdLetter = (char)intThirdLetter;

                return string.Concat(firstLetter, secondLetter,
                    thirdLetter).Trim();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static Cell CreateTextCell(string header, UInt32 index,string text)
        {
            try
            {
                var cell = new Cell
                {
                    DataType = CellValues.InlineString,
                    CellReference = header + index
                };

                var istring = new InlineString();
                var t = new Text { Text = text };
                istring.AppendChild(t);
                cell.AppendChild(istring);
                return cell;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static byte[] GenerateExcel(SLExcelData data)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                    {
                        var workbookpart = document.AddWorkbookPart();
                        workbookpart.Workbook = new Workbook();
                        var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();

                        worksheetPart.Worksheet = new Worksheet(new List<OpenXmlElement> { sheetData });

                        var sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                        var sheet = new Sheet()
                        {
                            Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = data.SheetName ?? "Sheet 1"
                        };
                        sheets.AppendChild(sheet);

                        // Add header
                        UInt32 rowIdex = 0;
                        var row = new Row { RowIndex = ++rowIdex };
                        sheetData.AppendChild(row);
                        var cellIdex = 0;

                        AddHeaders(data, row, ref cellIdex, rowIdex);
                        AddColumnConfigurations(data, worksheetPart);

                        foreach (var rowData in data.DataRows)
                        {
                            if (rowData == null || rowData.Count == 0) continue;

                            cellIdex = 0;
                            row = new Row { RowIndex = ++rowIdex };
                            sheetData.AppendChild(row);

                            foreach (var cellData in rowData)
                            {
                                string cellDataText = cellData ?? string.Empty;
                                var cell = CreateTextCell(ColumnLetter(cellIdex++), rowIdex, cellDataText);
                                row.AppendChild(cell);
                            }
                        }

                        workbookpart.Workbook.Save();
                    }

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        private static void AddHeaders(SLExcelData data, Row row, ref int cellIdex, uint rowIdex)
        {
            if (data.Headers == null || data.Headers.Count == 0) return;

            foreach (var header in data.Headers)
            {
                string textHeader = header ?? string.Empty;
                row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++), rowIdex, textHeader));
            }
        }



        private static void AddColumnConfigurations(SLExcelData data, WorksheetPart worksheetPart)
        {
            if (data.ColumnConfigurations == null) return;

            var columns = (Columns)data.ColumnConfigurations.Clone();
            if (columns != null)
            {
                worksheetPart.Worksheet.InsertAfter(columns, worksheetPart.Worksheet.SheetFormatProperties);
            }

        }
    }
}
