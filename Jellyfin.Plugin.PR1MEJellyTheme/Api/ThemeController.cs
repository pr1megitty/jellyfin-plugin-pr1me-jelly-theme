using System.Security.Cryptography;
using System.Text;
using Jellyfin.Plugin.PR1MEJellyTheme.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.PR1MEJellyTheme.Api;

/// <summary>
/// Serves the theme files. These are static style/script assets only: nothing here touches
/// streaming, transcoding or any media endpoint. They are public on purpose so the login page can be styled.
/// </summary>
[ApiController]
[Route("pr1mejellytheme")]
public class ThemeController : ControllerBase
{
    [HttpGet("theme.css")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public ActionResult GetCss()
    {
        string css = ThemeBuilder.BuildCss(Plugin.Instance!.Configuration);
        return Serve(css, "text/css; charset=utf-8");
    }

    [HttpGet("theme.js")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public ActionResult GetScript()
    {
        string js = ThemeBuilder.BuildScript(Plugin.Instance!.Configuration);
        return Serve(js, "application/javascript; charset=utf-8");
    }

    private ActionResult Serve(string content, string contentType)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        string etag = "\"" + Convert.ToHexString(SHA1.HashData(bytes)) + "\"";

        // Browsers revalidate every time, so a settings change shows up on the next refresh.
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["ETag"] = etag;

        if (Request.Headers["If-None-Match"].ToString() == etag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return File(bytes, contentType);
    }
}
