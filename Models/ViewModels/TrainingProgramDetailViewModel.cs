﻿using BangazonWorkforce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.ViewModels
{
    public class TrainingProgramDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Max Attendees")]
        public int MaxAttendees { get; set; }
        [Display(Name = "Employees Attending")]
        public List<Employee> Employees { get; set; }
        public int EmployeeCount { get
            {
               return Employees.Count;
            }
        }

    }
}
