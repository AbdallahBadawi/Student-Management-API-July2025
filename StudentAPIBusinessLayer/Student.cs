using StudentDataAccessLayer;

namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

        public StudentDTO SDTO
        {
            // This property returns a new instance of StudentDTO with the current properties.
            // This insures that the DTO is always up-to-date with the current state of the Student object.
            get { return (new StudentDTO(ID, Name, Age, Grade)); }
        }
        public Student(StudentDTO SDTO, enMode cMode = enMode.AddNew)
        {
            // Default constructor of Student class
            this.ID = SDTO.Id;
            this.Name = SDTO.Name;
            this.Age = SDTO.Age;
            this.Grade = SDTO.Grade;
            this.Mode = cMode;
        }


        public static List<StudentDTO> GetAllStudents()
        {
            return StudentData.GetAllStudents();
        }
        public static List<StudentDTO> GetPassedStudents()
        {
            return StudentData.GetPassedStudents();
        }
        public static double GetAverageGrade()
        {
            return StudentData.GetAverageGrade();
        }
        public static Student Find(int ID)
        {
            StudentDTO SDTO = StudentData.GetStudentById(ID);

            if (SDTO != null)
            {
                //we return new object of that student with the right data
                return new Student(SDTO, enMode.Update);
            }
            else
                return null;
        }

        private bool _AddNewStudent()
        {
            // Call DataAccessLayer
            this.ID = StudentData.AddStudent(this.SDTO);
            return (this.ID != -1);
        }
        private bool _UpdateStudent()
        {
            return StudentData.UpdateStudent(SDTO);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewStudent())
                        {
                            Mode = enMode.Update;
                            return true; // Successfully added new student
                        }
                        else
                        {
                            return false; // Failed to add new student
                        }
                    }
                case enMode.Update:
                    return _UpdateStudent();
            }

            return false;
        }
        public static bool DeleteStudent(int ID)
        {
            return StudentData.DeleteStudent(ID);
        }
    }
}
