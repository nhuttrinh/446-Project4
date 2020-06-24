using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assignment8
{

    /*Define an Instructor Class*/
    class Instructor
    {
        public string InstructorName { get; set; }
        public string OfficeNumber { get; set; }
        public string EmailAddress { get; set; }
    }

    class Program
    {
        protected static List<Instructor> Instructors = new List<Instructor>();//List of Instructor objects.

        /*2.1 AND 2.2 Create the Courses.xml file and save it in App_Data*/
        protected static void CreateXMLCourses()
        {
            string[] source = File.ReadAllLines("Courses.csv"); //Read entire file
            XElement xmlDocument = new XElement("Courses", //Create the root Element.
                from str in source          //separate each line in the source file
                let columns = str.Split(',') //by comma
                select new XElement("Course", //Create elemnt Course for each class
                            new XElement("CourseId", columns[2]), // Create element CourseId
                            new XElement("Subject", columns[0]),// Create element Subject
                            new XElement("CourseCode", columns[1]),// Create element CourseCode
                            new XElement("Title", columns[3]),// Create element Title
                            new XElement("Days", columns[4]),// Create element Days
                            new XElement("Location", columns[7]),// Create element Location
                            new XElement("Classroom", columns[9]),// Create element Classroom
                            new XElement("Instructor", columns[10])));// Create element Instructor
            xmlDocument.Save("Courses.xml");//Save the XML file as Courses.xml in App_Data
        }

        //Create Instructor list from the CSV file
        public static void CreateInstructorList()
        {
            int entriesFound = 0;//Declare variable for reading each line.
            using (var textReader = new StreamReader("Instructors.csv"))//Reading Course.csv file using StreamReader.
            {
                string line = textReader.ReadLine();//Read each line as string.
                int skipCount = 0;//Variable to skip the headline of the file.

                while (line != null && skipCount < 1)//Skip the header of the file.
                {
                    line = textReader.ReadLine();//Read the next line.
                    skipCount++;//Increament the skip variable.
                }

                while (line != null)//Keep reading the file till the end of file.
                {
                    string[] columns = line.Split(',');//Using comma as delimiter to split every element of a line.

                    Instructor instructor = new Instructor //Create a Instructor object.
                    {
                        InstructorName = columns[0],//Store Instructor Name to object.
                        OfficeNumber = columns[1], //Store Office Number to object.                     
                        EmailAddress = columns[2], //Store Email Address to object. 
                    };

                    Instructors.Add(instructor); //Add object to the list of Instructors.
                    entriesFound++;//Increament the line varible.
                    line = textReader.ReadLine();//Read the next line.
                }
            }
        }

        /*2.3a Retrive the list of CPI*/
        protected static void ListOfCPI()
        {
            XElement myCourses = XElement.Load("Courses.xml");
            IEnumerable<XElement> course =
                from c in myCourses.Elements("Course")
                where (string)c.Element("Subject") == "CPI" && (int)c.Element("CourseCode") >= 200
                orderby (string)c.Element("Instructor")
                select c;
            foreach (XElement c in course)
            {
                Console.WriteLine("Title: {0}, Instructor: {1}", c.Element("Title").Value, c.Element("Instructor").Value);
            }
        }

        /*23.b Create Group and Sub-Group by Subject and CourseCode*/
        protected static void GroupAndSubGroup()
        {
            XElement myCourses = XElement.Load("Courses.xml");//Load xml file from App_Data folder
            var courseGroups =
                from course in myCourses.Elements("Course") // from each Element in xml 
                group course by (string)course.Element("Subject") into g //Put same Subject elements in g
                select new
                {
                    Subject = g.Key,
                    subCourses =
                        from c in g //from each Element group in parent group.
                        group c by (string)c.Element("CourseCode") into g2 //Group each course by its course code.
                        select new { Code = g2.Key, C = g2 }  //Select the Element
                };
            foreach (var g in courseGroups)
            {
                if (g.subCourses.Count() >= 2)//Proceed if the group has two or more courses.
                {
                    Console.WriteLine("Subject {0} contains these following Course Codes: ", g.Subject);//Write the result to console.
                    foreach (var g2 in g.subCourses)
                    {
                        Console.WriteLine(g2.Code);//Write the result to console.
                    }
                }
            }
        }

        /*2.4 Deliver Subject CourseCode and Insructor Email*/
        protected static void InstructorEmailByCourse()
        {
            XElement myCourses = XElement.Load("Courses.xml");//Load XML file in App_data
            IEnumerable<XElement> query =
                 from instructor in Instructors //From each object in Instructor list.
                 join course in myCourses.Elements("Course") on instructor.InstructorName.ToLower() equals (string)(course.Element("Instructor").Value).ToLower()//Join the XML and list in which the instructor name in Crourses.xml and instructors list matched.
                 where Convert.ToInt32(course.Element("CourseCode").Value) >= 200 && Convert.ToInt32(course.Element("CourseCode").Value) < 300 //Filter the result with 200 level courses
                 orderby (string)course.Element("CourseCode") //Sort the result by Course Code in ascending order.
                 select new XElement("Course", //New Course Element
                        new XElement("CourseCode", (string)course.Element("CourseCode")), //New CourseCode Element
                        new XElement("Subject", (string)course.Element("Subject")), //New Subject Element
                        new XElement("Email", instructor.EmailAddress));//New Email Element
            foreach (XElement IEmail in query)
            {
                Console.WriteLine("Subject CourseCode: {0}{1}, Instructor's Email: {2}", IEmail.Element("Subject").Value, IEmail.Element("CourseCode").Value, IEmail.Element("Email").Value);//Write the result to console.
            }
        }

        static void Main(string[] args)
        {
            /*2.1 AND 2.2 Create the Courses.xml file and save it in App_Data*/
            Console.WriteLine("/*2.1 AND 2.2 Create the Courses.xml file and save it in App_Data*/");
            CreateXMLCourses();

            /*2.3a Retrive the list of CPI*/
            Console.WriteLine("\n/*2.3a Retrive the list of CPI*/");
            ListOfCPI();

            /*2.3b Create Group and Sub-Group by Subject and CourseCode*/
            Console.WriteLine("\n/*2.3b Create Group and Sub-Group by Subject and CourseCode*/");
            GroupAndSubGroup();

            CreateInstructorList();//Create Instructor list from the CSV file

            /*2.4 Deliver Subject CourseCode and Insructor Email*/
            Console.WriteLine("\n/*2.4 Deliver Subject CourseCode and Insructor Email*/");
            InstructorEmailByCourse();
            Console.Read();
        }
    }
}
