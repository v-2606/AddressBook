using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {

        private readonly UserContext _context;

        public UserRL(UserContext context)
        {
            _context = context;
        }

        public bool Register(UsersEntity userEntity)
        {
            try
            {
                _context.Users.Add(userEntity);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB Update Error: {ex.InnerException?.Message}");
            }
            return false;
        }

        public UsersEntity Login(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }


        public bool CheckEmail(ForgotPasswordDTO forgotPasswordDTO)
        {
            return _context.Users.Any(u => u.Email == forgotPasswordDTO.email);
        }

        public UsersEntity GetUserByEmail(ForgotPasswordDTO forgotPasswordDTO)
        {
            return _context.Users.FirstOrDefault(u => u.Email == forgotPasswordDTO.email);
        }

        public bool SaveResetToken(int userId, string token, DateTime expiry)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.ResetToken = token;
                user.ResetTokenExpiry = expiry;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public UsersEntity GetUserByToken(string token)
        {
            return _context.Users.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.Now);
        }

        public bool UpdatePassword(int userId, string newPasswordHash)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.Password = newPasswordHash;
                user.ResetToken = null; 
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}