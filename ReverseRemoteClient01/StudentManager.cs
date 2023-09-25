using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReverseRemoteClient01
{
    public class StudentManager
    {
        string filePath;
        public List<Student> students = new List<Student>();

        public StudentManager(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    students = JsonSerializer.Deserialize<List<Student>>(json);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public List<Student> GetStudents() {
            return this.students;
        }


    }
}
