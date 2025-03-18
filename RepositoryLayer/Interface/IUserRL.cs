using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public  interface  IUserRL
    {

       
        bool Register(UsersEntity user);
        UsersEntity Login(string email);


        bool CheckEmail(ForgotPasswordDTO forgotPasswordDTO);
        UsersEntity GetUserByEmail(ForgotPasswordDTO forgotPasswordDTO);
        bool SaveResetToken(int userId, string token, DateTime expiry);
        UsersEntity GetUserByToken(string token);
        bool UpdatePassword(int userId, string newPassword);



    }
}
