using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlWorkMVC1.Models;
using System.Web.Helpers;
using System.IO;
using System.Text;
using System.Web.Routing;

namespace ControlWorkMVC1.Controllers
{
    public class ViewDataController : Controller
    {
        private Model1 db = new Model1();
        public List<decimal> earnedPerTypeWork = new List<decimal>(); // to show on Chart
        public List<decimal> earnedWeek = new List<decimal>();
        public List<int> infoNumerOfWeek = new List<int>();
        public List<decimal> timeWorkedPerWeekL = new List<decimal>(); 
        public ActionResult Index()
        {
            decimal totalEarned = 0;
            decimal earnedAfterSchools = 0;
            decimal earnedBreakfastSchools = 0;
            decimal earnerCreche = 0;
            decimal earnedNursery = 0;
            decimal timeWorkedPerWeek = 0;
            decimal timeWorkedPerWeekTemp = 0;
            decimal dec = 0;
            var listBd = from p in db.DataRegs select p;
            List<DataRegs> valuesBd = new List<DataRegs>();
            int wn = 0; //week number
            int j = 0;
            int ws = 8; //start work at this week, week start
            long countRegs = listBd.LongCount() - 1; //to calculate earned per week
            decimal totalEarnedWeek = 0;
            decimal totalEarnedWeekTemp = 0;
            foreach (var p in listBd)
            {
                valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                DateTime DayWorked = DateTime.Parse(p.dateWork.ToString());
                totalEarned = p.earned + totalEarned;
                switch (p.typeWork)
                {
                    case "After School":
                        earnedAfterSchools = earnedAfterSchools + p.earned;
                        break;
                    case "Creche":
                        earnerCreche = earnerCreche + p.earned;
                        break;
                    case "Breakfast Club":
                        earnedBreakfastSchools = earnedBreakfastSchools + p.earned;
                        break;
                    case "Nursery":
                        earnedNursery = earnedNursery + p.earned;
                        break;
                }
                wn = GetWeekNumber(DayWorked);
                if ((ws == wn) && !(j == countRegs))   //this if it´s for shoew earnedWeek, store in earnedWeek list the earend per week
                {
                    totalEarnedWeek = p.earned + totalEarnedWeek;
                    dec = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours);
                    timeWorkedPerWeek = dec + timeWorkedPerWeek;
                }
                else
                {
                    if (j == countRegs)
                    {
                        totalEarnedWeek = p.earned + totalEarnedWeek;

                        dec = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours);
                        timeWorkedPerWeek = dec + timeWorkedPerWeek;

                        wn++;
                    }
                    earnedWeek.Add(totalEarnedWeek + totalEarnedWeekTemp);
                    infoNumerOfWeek.Add(wn - 1);
                    timeWorkedPerWeekL.Add(timeWorkedPerWeek + timeWorkedPerWeekTemp);
                    totalEarnedWeekTemp = p.earned; //for the last day of the week
                    timeWorkedPerWeekTemp = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours); //for the last day of the week
                    totalEarnedWeek = 0;
                    timeWorkedPerWeek = 0;
                    ws = wn;
                }
                j++;
            }
            ViewData["totalEarned"] = totalEarned;
            ViewData["earnedAfterSchools"] = earnedAfterSchools;
            ViewData["earnerCreche"] = earnerCreche;
            ViewData["earnedBreakfastSchools"] = earnedBreakfastSchools;
            ViewData["earnedNursery"] = earnedNursery;
            ViewData["infoNumerOfWeek"] = infoNumerOfWeek;
            ViewData["earnedWeek"] = earnedWeek;
            ViewData["timeWorkedPerfWeek"] = timeWorkedPerWeekL;
            ViewData["countRegs"] = countRegs +1;
            //ViewData["chart"] = GetChart();

            return View(db.DataRegs.ToList());
            
        }

        int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public ActionResult ReportChartEaernedPerWeek()
        {
            infoNumerOfWeek = getData(earnedWeek, earnedPerTypeWork);
            var chart = new Chart(width: 500, height: 400, theme: ChartTheme.Green)
            .AddTitle("Eaerned Per Week")
            //.SetXAxis("Number of Week")
            //.SetXAxis("Earned per Week")
            .AddSeries( chartType: "Column",
                            xValue: infoNumerOfWeek.ToArray(),
                            yValues: earnedWeek.ToArray())
                            .GetBytes("png");
                    
            return File(chart, "image/bytes");
        }

        public ActionResult ReportChartEaernedPerTypeWork()
        {
            infoNumerOfWeek = getData(earnedWeek, earnedPerTypeWork);
            var chart = new Chart(width: 500, height: 400, theme: ChartTheme.Green)
                .AddTitle("Eaerned Per Type Of Work")
                 .AddSeries(chartType: "Radar",
                            xValue: new[] { "After Schools", "Créche", "Breakfast", "Nursery" },
                            yValues: earnedPerTypeWork.ToArray())
                            .GetBytes("png");
            return File(chart, "image/bytes");
        }

        List<int> getData(List<decimal> earnedWeek,List<decimal> earnedPerTypeWork)
        {
            decimal totalEarned = 0;
            decimal earnedAfterSchools = 0;
            decimal earnedBreakfastSchools = 0;
            decimal earnerCreche = 0;
            decimal earnedNursery = 0;
            decimal timeWorkedPerWeek = 0;
            decimal timeWorkedPerWeekTemp = 0;
            decimal dec = 0;
            var listBd = from p in db.DataRegs select p;
            List<DataRegs> valuesBd = new List<DataRegs>();
            int wn = 0; //week number
            int j = 0;
            int ws = 8; //start work at this week, week start
            long countRegs = listBd.LongCount() - 1; //to calculate earned per week
            decimal totalEarnedWeek = 0;
            decimal totalEarnedWeekTemp = 0;
            foreach (var p in listBd)
            {
                valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                DateTime DayWorked = DateTime.Parse(p.dateWork.ToString());
                totalEarned = p.earned + totalEarned;
                switch (p.typeWork)
                {
                    case "After School":
                        earnedAfterSchools = earnedAfterSchools + p.earned;
                        break;
                    case "Creche":
                        earnerCreche = earnerCreche + p.earned;
                        break;
                    case "Breakfast Club":
                        earnedBreakfastSchools = earnedBreakfastSchools + p.earned;
                        break;
                    case "Nursery":
                        earnedNursery = earnedNursery + p.earned;
                        break;
                }
                wn = GetWeekNumber(DayWorked);
                if ((ws == wn) && !(j == countRegs))   //this if it´s for shoew earnedWeek, store in earnedWeek list the earend per week
                {
                    totalEarnedWeek = p.earned + totalEarnedWeek;

                    dec = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours);
                    timeWorkedPerWeek = dec + timeWorkedPerWeek;
                }
                else
                {
                    if (j == countRegs)
                    {
                        totalEarnedWeek = p.earned + totalEarnedWeek;

                        dec = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours);
                        timeWorkedPerWeek = dec + timeWorkedPerWeek;

                        wn++;
                    }
                    earnedWeek.Add(totalEarnedWeek + totalEarnedWeekTemp);
                    infoNumerOfWeek.Add(wn - 1);
                    timeWorkedPerWeekL.Add(timeWorkedPerWeek + timeWorkedPerWeekTemp);
                    totalEarnedWeekTemp = p.earned; //for the last day of the week
                    timeWorkedPerWeekTemp = Convert.ToDecimal(TimeSpan.Parse(p.timeWorked.ToString()).TotalHours); //for the last day of the week
                    totalEarnedWeek = 0;
                    timeWorkedPerWeek = 0;
                    ws = wn;
                }
                j++;
            }
            earnedPerTypeWork.Add(earnedAfterSchools);
            earnedPerTypeWork.Add(earnerCreche);
            earnedPerTypeWork.Add(earnedBreakfastSchools);
            earnedPerTypeWork.Add(earnedNursery);
            return infoNumerOfWeek;
        }
        
        public ActionResult Select(String typeWork, String dateStart, String dateFinish)
        {
            DateTime ndateStart = Convert.ToDateTime(dateStart.ToString());
            string ndateStart2 = ndateStart.ToShortDateString();
            DateTime ndateFinish = Convert.ToDateTime(dateFinish.ToString());
            string ndateFinish2 = ndateFinish.ToShortDateString();
            List<DataRegs> valuesBd = new List<DataRegs>();
            var listBd = from p in db.DataRegs select p;
            int countRegs = 0;
            foreach (var p in listBd)
            {
                if (p.typeWork == typeWork)
                {
                    if ((Convert.ToDateTime(p.dateWork) >= Convert.ToDateTime(dateStart)) && (Convert.ToDateTime(p.dateWork) <= Convert.ToDateTime(dateFinish)))
                    {
                        valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                        countRegs++;
                    }
                }
                if (typeWork == "Any Job")
                {
                    if ((Convert.ToDateTime(p.dateWork) >= Convert.ToDateTime(dateStart)) && (Convert.ToDateTime(p.dateWork) <= Convert.ToDateTime(dateFinish)))
                    {
                        valuesBd.Add(new DataRegs() { Id = p.Id, typeWork = p.typeWork, siteWork = p.siteWork, zipCode = p.zipCode, detailsWork = p.detailsWork, dateWork = p.dateWork, timeStartWork = p.timeStartWork, timeFinishWork = p.timeFinishWork, timeWorked = p.timeWorked, earned = p.earned, paid = p.paid });
                        countRegs++;
                    }
                }
            }
            ViewData["countRegs"] = countRegs;
            return PartialView(valuesBd);
        }
    }
}
