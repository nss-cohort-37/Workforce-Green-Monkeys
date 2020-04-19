using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeTrainingAssignViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Training Programs")]
        public int TrainingProgramId { get; set; }
        public int EmployeeCount { get; set; }



        public List<SelectListItem> ProgramOptions { get; set; }

    }
}
