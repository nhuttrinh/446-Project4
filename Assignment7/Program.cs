using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Assignment7
{
    class Course
    {
        public string CourseId { get; set; }
        public string Subject { get; set; }
        public Int32 CourseCode { get; set; }
        public string Title { get; set; }
        public string Days { get; set; }
        public string Location { get; set; }
        public string Classroom { get; set; }
        public string Instructor { get; set; }
    }

    class Program
    {
        protected static List<Course> Courses = new List<Course>();

       /* protected static void AddCourse()
        {
            using (StreamWriter textWriter = File.AppendText("Courses.csv"))
            {
                textWriter.WriteLine()
            }
        }*/
        protected static void CreateListCourse()
        {
            int entriesFound = 0;

            using (var textReader = new StreamReader("Courses.csv"))
            {
                string line = textReader.ReadLine();
                int skipCount = 0;

                while (line != null && skipCount < 1)
                {
                    line = textReader.ReadLine();
                    skipCount++;
                }

                while (line != null)
                {
                    string[] columns = line.Split(",");

                    Course course = new Course
                    {
                        CourseId = columns[2],
                        Subject = columns[0],
                        CourseCode = Convert.ToInt32(columns[1]),
                        Title = columns[3],
                        Days = columns[4],
                        Location = columns[7],
                        Classroom = columns[9],
                        Instructor = columns[10]
                    };

                    Courses.Add(course);
                    entriesFound++;
                    line = textReader.ReadLine();
                }
            }
        }

        protected static void ListOfIEE()
        {
            IEnumerable<Course> myQuery =
                from course in Courses
                where course.CourseCode >= 300 && course.Subject.Equals("IEE")
                orderby course.Instructor
                select course;
            foreach(Course item in myQuery)
            {
                Console.WriteLine("Title = \"{0}\", Instructor = {1}", item.Title, item.Instructor);
            }
        }

        protected static void GroupAndSubGroup()
        {
            var courseGroups =
                from course in Courses
                group course by course.Subject into g
                select new { Subject = g.Key, Class = g,
                    subCourses =
                        from c in g
                        group c by c.CourseCode into g2
                        select new { Code = g2.Key, C = g2 }  
                };
                foreach (var g in courseGroups)
            {
                if (g.subCourses.Count() >= 2)
                {
                    Console.WriteLine("Subject {0} contains these following Course Codes: ", g.Subject);
                    foreach (var g2 in g.subCourses)
                    {
                        Console.WriteLine(g2.Code);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            CreateListCourse();
            ListOfIEE();
            GroupAndSubGroup();
            //Console.WriteLine(Courses[200].CourseId);
        }
    }
}
