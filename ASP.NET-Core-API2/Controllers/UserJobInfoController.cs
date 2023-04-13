using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Models;
using ASP.NET_Core_API2.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_API2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserJobInfoController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserJobInfoController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        // ---------- GET ALL ----------
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<UserJobInfo>> GetUserJobInfo()
        {
            string sql = @"
            SELECT 
                [UserId]
               ,[JobTitle]
               ,[Department]
            FROM 
                TutorialAppSchema.UserJobInfo";
            try
            {
                IEnumerable<UserJobInfo> users = _dapper.LoadData<UserJobInfo>(sql);
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
        public ActionResult<UserSalary> GetSingleUserJobInfo(int userId)
        {
            if (userId == 0) { return BadRequest("ID Does Not Exist"); }

            string sql = @"
            SELECT 
                [UserId]
               ,[JobTitle]
               ,[Department]
            FROM  
                TutorialAppSchema.UserJobInfo
            WHERE UserId = " + userId.ToString();

            try
            {
                UserJobInfo user = _dapper.LoadDataSingle<UserJobInfo>(sql);
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
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @"
        UPDATE TutorialAppSchema.UserJobInfo
            SET [JobTitle] = '" + userJobInfo.JobTitle +
             "', [Department] = '" + userJobInfo.Department +
            "' WHERE UserId = " + userJobInfo.UserId.ToString();

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
            return BadRequest("Failed to Update UserJobInfo");
        }

        // ---------- CREATE ----------
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            // the id must be for an actual user because its a foreign key for the main users table.

            string sql = @"INSERT INTO TutorialAppSchema.UserJobInfo(
                [UserId],
                [JobTitle],
                [Department]
            ) VALUES (" +
                    "'" + userJobInfo.UserId +
                    "', '" + userJobInfo.JobTitle +
                    "', '" + userJobInfo.Department +
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
            return BadRequest("Failed to Add UserJobInfo");
        }

        // ---------- DELETE ----------
        [HttpDelete("Delete/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
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
            return BadRequest("Failed to Delete UserJobInfo");
        }

    }
}
