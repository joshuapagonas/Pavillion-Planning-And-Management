using BCrypt.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PhoneNumbers;
using PPM.Interfaces;
using PPM.Models;
using PPM.Models.DTOs;
using PPM.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PPM.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public async Task<UserDTO> GetUserDetailsAsync(int user_id)
        {
            var user = await _userRepository.GetUserByIdAsync(user_id);
            return new UserDTO
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                username = user.username,
                email = user.email,
                is_admin = user.is_admin,
            };
        }
        public async Task<UserDTO> RegisterUserAsync(RegisterUserDTO userDTO)
        {
            var existing_user = await _userRepository.GetUserByUsernameAsync(userDTO.username);
            if (existing_user != null)
            {
                throw new ApplicationException("Username already exists.");
            }

            bool valid_email = IsValidEmail(userDTO.email);
            if (!valid_email)
            {
                throw new ApplicationException("Email is invalid.");
            }

            string normalized_phone_number = NormalizeAndValidatePhoneNumber(userDTO.phone_number);
            if (string.IsNullOrEmpty(userDTO.password))
            {
                throw new ApplicationException("Password is invalid.");
            }
            string hashed_password = BCrypt.Net.BCrypt.HashPassword(userDTO.password);
            bool is_valid_password = BCrypt.Net.BCrypt.Verify(userDTO.password, hashed_password);
            if (!is_valid_password)
            {
                throw new ApplicationException("Password is invalid");
            }

            var user = new User
            {
                first_name = userDTO.first_name,
                last_name = userDTO.last_name,
                username = userDTO.username,
                password_hash = hashed_password,
                email = userDTO.email,
                phone_number = normalized_phone_number,
                is_admin = false,
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            return new UserDTO
            {
                user_id = createdUser.user_id,
                first_name = createdUser.first_name,
                last_name = createdUser.last_name,
                username = createdUser.username,
                email = createdUser.email,
                phone_number = createdUser.phone_number,
                is_admin = createdUser.is_admin
            };
        }
        public async Task<AuthResponseDTO?> GetUserByLoginAsync(LoginUserDTO loginuserDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginuserDTO.username);
            if (user == null)
            {
                throw new KeyNotFoundException($"Username Empty.");
            }
            string stored_password_hash = user.password_hash; //Retrieves the stored_hash within the user logging in
            bool hashed_properly = BCrypt.Net.BCrypt.Verify(loginuserDTO.password, stored_password_hash); //Compares with the hash and salt in the database
            if (!hashed_properly)
            {
                throw new ApplicationException("Invalid Password Entered.");
            }
            var userDTO = new UserDTO
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                username = user.username,
                is_admin = user.is_admin
            };
            var token = GenerateJwtToken(userDTO);
            return new AuthResponseDTO
            {
                token = token,
                User = userDTO,
            };
        }
        public async Task<UserDTO> UpdateUserDetailsAsync(int user_id, UpdateUserDTO userDTO)
        {
            var user = await _userRepository.GetUserByIdAsync(user_id);

            user.first_name = userDTO.first_name;
            user.last_name = userDTO.last_name;
            user.username = userDTO.username;

            await _userRepository.UpdateUserAsync(user);

            return new UserDTO
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                username = user.username
            };
        }
        public async Task<UserDTO> UpdateUserAdminDetailsAsync(int user_id, UpdateUserAdminDTO userAdminDTO)
        {
            var user = await _userRepository.GetUserByIdAsync(user_id);

            user.first_name = userAdminDTO.first_name;
            user.last_name = userAdminDTO.last_name;
            user.username = userAdminDTO.username;
            user.is_admin = userAdminDTO.is_admin;

            await _userRepository.UpdateUserAdminAsync(user);

            return new UserDTO
            {
                user_id = user.user_id,
                first_name = user.first_name,
                last_name = user.last_name,
                username = user.username,
                is_admin = user.is_admin
            };
        }
        public async Task DeleteUserRecordById(int user_id)
        {
            var user = await _userRepository.GetUserByIdAsync(user_id);
            await _userRepository.DeleteUserByIdAsync(user_id);
        }
        public int? GetLoggedInUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                _logger.LogWarning("No Authenticated User was found");
                return null;
            }

            var user_id_claim = user.FindFirst(ClaimTypes.NameIdentifier);
            /* user.FindFirst(ClaimTypes.NameIdentifier) ?? 
            * user.FindFirst("user_id");*/
            if (user_id_claim == null)
            {
                _logger.LogWarning("User ID Claim not found.");
                return null;
            }
            foreach (var claim in user.Claims)
            {
                _logger.LogInformation("Claim Type: {Type}, Value: {Value}", claim.Type, claim.Value);
            }

            bool is_parsed = int.TryParse(user_id_claim.Value, out var user_id_parsed);
            if (is_parsed)
            {
                return user_id_parsed;
            }
            _logger.LogWarning("User ID claim value is not a valid integer: {Value}", user_id_claim.Value);
            return null;
        }
        /*
        //Make this private in the future and create a new function named ChangePasswordAsync();
        public async Task SetPasswordAsync(int user_id, string password)
        {
            var user = await _userRepository.GetUserByIdAsync(user_id);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            string hashed_password = BCrypt.Net.BCrypt.HashPassword(password); //Hashes and Salts the password
            user.password_hash = hashed_password; // Stores the hashed_password in the database.
            await _userRepository.UpdateUserAsync(user);
        }
        */
        private string GenerateJwtToken(UserDTO userDTO)
        {
            _logger.LogInformation("userDTO: ID={UserId}, Username={Username}, Admin={IsAdmin}", userDTO.user_id, userDTO.username, userDTO.is_admin);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDTO.user_id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userDTO.username),
                new Claim("is_admin", userDTO.is_admin.ToString().ToLowerInvariant()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique Token ID
            };
            foreach (var claim in claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            /*
            byte[] keyBytes = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes); //Call GetBytes() on the instance
            }
            */
            var secret = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new ApplicationException("JWT secret key is missing from configuration.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Creates token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            var handler = new JwtSecurityTokenHandler();
            string jwt = handler.WriteToken(token);
            _logger.LogInformation("Generated JWT: {Token}", jwt);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch { return false; }
        }
        private string NormalizeAndValidatePhoneNumber(string phone_number)
        {
            try
            {
                var phone_util = PhoneNumberUtil.GetInstance();
                var number = phone_util.Parse(phone_number, "US");
                bool valid_phone_number = phone_util.IsValidNumber(number);
                if (!valid_phone_number)
                {
                    throw new ApplicationException("Phone Number is invalid.");
                }
                string phone_number_formatted = phone_util.Format(number, PhoneNumberFormat.NATIONAL);
                return phone_number_formatted;
            }
            catch { throw new ApplicationException("Phone Number is invalid."); }
        }
    }
}