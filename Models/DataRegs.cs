namespace ControlWorkMVC1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;


    public partial class DataRegs
    {

        public List<SelectListItem> WorkTypesList = new List<SelectListItem>
         {
             new SelectListItem { Text = "After School Club", Value = "After School" },
             new SelectListItem { Text = "Nursery", Value = "Nursery" },
             new SelectListItem { Text = "Créche", Value = "Creche" },
             new SelectListItem { Text = "Babysitting", Value = "Babysitting" },
             new SelectListItem { Text = "Nanny", Value = "Nanny" },
             new SelectListItem { Text = "Breakfast Club", Value = "Breakfast Club" },
        };

        public List<string> zipCodes = new List<string>();
        public List<string> infoSites = new List<string>();

        public int Id { get; set; }

        [Display(Name = "Work Type")]
        [Required(ErrorMessage = "The Type Work is required")]
        public string typeWork { get; set; }

        [Required]
        [Display(Name = "Work Site")]
        public string siteWork { get; set; }

        [Display(Name = "Work Details")]
        public string detailsWork { get; set; }

        [Display(Name = "Work Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string dateWork { get; set; }

        [Display(Name = "Starting Time")]
        [Required]
        public string timeStartWork { get; set; }

        [Required]
        [Display(Name = "Finishing Time")]
        public string timeFinishWork { get; set; }

        [Display(Name = "Time Worked")]
        public string timeWorked { get; set; }

        [Required] //add regular expression for . instead ,
        [Display(Name = "Earned")]
        public decimal earned { get; set; }

        [Display(Name = "Paid")]
        public bool paid { get; set; }

        [Required]
        [Display(Name = "ZipCode")]
        public string zipCode { get; set; }
    }
}
