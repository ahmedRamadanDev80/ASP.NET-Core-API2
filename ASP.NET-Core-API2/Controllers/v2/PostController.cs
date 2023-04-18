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
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        // ---------- GET ----------
        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Post>> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            if (postId != 0)
            {
                parameters += ", @PostId=" + postId.ToString();
            }
            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue='" + searchParam + "'";
            }

            if (parameters.Length > 0)
            {
                sql += parameters.Substring(1);
            }

            try
            {
                IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- GET MY POSTS ----------
        [HttpGet("GetMyPosts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Post>> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = " +
                this.User.FindFirst("userId")?.Value;
            try
            {
                IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ---------- UPSERT POSTS ----------
        [HttpPut("UpsertPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId =" + this.User.FindFirst("userId")?.Value +
                ", @PostTitle ='" + postToUpsert.PostTitle +
                "', @PostContent ='" + postToUpsert.PostContent + "'";

            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId = " + postToUpsert.PostId;
            }

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
            return BadRequest("Failed to upsert post!");
        }

        // ---------- DELETE ----------
        [HttpDelete("Delete/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = " +
                   postId.ToString() +
                   ", @UserId = " + this.User.FindFirst("userId")?.Value;

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
            return BadRequest("Failed to Delete post!");
        }

    }
}
