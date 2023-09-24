using System.Security.Claims;


namespace SustiVestShared
{

    public class CommonUtilities
    {
        public static int GetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }

            // Handle the case where the claim is not found or the value cannot be parsed as an integer.
            // You might want to return a default value or throw an exception here.
            return -1; // Example: Returning -1 if the claim is not found or cannot be parsed.
        }
    }
}

