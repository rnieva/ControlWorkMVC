using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlWorkMVC1.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace ControlWorkMVC1.Controllers
{
    public class ImportFileController : Controller
    {
        private Model1 db = new Model1();

        // GET: ImportFile
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Upload(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    importButton_Click(fileName);
                }
            }
            //return View("Index");
            return RedirectToAction("Index", "DataRegs"); //go to Index view in Dataregs controller for show the work list
        }

        public void importButton_Click(string fileName)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;
            string str;
            int rCnt = 0;
            int cCnt = 0;
            int contAdd = 0;
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            range = xlWorkSheet.UsedRange;
            var dbContext = new Model1();
            for (rCnt = 2; rCnt <= range.Rows.Count; rCnt++) //start on sencond row
            {
                var newDataFromExcel = new DataRegs();
                for (cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                {
                    if ((range.Cells[rCnt, cCnt] as Excel.Range).Value2 == null) //when there is null cells on excel
                    {
                        str = "";
                    }
                    else
                    {
                        str = (range.Cells[rCnt, cCnt] as Excel.Range).Value2.ToString();
                    }
                    if (newDataFromExcel.typeWork != "") //execute only if exists a data in the first column
                    {
                        switch (cCnt)
                        {
                            case 1:
                                newDataFromExcel.typeWork = str;
                                break;
                            case 2:
                                double oleDateTime = double.Parse(str);
                                DateTime dt = DateTime.FromOADate(oleDateTime);
                                dt = DateTime.FromOADate(oleDateTime);
                                string date = dt.ToString("dd/MM/yyyy");
                                newDataFromExcel.dateWork = date;
                                break;
                            case 3:
                                double oleDateTime2 = double.Parse(str);
                                DateTime dt2 = DateTime.FromOADate(oleDateTime2);
                                string time = dt2.ToString("HH:mm");
                                newDataFromExcel.timeStartWork = time;
                                break;
                            case 4:
                                double oleDateTime3 = double.Parse(str);
                                DateTime dt3 = DateTime.FromOADate(oleDateTime3);
                                string time2 = dt3.ToString("HH:mm");
                                newDataFromExcel.timeFinishWork = time2;
                                break;
                            case 5:
                                double oleDateTime4 = double.Parse(str);
                                DateTime dt4 = DateTime.FromOADate(oleDateTime4);
                                string time3 = dt4.ToString("HH:mm");
                                newDataFromExcel.timeWorked = time3;
                                break;
                            case 6:
                                newDataFromExcel.siteWork = str;
                                break;
                            case 7:
                                newDataFromExcel.zipCode = str;
                                break;
                            case 8:
                                newDataFromExcel.detailsWork = str;
                                break;
                            case 9:
                                // nothing, it´s coef hours  
                                break;
                            case 10:
                                // nothing, it´s earn per hour 
                                break;
                            case 11:
                                newDataFromExcel.earned = decimal.Parse(str);
                                break;
                            case 14:
                                newDataFromExcel.paid = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("First Column empty");  //to stop at final of Excel File
                    }
                }
                if (checkIfRepeat(newDataFromExcel) == true) //clean last rows excel file
                {
                    dbContext.DataRegs.Add(newDataFromExcel);
                    dbContext.SaveChanges();
                    contAdd++;
                }
                else
                {
                    Debug.WriteLine("Any new Data in Excel");
                }
            }
            if (contAdd != 0)
                Debug.WriteLine(contAdd + " Data Imported");
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Debug.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        bool checkIfRepeat(DataRegs newDataFromExcel) //to check this
        {
            string dateWorkForCheck = newDataFromExcel.dateWork;
            string typeWorkForCheck = newDataFromExcel.typeWork;
            var dbContext = new Model1();
            var elementDataReg = dbContext.DataRegs.Where(DataRegs => DataRegs.dateWork == dateWorkForCheck && DataRegs.typeWork == typeWorkForCheck);
            if (elementDataReg.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult HelpImportFile()
        {
            return PartialView();
        }
    }
}
