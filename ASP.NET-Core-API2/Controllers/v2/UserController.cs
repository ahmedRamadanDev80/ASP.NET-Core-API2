using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Models;
using ASP.NET_Core_API2.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_API2.Controllers.v2
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        // ---------- GET ----------
        [HttpGet("GetUsers/{userId}/{isActive}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<UserV2>> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameters = "";

            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }
            if (isActive)
            {
                parameters += ", @Active=" + isActive.ToString();
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }
            try
            {
                IEnumerable<UserV2> users = _dapper.LoadData<UserV2>(sql);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- UPSERT ----------
        [HttpPut("UpsertUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpsertUser(UserV2 user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
           "', @LastName = '" + user.LastName +
           "', @Email = '" + user.Email +
           "', @Gender = '" + user.Gender +
           "', @Active = '" + user.Active +
           "', @JobTitle = '" + user.JobTitle +
           "', @Department = '" + user.Department +
           "', @Salary = '" + user.Salary +
           "', @UserId = " + user.UserId;

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
            return BadRequest(" Request Failed ");
        }

        // ---------- Delete ----------
        [HttpDelete("DeleteUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"TutorialAppSchema.spUser_Delete
            @UserId = " + userId.ToString();

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
            return BadRequest(" Request Failed ");
        }

    }
}
