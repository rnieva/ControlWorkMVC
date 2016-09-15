using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlWorkMVC1.Models;
using GoogleMaps.LocationServices;


namespace ControlWorkMVC1.Controllers
{
    public class LocationWorksController : Controller
    {
        private Model1 db = new Model1();
        // GET: LocationWorks
        public ActionResult Index(string id)    //id is a value if the user select the zipcode in Work List
        {
            DataRegs modelWorkLocations = new DataRegs();
            List<string> zipCodes2 = new List<string>();
            List<string> infoSites2 = new List<string>();
            var listBd = from p in db.DataRegs select p;
            foreach (var p in listBd)   // TODO: sometimes I get an error, point line 53, Request Not Authorized or Over QueryLimit
            {
                    zipCodes2.Add(p.zipCode); 
                    infoSites2.Add(p.siteWork); 
            }
            modelWorkLocations.zipCodes = zipCodes2.Distinct().ToList();   //delete repeats
            modelWorkLocations.infoSites = infoSites2.Distinct().ToList();
            ViewData["listaZipcodes"] = modelWorkLocations.zipCodes;        //datos no tipados
            ViewData["listaInfoSites"] = modelWorkLocations.infoSites; List<InfoMarker> listMarkers = new List<InfoMarker> { };
            int i = 0;
            for (i = 0; i < modelWorkLocations.zipCodes.LongCount(); i++)
            {
                var locationService = new GoogleLocationService(); //Install-Package GoogleMaps.LocationServices, to get position
                var point = locationService.GetLatLongFromAddress(modelWorkLocations.zipCodes[i]);
                var googleMarker = new InfoMarker
                {
                    SiteName = modelWorkLocations.infoSites[i],
                    Latitude = point.Latitude,
                    Longitude = point.Longitude,
                    InfoWindow = modelWorkLocations.infoSites[i],
                    ZipCode = modelWorkLocations.zipCodes[i],
                };
                listMarkers.Add(googleMarker);
            }
            ViewData["id"] = id;
            return View(listMarkers);
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
