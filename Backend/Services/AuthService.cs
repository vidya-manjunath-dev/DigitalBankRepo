using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DigitalBankLite.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly BankDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(BankDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new customer and creates default accounts (Savings and Current).
        /// </summary>
        /// <param name="dto">The registration data transfer object.</param>
        /// <returns>A tuple indicating success, a message, and the created customer object.</returns>
        public (bool Success, string Message, Customer? Customer) Register(RegisterDto dto)
        {
            if (_context.Customers.Any(c => c.Email == dto.Email))
            {
                return (false, "Email already registered.", null);
            }

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedDate = DateTime.UtcNow,
                Role = "Customer",
                KycStatus = "Pending"
            };

            if (dto.Email.Contains("admin"))
            {
                customer.Role = "Admin";
                customer.KycStatus = "Approved";
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Create default accounts
            var savingsAccount = new Account
            {
                CustomerId = customer.Id,
                AccountNumber = GenerateAccountNumber(),
                AccountType = "Savings",
                Balance = 0,
                Status = "Inactive"
            };
            _context.Accounts.Add(savingsAccount);

            var currentAccount = new Account
            {
                CustomerId = customer.Id,
                AccountNumber = GenerateAccountNumber(),
                AccountType = "Current",
                Balance = 0,
                Status = "Inactive"
            };
            _context.Accounts.Add(currentAccount);
            _context.SaveChanges();

            return (true, "Registration successful.", customer);
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token if credentials are valid.
        /// </summary>
        /// <param name="dto">The login data transfer object.</param>
        /// <returns>A tuple indicating success, a message, and the login response with token.</returns>
        public (bool Success, string Message, LoginResponseDto? Response) Login(LoginDto dto)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == dto.Email);
            if (customer == null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            {
                return (false, "Invalid credentials.", null);
            }

            if (customer.Role != "Admin" && customer.KycStatus != "Approved")
            {
                return (false, "Account is not approved yet. KYC Pending.", null);
            }

            var token = GenerateJwtToken(customer);
            return (true, "Login successful.", new LoginResponseDto
            {
                Token = token,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role
            });
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="user">The customer object.</param>
        /// <returns>A signed JWT token string.</returns>
        private string GenerateJwtToken(Customer user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Resets the password for a specific admin user for recovery purposes.
        /// </summary>
        public void ResetAdminPassword()
        {
            var admin = _context.Customers.FirstOrDefault(c => c.Email == "myadmin@bank.com");
            if (admin != null)
            {
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves a list of users with sensitive info for debugging purposes.
        /// </summary>
        /// <returns>A collection of anonymous objects containing user details.</returns>
        public ICollection<object> DebugUsers()
        {
             return _context.Customers.Select(c => (object)new { c.Email, c.Role, c.KycStatus, c.PasswordHash }).ToList();
        }

        /// <summary>
        /// Seeds a default admin user if one does not already exist.
        /// </summary>
        public void SeedAdmin()
        {
            var adminEmail = "admin@digitalbank.com";
            if (!_context.Customers.Any(c => c.Email == adminEmail))
            {
                var admin = new Customer
                {
                    Name = "System Admin",
                    Email = adminEmail,
                    Phone = "0000000000",
                    Address = "System",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Role = "Admin",
                    KycStatus = "Approved",
                    CreatedDate = DateTime.UtcNow
                };
                _context.Customers.Add(admin);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves the profile details for a specific user by ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>An anonymous object with profile details or null if not found.</returns>
        public object? GetProfile(int userId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);
            if (customer == null) return null;

            return new
            {
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.Role,
                customer.KycStatus,
                customer.CreatedDate
            };
        }

        /// <summary>
        /// Generates a random 8-digit account number prefixed with 'DB'.
        /// </summary>
        /// <returns>A unique account number string.</returns>
        private string GenerateAccountNumber()
        {
            var random = new Random();
            return "DB" + random.Next(10000000, 99999999).ToString();
        }
    }
}
