using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Models;
using ASP.NET_Core_API2.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_API2.Controllers.v1
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserSalaryController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        // ---------- GET ALL ----------
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<UserSalary>> GetUserSalarys()
        {
            string sql = @"
            SELECT 
                [UserId]
               ,[Salary]
            FROM 
                TutorialAppSchema.UserSalary";
            try
            {
                IEnumerable<UserSalary> users = _dapper.LoadData<UserSalary>(sql);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- GET BY ID ----------
        [HttpGet("Get/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserSalary> GetSingleUserSalary(int userId)
        {
            if (userId == 0) { return BadRequest("ID Does Not Exist"); }

            string sql = @"
            SELECT 
                [UserId]
               ,[Salary]
            FROM  
                TutorialAppSchema.UserSalary
            WHERE UserId = " + userId.ToString();

            try
            {
                UserSalary user = _dapper.LoadDataSingle<UserSalary>(sql);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- UPDATE ----------
        [HttpPut("Edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult EditUser(UserSalary userSalary)
        {
            string sql = @"
        UPDATE TutorialAppSchema.UserSalary
            SET [Salary] = '" + userSalary.Salary +
            "' WHERE UserId = " + userSalary.UserId.ToString();

            Console.WriteLine(sql);
            try
            {
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Failed to Update UserSalary");
        }

        // ---------- CREATE ----------
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            // the id must be for an actual user because its a foreign key for the main users table.

            string sql = @"INSERT INTO TutorialAppSchema.UserSalary(
                [UserId],
                [Salary]
            ) VALUES (" +
                    "'" + userSalary.UserId +
                    "', '" + userSalary.Salary +
                "')";

            Console.WriteLine(sql);
            try
            {
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Failed to Add UserSalary");
        }

        // ---------- DELETE ----------
        [HttpDelete("Delete/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserSalary 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);
            try
            {
                if (_dapper.ExecuteSql(sql))
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Failed to Delete UserSalary");
        }

    }
}
