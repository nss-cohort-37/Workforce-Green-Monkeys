using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentEditViewmodel
    {
        public int Id { get; set; }


        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        [MinLength(2, ErrorMessage = "Name should be at least 2 characters")]
        public string Name { get; set; }
        

        [Display(Name = "Budget")]
        [Required(ErrorMessage = "Budget is Required")]
        public int Budget { get; set; }

        public int EmployeeCount { get; set; }

    }
}