using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Models;
using ASP.NET_Core_API2.Models.Dtos;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (isActive)
            {
                stringParameters += ", @Active=@ActiveParameter";
                sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }
            try
            {
                IEnumerable<UserV2> users = _dapper.LoadDataWithParameters<UserV2>(sql, sqlParameters);
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
            @FirstName = @FirstNameParameter, 
            @LastName = @LastNameParameter, 
            @Email = @EmailParameter, 
            @Gender = @GenderParameter, 
            @Active = @ActiveParameter, 
            @JobTitle = @JobTitleParameter, 
            @Department = @DepartmentParameter, 
            @Salary = @SalaryParameter, 
            @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
            sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
            sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);
            try
            {
                if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
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
            @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);


            try
            {
                if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
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
