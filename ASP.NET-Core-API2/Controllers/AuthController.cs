using ASP.NET_Core_API2.Data;
using ASP.NET_Core_API2.Helpers;
using ASP.NET_Core_API2.Models;
using ASP.NET_Core_API2.Models.Dtos;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Cryptography;


namespace ASP.NET_Core_API2.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        // ---------- Register ----------
        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register(UserToRegisterDto userToRegister)
        {
            // check if user entered the same pass and pass confirm. 
            if (userToRegister.Password == userToRegister.PasswordConfirm)
            {
                // check if user already exists in Db.
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userToRegister.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    // making an object with email and pass to pass to setpassword in authhelper that encrypt pass and add it to Auth table.
                    UserToLoginDto userForSetPassword = new UserToLoginDto()
                    {
                        Email = userToRegister.Email,
                        Password = userToRegister.Password
                    };
                    try
                    {
                        //creating user in auth table
                        if (_authHelper.SetPassword(userForSetPassword))
                        {
                            string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userToRegister.FirstName +
                            "', @LastName = '" + userToRegister.LastName +
                            "', @Email = '" + userToRegister.Email +
                            "', @Gender = '" + userToRegister.Gender +
                            "', @Active = 1" +
                            ", @JobTitle = '" + userToRegister.JobTitle +
                            "', @Department = '" + userToRegister.Department +
                            "', @Salary = '" + userToRegister.Salary + "'";
                            //create user in other tables using a stored proc
                            if (_dapper.ExecuteSql(sqlAddUser))
                            {
                                return Ok();
                            }
                            return BadRequest("Failed to add user.");
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Failed to register user.");
                    }
                }
                return BadRequest("User with this email already exists!");
            }
            return BadRequest("Passwords do not match!");
        }

        // ---------- Login ----------
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login(UserToLoginDto userToLogin)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
                @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@EmailParam", userToLogin.Email, DbType.String);
            // getting the hased password and salt from Db
            try
            {
                UserToConfirmLoginDto userToConfirmLogin = _dapper
                    .LoadDataSingleWithParameters<UserToConfirmLoginDto>(sqlForHashAndSalt, sqlParameters);

                // hashing the password the user entered with the salt from the Db 
                byte[] passwordHash = _authHelper.GetPasswordHash(userToLogin.Password, userToConfirmLogin.PasswordSalt);
                //comparing the hashed pass from dp with the hash generated from the string pass the user entered in the login form
                for (int index = 0; index < passwordHash.Length; index++)
                {
                    if (passwordHash[index] != userToConfirmLogin.PasswordHash[index])
                    {
                        return StatusCode(401, "Incorrect password!");
                    }
                }
                // if user entered the correct email and pass give him a token that has his id in it 
                string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                 userToLogin.Email + "'";

                int userId = _dapper.LoadDataSingle<int>(userIdSql);

                return Ok(new Dictionary<string, string> {
                    {"token", _authHelper.CreateToken(userId)} });
            }
            catch (Exception ex)
            {
                return BadRequest("email is invalid!");
            }
        }

        // ---------- ResetPassword ----------
        [HttpPut("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ResetPassword(UserToLoginDto userForSetPassword)
        {
            try
            {
                if (_authHelper.SetPassword(userForSetPassword))
                {
                    return Ok();
                }
            }catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Failed to update password!");
        }

        // ---------- RefreshToken ----------
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }
    }
}
