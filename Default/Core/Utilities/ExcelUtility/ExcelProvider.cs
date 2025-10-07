using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LCW.Core.Entities.Concrete;

namespace LCW.Core.Utilities.ExcelUtility
{
    public class ExcelProvider
    {
        protected delegate T FillerToList<out T>(Row dr, WorkbookPart w);

        protected static List<K> ExecuteReaderToList<K>(string sheetName, SpreadsheetDocument document, FillerToList<K> filler)
        {
            List<K> ret = new List<K>();

            try
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                var sheets = workbookPart.Workbook.Descendants<Sheet>();

                var sheet = sheets.FirstOrDefault(s => s.Name == sheetName);
                if (sheet == null)
                {
                    throw new ArgumentException($"Sheet with name '{sheetName}' not found.");
                }

                var workSheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
                var sheetData = workSheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null)
                {
                    throw new InvalidOperationException("SheetData not found in the worksheet.");
                }

                var rows = sheetData.Elements<Row>().Skip(1).ToList(); // Skip header row

                foreach (var row in rows)
                {
                    ret.Add(filler(row, workbookPart));
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error in ExecuteReaderToList", ex);
            }
        }

    }

}
