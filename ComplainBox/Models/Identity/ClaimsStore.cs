using System.Collections.Generic;
using System.Security.Claims;

namespace ComplainBox.Models.Identity
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Dean Office", "Dean Office"),
            new Claim("Hall","Hall"),
            new Claim("Department","Department"),
        };
    }
}
