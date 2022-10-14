using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagment.Services.Models.Eums
{
    public enum Status
    {
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500,
        Success = 200,
        Unauthorized = 401,
    }
}
