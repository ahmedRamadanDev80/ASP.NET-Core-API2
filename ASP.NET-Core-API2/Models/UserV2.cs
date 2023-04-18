﻿namespace ASP.NET_Core_API2.Models
{
    public class UserV2
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public bool Active { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";
        public decimal Salary { get; set; }
        public decimal AvgSalary { get; set; }
    }
}