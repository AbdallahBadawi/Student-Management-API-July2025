using System.Collections.Generic;
using StudentAPIBusinessLayer;
using Microsoft.AspNetCore.Http;
//using StudentApi.Models;
//using StudentApi.DataSimulation;
using Microsoft.AspNetCore.Mvc;
using StudentDataAccessLayer;

namespace StudentApi.Controllers
{
    [Route("api/StudentAPI")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            //// StudentDataSimulation.StudentsList.Clear();
            //if (StudentDataSimulation.StudentsList.Count == 0)
            //{
            //    return NotFound("No students found."); // Returns a 404 Not Found if there are no students.
            //}

            //return Ok(StudentDataSimulation.StudentsList); // Returns the list of students.

            List<StudentDTO> StudentsList = StudentAPIBusinessLayer.Student.GetAllStudents();
            if (StudentsList.Count == 0)
            {
                return NotFound("No students found.");
            }

            return Ok(StudentsList); // Returns the list of students.
        }

        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            //var passedStudents = StudentDataSimulation.StudentsList.Where(student => student.Grade >= 50).ToList();
            //passedStudents.Clear();

            List<StudentDTO> passedStudents = StudentAPIBusinessLayer.Student.GetPassedStudents();
            if (passedStudents.Count == 0)
            {
                return NotFound("No Students Passed");
            }

            return Ok(passedStudents); // Returns the list of students who passed.
        }


        [HttpGet("AverageGrade", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrade()
        {
            //var averageGrade = StudentDataSimulation.StudentsList.Average(student => student.Grade);
            double averageGrade = StudentAPIBusinessLayer.Student.GetAverageGrade();
            return Ok(averageGrade);
        }


        [HttpGet("{id}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}

            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.Find(id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            StudentDTO SDTO = student.SDTO;
            return Ok(SDTO);
        }


        //For add new we use Http Post
        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudentDTO)
        {
            //we validate the data here
            if (newStudentDTO == null || string.IsNullOrEmpty(newStudentDTO.Name) || newStudentDTO.Age < 0 || newStudentDTO.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            StudentAPIBusinessLayer.Student student = new StudentAPIBusinessLayer.Student(new StudentDTO(newStudentDTO.Id, newStudentDTO.Name, newStudentDTO.Age, newStudentDTO.Grade));
            student.Save();

            newStudentDTO.Id = student.ID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.Id }, newStudentDTO);
        }


        //here we use Httpput method for update
        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudentDTO)
        {
            if (id < 1 || updatedStudentDTO == null || string.IsNullOrEmpty(updatedStudentDTO.Name) || updatedStudentDTO.Age < 0 || updatedStudentDTO.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.Find(id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            student.Name = updatedStudentDTO.Name;
            student.Age = updatedStudentDTO.Age;
            student.Grade = updatedStudentDTO.Grade;
            if (student.Save())
            {
                //we return the DTO not the full student object.
                return Ok(student.SDTO);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the student." });
            }
        }


        //For Delete we use Http Delete
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (StudentAPIBusinessLayer.Student.DeleteStudent(id))
                return Ok($"Student with ID {id} has been deleted.");
            else
                return NotFound($"Student with ID {id} not found. no rows deleted!");
        }


        //******************************************EXTRA ENDPOINTS******************************************//

        // This endpoint is for uploading an image file to the server.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            // Check if no file is uploaded
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Directory where files will be uploaded
            var uploadDirectory = @"C:\MyUploads";

            // Generate a unique filename GUID and combine with the upload directory
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Ensure the uploads directory exists, create if it doesn't
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return the file path as a response
            return Ok(new { filePath });
        }


        // Endpoint to retrieve image from the server
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetImage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            // Directory where files are stored
            var uploadDirectory = @"C:\MyUploads";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
                return NotFound("Image not found.");

            // Open the image file for reading
            var image = System.IO.File.OpenRead(filePath);
            var mimeType = GetMimeType(filePath);

            // Return the file with the correct MIME type
            return File(image, mimeType);
        }

        // Helper method to get the MIME type based on file extension
        /*
         This code defines a  method called GetMimeType that takes a file path as a parameter 
         and returns the corresponding MIME type as a string. 
         MIME types are used to indicate the nature and format of a file, 
         especially in web contexts where you need to specify the type of content you're sending, 
         like images, text, etc.

        MIME type stands for Multipurpose Internet Mail Extensions type. 
        It's a standard way to indicate the nature and format of a file or content. 
        MIME types are used to tell browsers, email clients, and 
        other software about the type of data they're handling, so they can process it correctly.
         */
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
        //*****************************************************************************************************//

    }
}
