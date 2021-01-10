using System.Security.Claims;

namespace API.Extensions
{
    public static class ClainsPrinciplieExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        

    }
}