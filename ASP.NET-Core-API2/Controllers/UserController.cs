﻿using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_API2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        // ---------- GET ALL ----------
        [HttpGet("GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return Ok(users);
        }

        // ---------- GET BY ID ----------
        [HttpGet("GetSingleUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> GetSingleUser(int userId)
        {
            if (userId == 0) { return BadRequest("ID Does Not Exist"); }

            string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString();

            try
            {
                User user = _dapper.LoadDataSingle<User>(sql);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- UPDATE ----------
        [HttpPut("EditUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult EditUser(User user)
        {
            string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId;

            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            return BadRequest("Failed to Update User");
        }

    }
}
