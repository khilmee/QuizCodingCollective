using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizCodingCollective.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Currency { get; set; }
        public double Salary { get; set; }
    }
}