using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;
using System.Configuration;

namespace LCW.Core.Utilities.ExcelUtility
{
    public class SLExcelReader : ExcelProvider
    {

        private static string GetColumnName(string cellReference)
        {
            var regex = new Regex("[A-Za-z]+", RegexOptions.None, TimeSpan.FromSeconds(5));
            var match = regex.Match(cellReference);

            return match.Value;
        }

        private static int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$", RegexOptions.None, TimeSpan.FromSeconds(5));
            if (!alpha.IsMatch(columnName))
                throw new ArgumentException($"Invalid column name: {columnName}. Column name must contain only uppercase letters.");

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                // ASCII 'A' = 65
                int current = i == 0 ? letter - 65 : letter - 64;
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private static IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            int currentCount = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell()
                    {
                        DataType = null,
                        CellValue = new CellValue(string.Empty)
                    };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }

        private static string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            return (text ?? string.Empty).Trim();
        }

        public static SLExcelData ReadExcel(MemoryStream stream)
        {
            var data = new SLExcelData();
            // Open the excel document
            WorkbookPart workbookPart; List<Row> rows;
            try
            {
                var document = SpreadsheetDocument.Open(stream, false);
                workbookPart = document.WorkbookPart;

                var sheets = workbookPart.Workbook.Descendants<Sheet>();
                var sheet = sheets.First();
                data.SheetName = sheet.Name;

                var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;
                var columns = workSheet.Descendants<Columns>().FirstOrDefault();
                data.ColumnConfigurations = columns;

                var sheetData = workSheet.Elements<SheetData>().First();
                rows = sheetData.Elements<Row>().ToList();
            }
            catch (Exception)
            {
                data.Status.Message = "Unable to open the file";
                return data;
            }

            // Read the header
            if (rows.Count > 0)
            {
                var row = rows[0];
                var cellEnumerator = GetExcelCellEnumerator(row);
                while (cellEnumerator.MoveNext())
                {
                    var cell = cellEnumerator.Current;
                    var text = ReadExcelCell(cell, workbookPart).Trim();
                    data.Headers.Add(text);
                }
            }

            // Read the sheet data
            if (rows.Count > 1)
            {
                for (var i = 1; i < rows.Count; i++)
                {
                    var dataRow = new List<string>();
                    data.DataRows.Add(dataRow);
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    while (cellEnumerator.MoveNext())
                    {
                        var cell = cellEnumerator.Current;
                        var text = ReadExcelCell(cell, workbookPart).Trim();
                        dataRow.Add(text);
                    }
                }
            }

            return data;
        }

        public static List<SLExcelData> CustomReadExcel(IOrganizationService crmService, MemoryStream stream)
        {
            List<SLExcelData> returnList = new List<SLExcelData>();

            SpreadsheetDocument document;
            try
            {
                #region Local Parameters
                List<ExcelColumn> excelColumns = new List<ExcelColumn>();

                List<string> excelColumnsString = new List<string>();

                #endregion

                #region Open the excel document
                WorkbookPart workbookPart; List<Row> rows;

                document = SpreadsheetDocument.Open(stream, false);
                workbookPart = document.WorkbookPart;

                var sheets = workbookPart.Workbook.Descendants<Sheet>();

                #endregion

                foreach (var sheet in sheets)
                {
                    var excelData = new SLExcelData();

                    var workSheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
                    var columns = workSheet.Descendants<Columns>().FirstOrDefault();
                    excelData.ColumnConfigurations = columns;

                    var sheetData = workSheet.Elements<SheetData>().First();
                    rows = sheetData.Elements<Row>().ToList();

                    ReadHeader(rows, workbookPart, excelData, excelColumnsString, excelColumns);
                    ReadSheetData(rows, workbookPart, excelData, excelColumns);

                    returnList.Add(excelData);
                }
            }
            catch (OpenXmlPackageException e)
            {
                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    // Handle the specific case of an invalid hyperlink
                    throw new ArgumentException("The document contains an invalid hyperlink.", e);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"An error occurred in CustomReadExcel: {ex.Message}", ex);
            }


            return returnList;
        }

        private static void ReadHeader(List<Row> rows, WorkbookPart workbookPart, SLExcelData excelData, List<string> excelColumnsString, List<ExcelColumn> excelColumns)
        {
            if (rows.Count == 0) return;

            var row = rows[0];

            foreach (Cell cell in row.Descendants<Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);
                var text = ReadExcelCell(cell, workbookPart).Trim();
                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                if (excelColumnsString.Contains(text))
                {
                    excelData.Headers.Add(text);
                }

                var excelColumn = new ExcelColumn
                {
                    Name = columnName,
                    Text = text,
                    Index = currentColumnIndex
                };

                excelColumns.Add(excelColumn);
            }
        }

        private static void ReadSheetData(List<Row> rows, WorkbookPart workbookPart, SLExcelData excelData, List<ExcelColumn> excelColumns)
        {
            if (rows.Count <= 1) return;

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var dataRow = new List<string>();

                foreach (Cell cell in row.Descendants<Cell>())
                {
                    string columnName = GetColumnName(cell.CellReference);
                    var text = ReadExcelCell(cell, workbookPart).Trim();

                    if (excelColumns.Exists(a => a.Name.Equals(columnName)))
                    {
                        dataRow.Add(text);
                    }
                }

                if (dataRow.Count > 0 && !string.IsNullOrEmpty(dataRow[0]))
                {
                    excelData.DataRows.Add(dataRow);
                }
            }
        }
    }
}
