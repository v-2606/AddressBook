using System;
using BussinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System.Security.Claims;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RabbitMQLayer.Interface;

namespace BussinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
       private readonly IEventPublisher _eventPublisher;

        public UserBL(IUserRL userRL, IMapper mapper, IConfiguration configuration, IEmailService emailService, IEventPublisher eventPublisher)
        {
            _userRL = userRL;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _eventPublisher = eventPublisher;
        }


        public bool Register(RegisterDTO userDTO)
        {
          
            var existingUser = _userRL.Login(userDTO.Email);
            if (existingUser != null)
            {
                return false; 
            }

          
            var userEntity = _mapper.Map<UsersEntity>(userDTO);
            userEntity.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            bool isRegistered = _userRL.Register(userEntity);
            if (isRegistered)
            {
              
                _eventPublisher.PublishEvent("user.registered", new { userDTO.Email, userEntity.UserId });
            }

            return isRegistered;
        }


        public string Login(LoginDTO loginDTO)
        {
            var user = _userRL.Login(loginDTO.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))

            {
                return GenerateJwtToken(user.Email, user.UserId);
            }
            return null;
        }

        private string GenerateJwtToken(string email, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Email, email),
                new Claim("UserId", userId.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            if (_userRL.CheckEmail(forgotPasswordDTO))
            {
                var user = _userRL.GetUserByEmail(forgotPasswordDTO);

               
                string token = Guid.NewGuid().ToString();
                DateTime expiryTime = DateTime.Now.AddMinutes(3);

                
                _userRL.SaveResetToken(user.UserId, token, expiryTime);

                string resetLink = $"https://V.com/resetpassword?token={token}";
                string subject = "Password Reset Request";
                string body = token;
                bool emailSent = _emailService.SendEmail(user.Email, subject, body);
                if (emailSent)
                {
                  
                    _eventPublisher.PublishEvent("user.forgotpassword", new { user.Email, Token = token });
                }

                return emailSent;
            }
            return false;
        }

        public bool ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = _userRL.GetUserByToken(resetPasswordDTO.Token);
            if (user != null && resetPasswordDTO.NewPassword == resetPasswordDTO.ConfirmPassword)
            {
                string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDTO.NewPassword);
                bool isUpdated = _userRL.UpdatePassword(user.UserId, newPasswordHash);

                if (isUpdated)
                {
                    
                    _eventPublisher.PublishEvent("user.passwordreset", new { user.Email });
                }

                return isUpdated;
            }
            return false;
        }

    }
}
