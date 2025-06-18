using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId);
    }
}
