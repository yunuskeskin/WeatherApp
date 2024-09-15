using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.Models;

namespace Weather.Core.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
