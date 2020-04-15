using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is Required Dum! Dum!")]
        [MinLength(3, ErrorMessage = "Last Name should have at least 3 characters Einstein!")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is Required Dum! Dum!")]
        [MinLength(3, ErrorMessage = "Last Name should have at least 3 characters Einstein!")]
        public string LastName { get; set; }

        [Display(Name= "Supervisor" )]
        [Required]
        public Boolean IsSupervisor { get; set; }

        [Display(Name = "Department Name")]
        //[Required(ErrorMessage = "Department Name is Required Dum! Dum!")]
        //[MinLength(3, ErrorMessage = "Department Name should have at least 3 characters Einstein!")]
        public string Name { get; set; }

        public List<SelectListItem> DepartmentOptions { get; set; }

    }
}
// Adam talks about StudentEditViewiModel in NSS_MVC_INTRO_P11 around 6 min point