using ApecMovieCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.Domain.Models;

namespace UserServices.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByNameAsync(string name);
    }
}
