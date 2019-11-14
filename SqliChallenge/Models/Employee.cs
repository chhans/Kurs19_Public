using System;

namespace SqliChallenge.Models
{

    public class Employee
    {
        public Employee(string first, string last, string email) {
            this.firstName = first;
            this.lastName = last;
            this.email = email;
        }

        public int employeeNumber { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string extension { get; set; }
        public string email { get; set; }
        public int officeCode { get; set; }
        public int reportsTo { get; set; }
        public String jobTitle { get; set; }
    }
}