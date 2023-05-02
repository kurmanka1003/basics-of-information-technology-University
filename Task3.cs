using System;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WindowsFormsApp 
{
  public partial class Form1: Form 
  {
    private string connectionString;
    private string selectedFilePath;

    public Form1() 
    {
      InitializeComponent();
    }

    private void SelectFileButton_Click(object sender, EventArgs e) 
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "OpenOffice Database (*.odb)|*.odb";
      openFileDialog.Title = "Выберите файл";

      if (openFileDialog.ShowDialog() == DialogResult.OK) 
      {
        selectedFilePath = openFileDialog.FileName;
        connectionString = $ "Driver={{OpenOffice.org 3.3}};Database={selectedFilePath};";
        LoadData();
      }
    }

    private void SaveFileButton_Click(object sender, EventArgs e) 
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
      saveFileDialog.Title = "Сохранение";

      if (saveFileDialog.ShowDialog() == DialogResult.OK) 
      {
        string saveFilePath = saveFileDialog.FileName;
        ExportToExcel(saveFilePath);
      }
    }

    private void LoadData() 
    {
      using(OdbcConnection connection = new OdbcConnection(connectionString)) 
      {
        string query = "SELECT * FROM \"Table1\"";
        OdbcDataAdapter adapter = new OdbcDataAdapter(query, connection);
        DataTable dataTable = new DataTable();

        try 
        {
          connection.Open();
          adapter.Fill(dataTable);
          dataGridView1.DataSource = dataTable;
        } 
        catch (Exception ex) 
        {
          MessageBox.Show(ex.Message);
        }
      }
    }

    private void ExportToExcel(string filePath) 
    {
      using(SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook)) 
      {
        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        WorksheetPart worksheetPart = workbookPart.AddNewPart < WorksheetPart > ();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        Sheet sheet = new Sheet() {
          Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet 1"
        };
        sheets.Append(sheet);

        SheetData sheetData = worksheetPart.Worksheet.GetFirstChild < SheetData > ();

        for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++) 
        {
          Row row = new Row();
          for (int columnIndex = 0; columnIndex < dataGridView1.Columns.Count; columnIndex++) 
          {
            object value = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value;

            Cell cell = new Cell();
            cell.DataType = GetCellDataType(value);
            cell.CellValue = new CellValue(value.ToString());
            row.Append(cell);
          }

          sheetData.Append(row);
        }

        worksheetPart.Worksheet.Save();
        workbookPart.Workbook.Save();
      }

      MessageBox.Show("Таблица успешно сохранена");
    }

    private static CellValues GetCellDataType(object value) 
    {
      if (value == null) 
      {
        return CellValues.String;
      } 
      else if (value is string) 
      {
        return CellValues.String;
      } 
      else if (value is int || value is long || value is short || value is byte || value is sbyte)
      {
        return CellValues.Number;
      }
      else if (value is decimal || value is float || value is double) 
      {
        return CellValues.Number;
      } 
      else if (value is DateTime) 
      {
        return CellValues.Date;
      } 
      else 
      {
        return CellValues.String;
      }
    }
  }
}