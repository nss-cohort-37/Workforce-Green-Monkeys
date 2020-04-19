using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        public string Email { get; set; }

        [Display(Name = "Supervisor")]
        public bool IsSupervisor { get; set; }
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public Department Department { get; set; }

        public int ComputerId { get; set; }

        [Display(Name = "Computer")]
        public Computer? Computer { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }


        [Display(Name = "Training")]
        public TrainingProgram TrainingProgram { get; set; }

        [Display(Name = "Department Name")]
        public string Name { get; set; }




    }
}
