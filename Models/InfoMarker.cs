using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlWorkMVC1.Models
{
    public class InfoMarker
    {
        public string SiteName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string InfoWindow { get; set; }
        public string ZipCode { get; set; }
    }
}