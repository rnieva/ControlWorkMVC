using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlWorkMVC1.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace ControlWorkMVC1.Controllers
{
    public class DataRegsController : Controller
    {
        private Model1 db = new Model1();

        // GET: DataRegs
        public ActionResult Index()
        {
            List<int> indexColoursRows = new List<int>(); //out because it uses in two methods to paint colours
            int wn = 0; //week number
            decimal totalEarned = 0;
            var listBd = from p in db.DataRegs select p;
            List<DataRegs> valuesBd = new List<DataRegs>();
            long countRegs = listBd.LongCount() - 1;
            foreach (var p in listBd)
            {
                valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                DateTime DayWorked = DateTime.Parse(p.dateWork.ToString());
                totalEarned = p.earned + totalEarned;
                wn = GetWeekNumber(DayWorked);
                indexColoursRows.Add(wn); //to paint weeks, but I use other way in the view, I can remove this element
            }
            ViewData["totalEarned"] = totalEarned;
            ViewData["indexColoursRows"] = indexColoursRows;
            ViewData["countRegs"] = countRegs+1;   // TODO: sort the works by date
            return View(db.DataRegs.ToList());
        }

        int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        // GET: DataRegs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataRegs dataRegs = db.DataRegs.Find(id);
            if (dataRegs == null)
            {
                return HttpNotFound();
            }
            return View(dataRegs);
        }

        // GET: DataRegs/Create
        public ActionResult Create()
        {
            DataRegs modelWorkTypesList = new DataRegs();
            
            return View(modelWorkTypesList);
        }

        // POST: DataRegs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public TimeSpan timeWorked;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,typeWork,siteWork,detailsWork,dateWork,timeStartWork,timeFinishWork,timeWorked,earned,paid,zipCode")] DataRegs dataRegs)
        {
            if (ModelState.IsValid)
            {
                DateTime dateWork = Convert.ToDateTime(dataRegs.dateWork.ToString());
                dataRegs.dateWork = dateWork.ToShortDateString();

                db.DataRegs.Add(dataRegs);
                //db.Entry(dataRegs).State = EntityState.Modified; // If execute this there will be error
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dataRegs);
        }

        // GET: DataRegs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataRegs dataRegs = db.DataRegs.Find(id);
            var date = DateTime.Parse(dataRegs.dateWork);
            dataRegs.dateWork = date.ToString("yyyy-MM-dd"); //para mostrar la fecha en el formulario

            if (dataRegs == null)
            {
                return HttpNotFound();
            }
            return View(dataRegs);
        }

        // POST: DataRegs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,typeWork,siteWork,detailsWork,dateWork,timeStartWork,timeFinishWork,timeWorked,earned,paid,zipCode")] DataRegs dataRegs)
        {
            if (ModelState.IsValid)
            {
                DateTime dateWork = Convert.ToDateTime(dataRegs.dateWork.ToString());
                dataRegs.dateWork = dateWork.ToShortDateString();
                DataRegs dataRegsTest = new DataRegs();
                dataRegsTest = dataRegs;
                db.Entry(dataRegs).State = EntityState.Modified; // I have to this for update the DB
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataRegs);
        }

        // GET: DataRegs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataRegs dataRegs = db.DataRegs.Find(id);
            if (dataRegs == null)
            {
                return HttpNotFound();
            }
            return View(dataRegs);
        }

        // POST: DataRegs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataRegs dataRegs = db.DataRegs.Find(id);
            db.DataRegs.Remove(dataRegs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult exportDataToExcelFile() //export to Excel File
            //improvement: add titles, change format
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 1;
            int z = 1;
            List<DataRegs> valuesBd = new List<DataRegs>();
            var listBd = from p in db.DataRegs select p;
            long countRegs = listBd.LongCount();
            foreach (var p in listBd)
            {
                valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                DateTime DayWorked = DateTime.Parse(p.dateWork.ToString());
            }
            for (i = 1; i < countRegs; i++)
                {
                    xlWorkSheet.Cells[j, z] = valuesBd[i].typeWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].siteWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].zipCode.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].detailsWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].dateWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].timeStartWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].timeFinishWork.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].timeWorked.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].earned.ToString();
                    xlWorkSheet.Cells[j, z++] = valuesBd[i].paid.ToString();
                    j++;
                    z = 1;
                }
            xlWorkBook.SaveAs("timeWorked.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            //xlApp.Visible = true;
            Debug.WriteLine("Excel file created, timeWorked.xls");

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
