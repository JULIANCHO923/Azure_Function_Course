using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace CourseDBFunction
{
    public static class AddCourse
    {
        [FunctionName("AddCourse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Course course = JsonConvert.DeserializeObject<Course>(requestBody);
           
            string _connection_string = Environment.GetEnvironmentVariable("SQLConnectionString_SQLAZURECONNSTR");
            string _statement = "INSERT INTO Course (CourseID,CourseName,Rating) VALUES(@param1,@param2,@param3);";
            // We first establish a connection to the database
            SqlConnection _connection = new SqlConnection(_connection_string);
            _connection.Open();

            SqlCommand _sqlcommand = new SqlCommand(_statement, _connection);

            int result = 0;
            using (SqlCommand _command = new SqlCommand(_statement,_connection))
            {
                _command.Parameters.Add("@param1", SqlDbType.Int).Value = course.CourseID;
                _command.Parameters.Add("@param2", SqlDbType.VarChar,25).Value = course.CourseName;
                _command.Parameters.Add("@param3", SqlDbType.Decimal).Value = course.Rating;
                _command.CommandType = CommandType.Text;
                result = _command.ExecuteNonQuery();
            }

            if (result == 1)
            {
                return new OkObjectResult("Curso adicionado exitosamente");
            }
            else
            {
                return new BadRequestObjectResult("Error adicionando el curso");
            }
        }
    }
}
