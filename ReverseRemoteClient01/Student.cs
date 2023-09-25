using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseRemoteClient01
{
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Phone { get; set; }

        public Student(int id, string name, DateTime dob, string phone)
        {
            ID = id;
            Name = name;
            DOB = dob;
            Phone = phone;
        }

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, DOB: {DOB.ToShortDateString()}, Phone: {Phone}";
        }
    }
}
