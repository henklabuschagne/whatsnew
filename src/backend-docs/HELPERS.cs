// ============================================
// What's New API - Helper Classes
// ============================================

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace WhatsNewAPI.Helpers
{
    // ============================================
    // JWT HELPER
    // ============================================
    
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string username, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetTokenExpiration()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480");
            return DateTime.UtcNow.AddMinutes(expirationMinutes);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }

    // ============================================
    // PASSWORD HELPER
    // ============================================
    
    public class PasswordHelper
    {
        private const int WorkFactor = 12; // BCrypt work factor (higher = more secure but slower)

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // At least 8 characters
            if (password.Length < 8)
            {
                return false;
            }

            // Contains uppercase
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Contains lowercase
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Contains digit
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Contains special character
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                return false;
            }

            return true;
        }

        public List<string> GetPasswordStrengthErrors(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required");
                return errors;
            }

            if (password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long");
            }

            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one digit");
            }

            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                errors.Add("Password must contain at least one special character");
            }

            return errors;
        }
    }

    // ============================================
    // VALIDATION HELPER
    // ============================================
    
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            // Validate version format (e.g., 1.0.0, 2.1.3, etc.)
            var versionPattern = @"^\d+\.\d+(\.\d+)?$";
            return System.Text.RegularExpressions.Regex.IsMatch(version, versionPattern);
        }

        public static bool IsValidChangeType(string changeType)
        {
            if (string.IsNullOrWhiteSpace(changeType))
            {
                return false;
            }

            var validTypes = new[] { "bug_fix", "new_feature", "enhancement" };
            return validTypes.Contains(changeType.ToLower());
        }

        public static bool IsValidTagValue(string tagValue)
        {
            if (string.IsNullOrWhiteSpace(tagValue))
            {
                return false;
            }

            // Tag value must be lowercase with underscores only
            var tagPattern = @"^[a-z_]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(tagValue, tagPattern);
        }

        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Remove potentially dangerous characters
            return input.Trim();
        }

        public static List<string> ValidateReleaseData(string version, DateTime releaseDate)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(version))
            {
                errors.Add("Version is required");
            }
            else if (!IsValidVersion(version))
            {
                errors.Add("Version must be in format X.Y or X.Y.Z (e.g., 1.0 or 1.0.0)");
            }

            if (releaseDate > DateTime.Now.AddYears(1))
            {
                errors.Add("Release date cannot be more than 1 year in the future");
            }

            return errors;
        }

        public static List<string> ValidateChangeData(string description, string changeType)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add("Description is required");
            }
            else if (description.Length > 5000)
            {
                errors.Add("Description cannot exceed 5000 characters");
            }

            if (string.IsNullOrWhiteSpace(changeType))
            {
                errors.Add("Change type is required");
            }
            else if (!IsValidChangeType(changeType))
            {
                errors.Add("Change type must be bug_fix, new_feature, or enhancement");
            }

            return errors;
        }
    }

    // ============================================
    // DATE HELPER
    // ============================================
    
    public static class DateHelper
    {
        public static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime? ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }

            return null;
        }

        public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        public static int GetDaysBetween(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days;
        }
    }

    // ============================================
    // STRING HELPER
    // ============================================
    
    public static class StringHelper
    {
        public static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, maxLength) + "...";
        }

        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }

        public static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = text.ToLowerInvariant();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-");
            text = text.Trim('-');

            return text;
        }

        public static List<string> SplitAndTrim(string text, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return text.Split(separator)
                      .Select(s => s.Trim())
                      .Where(s => !string.IsNullOrWhiteSpace(s))
                      .ToList();
        }
    }

    // ============================================
    // MAPPER HELPER
    // ============================================
    
    public static class MapperHelper
    {
        public static TDestination Map<TSource, TDestination>(TSource source) 
            where TDestination : new()
        {
            if (source == null)
            {
                return default;
            }

            var destination = new TDestination();
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            foreach (var sourceProp in sourceType.GetProperties())
            {
                var destProp = destinationType.GetProperty(sourceProp.Name);
                if (destProp != null && destProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);
                    destProp.SetValue(destination, value);
                }
            }

            return destination;
        }
    }
}
