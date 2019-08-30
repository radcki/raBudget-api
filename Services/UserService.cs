using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Contexts;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Enum;

namespace WebApi.Services
{
    public class UserService
    {
        private readonly AppSettings _appSettings;
        private readonly DataContext DatabaseContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger Logger;


        public UserService(DataContext context, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory)
        {
            DatabaseContext = context;
            _appSettings = appSettings.Value;
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger("UserService");
        }

        public BaseResult<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new BaseResult<User>(){Result = eResultType.Error};
            }

            var user = DatabaseContext.Users.FirstOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
            {
                Logger.Log(LogLevel.Warning, "Username " + username + " authentication: requested user not found");
                return new BaseResult<User>() { Result = eResultType.NotFound };
            }

            // check if password is correct
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                Logger.Log(LogLevel.Information, "User " + user.UserId + " authentication: bad password");
                return new BaseResult<User>() { Result = eResultType.Unauthorized };
            }
            Logger.Log(LogLevel.Information, "User " + user.UserId + " authentication: success");
            // authentication successful
            return new BaseResult<User>()
                   {
                       Result = eResultType.Success,
                       Data = user
            };
        }

        public string GenerateAccessToken(ClaimsPrincipal principal)
        {
            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = DatabaseContext.Users.Single(x => x.UserId == userId);
            return GenerateAccessToken(user);
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

            var claims = new[]
                         {
                             new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                             new Claim(ClaimTypes.Name, user.Username)
                         };
            foreach (var roleEntity in user.UserRoles)
                claims.Append(new Claim(ClaimTypes.Role, roleEntity.Role.ToString()));

            var jwt = new JwtSecurityToken("rabt.pl",
                                           "Everyone",
                                           claims,
                                           DateTime.UtcNow,
                                           DateTime.UtcNow.AddMinutes(15),
                                           new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                                          );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public string GenerateRefreshToken(ClaimsPrincipal principal, string clientId)
        {
            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = DatabaseContext.Users.Single(x => x.UserId == userId);

            return GenerateRefreshToken(user, clientId);
        }

        public string GenerateRefreshToken(User user, string clientId)
        {
            var token = new RefreshToken();
            var oldTokens = DatabaseContext.RefreshTokens.Where(x => x.ClientId == clientId);
            var newClient = true;
            if (oldTokens.Any())
            {
                newClient = false;
                token = oldTokens.FirstOrDefault();
            }

            token.User = user;
            token.ValidTo = DateTime.UtcNow.AddMonths(1);
            token.ClientId = clientId;

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                token.Token = Convert.ToBase64String(randomNumber);
            }

            if (newClient)
            {
                DatabaseContext.RefreshTokens.Add(token);
            }

            DatabaseContext.SaveChanges();

            return token.Token;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
                                            {
                                                ValidateAudience = false,
                                                ValidateIssuer = false,
                                                ValidateIssuerSigningKey = true,
                                                ClockSkew = TimeSpan.Zero,
                                                IssuerSigningKey =
                                                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings
                                                                                                       .Secret)),
                                                ValidateLifetime = false // pomini�cie wa�no�ci
                                            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("invalid_token");

            return principal;
        }


        public bool ValidateRefreshToken(string token, string refreshToken, string clientId)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = DatabaseContext.Users.Single(x => x.UserId == userId);
            var savedRefreshToken = user.RefreshTokens.Where(x => x.ClientId == clientId);

            var refreshTokens = savedRefreshToken.ToList();
            if (!refreshTokens.Any()
                || refreshTokens.First().Token != refreshToken
                || refreshTokens.First().ValidTo < DateTime.UtcNow)
            {
                Logger.Log(LogLevel.Information, "User " + user.UserId + " refresh token check: failure");
                return false;
            }
            Logger.Log(LogLevel.Information, "User " + user.UserId + " refresh token check: success");
            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return DatabaseContext.Users;
        }

        public User GetById(int id)
        {
            return DatabaseContext.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password)) throw new Exception("account.passwordRequired");

            if (DatabaseContext.Users.Any(x => x.Username == user.Username))
                throw new Exception("account.usernameTaken");

            if (DatabaseContext.Users.Any(x => x.Email == user.Email)) throw new Exception("account.emailTaken");

            if (!IsValidEmail(user.Email)) throw new Exception("account.emailInvalid");

            var ascciiRegex = new Regex(@"xxx[!-~]+xxx");
            var badNames = "about access account accounts add address adm admin administration adult " +
                           "advertising affiliate affiliates ajax analytics android anon anonymous api " +
                           "app apps archive atom auth authentication avatar " +
                           "backup banner banners bin billing blog blogs board bot bots business " +
                           "chat cache cadastro calendar campaign careers cgi client cliente code comercial " +
                           "compare config connect contact contest create code compras css " +
                           "dashboard data db design delete demo design designer dev devel dir directory" +
                           "doc docs domain download downloads edit editor email ecommerce " +
                           "forum forums faq favorite feed feedback flog follow file files free ftp" +
                           "gadget gadgets games guest group groups " +
                           "help home homepage host hosting hostname html http httpd https hpg " +
                           "info information image img images imap index invite intranet indice ipad iphone irc " +
                           "java javascript job jobs js " +
                           "knowledgebase " +
                           "log login logs logout list lists " +
                           "mail mail1 mail2 mail3 mail4 mail5 mailer mailing mx manager marketing master me media message " +
                           "microblog microblogs mine mp3 msg msn mysql messenger mob mobile movie movies music musicas my " +
                           "name named net network new news newsletter nick nickname notes noticias ns ns1 ns2 ns3 ns4 " +
                           "old online operator order orders " +
                           "page pager pages panel password perl pic pics photo photos photoalbum php plugin plugins pop pop3 post " +
                           "postmaster postfix posts profile project projects promo pub public python " +
                           "random register registration root ruby rss " +
                           "sale sales sample samples script scripts secure send service shop sql signup signin search security " +
                           "settings setting setup site sites sitemap smtp soporte ssh stage staging start subscribe subdomain " +
                           "suporte support stat static stats status store stores system " +
                           "tablet tablets tech telnet test test1 test2 test3 teste tests theme themes tmp todo task tasks tools tv talk " +
                           "update upload url user username usuario usage " +
                           "vendas video videos visitor " +
                           "win ww www www1 www2 www3 www4 www5 www6 www7 wwww wws wwws web webmail website websites webmaster workshop " +
                           "xxx xpg you yourname yourusername yoursite yourdomain";

            if (ascciiRegex.Match(user.Username).Success
                || IsValidEmail(user.Username)
                || badNames.Split(" ").Contains(user.Username)
                || user.Username.Contains(" ")
                || user.Username.Any(x => char.IsWhiteSpace(x)))
                throw new Exception("account.usernameInvalid");
            
            user.Password = HashPassword(password);
            user.CreationTime = DateTime.Now;

            DatabaseContext.Users.Add(user);
            DatabaseContext.SaveChanges();

            var userId = user.UserId;

            DatabaseContext.UserRoles.Add(new UserRole
                                          {
                                              UserId = userId,
                                              Role = eRole.User
                                          });

            if (DatabaseContext.Users.Count() == 1)
                DatabaseContext.UserRoles.Add(new UserRole
                                              {
                                                  UserId = userId,
                                                  Role = eRole.Admin
                                              });
            DatabaseContext.SaveChanges();
            EmailVerifyRequest(user);
            return user;
        }

        public BaseResult VerifyEmail(User user, string verificationCode)
        {
            var userEntity = DatabaseContext.Users.FirstOrDefault(x => x.Username == user.Username);
            if (userEntity == null)
            {
                return new BaseResult(){Result = eResultType.NotFound};
            }
            if (userEntity.EmailVerificationCode == verificationCode)
            {
                userEntity.EmailVerified = true;
                userEntity.EmailVerificationCode = null;
                DatabaseContext.SaveChanges();
                return new BaseResult(){Result = eResultType.Success};
            }
            else
            {
                return new BaseResult(){Result = eResultType.Error};
            }
        }

        public BaseResult EmailVerifyRequest(User user)
        {
            var userEntity = DatabaseContext.Users.FirstOrDefault(x => x.UserId == user.UserId);
            if (userEntity == null)
            {
                return new BaseResult(){Result = eResultType.NotFound};
            }
            var emailConfirmString = ExtensionMethods.RandomString(6);
            userEntity.EmailVerificationCode = emailConfirmString;
            userEntity.EmailVerified = false;
            DatabaseContext.SaveChanges();

            var emailService = new EmailService();
            emailService.SendEmailConfirmEmail(user.Email, emailConfirmString);
            return new BaseResult() { Result = eResultType.Success };
        }

        public BaseResult PasswordResetRequest(string email)
        {
            var userEntity = DatabaseContext.Users.FirstOrDefault(x => x.Email == email);
            if (userEntity == null)
            {
                return new BaseResult() { Result = eResultType.NotFound };
            }
            var passwordResetToken = ExtensionMethods.RandomString(20);
            

            userEntity.PasswordResets.Add(new PasswordReset()
                                          {
                                              GenerationDateTime = DateTime.Now,
                                              Token = passwordResetToken
            });
            DatabaseContext.SaveChanges();
            var emailService = new EmailService();
            emailService.SendPasswordRecoveryEmail(userEntity.Email, passwordResetToken);
            return new BaseResult(){Result = eResultType.Success};
        }

        public BaseResult Update(User user)
        {
            var userEntity = DatabaseContext.Users.Find(user.UserId);

            if (userEntity == null)
            {
                return new BaseResult()
                       {
                           Result = eResultType.NotFound,
                           Message = "account.userNotFound"
                       };
            }

            Logger.Log(LogLevel.Information, "User update request, from: "
                                         + JsonConvert.SerializeObject(new User {UserId = userEntity.UserId, Email = userEntity.Email, Username = userEntity.Username})
                                         + " to: "
                                         + JsonConvert.SerializeObject(new User {UserId = user.UserId, Email = user.Email, Username = user.Username}));
            // test zaj�to�ci loginu
            if (user.Username != null && user.Username != userEntity.Username && DatabaseContext.Users.Any(x => x.Username == user.Username))
            {
                Logger.Log(LogLevel.Warning, "User " + user.UserId + " username update request failed: username " + user.Username + " is taken");
                return new BaseResult()
                       {
                           Result = eResultType.Error,
                           Message = "account.usernameTaken"
                       };
            }

            if (user.Username != null)
            {
                userEntity.Username = user.Username;
            }

            if (user.Email != null)
            {
                userEntity.Email = user.Email;
            }

            try
            {
                DatabaseContext.SaveChanges();
                return new BaseResult() {Result = eResultType.Success};
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "User " + user.UserId + " update: Exception " + (e.InnerException != null ? e.InnerException.Message : e.Message));
                return new BaseResult()
                       {
                           Result = eResultType.Error
                       };
            }
        }

        public BaseResult UpdateRoles(User user, List<eRole> roles)
        {
            var oldRoles = DatabaseContext.UserRoles.Where(x => x.UserId == user.UserId);
            DatabaseContext.UserRoles.RemoveRange(oldRoles);
            var newRoles = roles.Select(x => new UserRole()
                                             {
                                                 UserId = user.UserId,
                                                 Role = x
                                             });
            DatabaseContext.UserRoles.AddRange(newRoles);
            try
            {
                DatabaseContext.SaveChanges();
                return new BaseResult() {Result = eResultType.Success};
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "User " + user.UserId + " roles update: Exception " + (e.InnerException != null ? e.InnerException.Message : e.Message));
                return new BaseResult()
                       {
                           Result = eResultType.Error
                       };
            }
        }

        public BaseResult ChangePassword(User userEntity, string newPassword)
        {
            var hashedPassword = HashPassword(newPassword);
            userEntity.Password = hashedPassword;
            var passwordChange = new PasswordChange
                                 {
                                     UserId = userEntity.UserId,
                                     ChangeDateTime = DateTime.Now
                                 };
            DatabaseContext.PasswordChanges.Add(passwordChange);
            try
            {
                DatabaseContext.SaveChanges();
                return new BaseResult() {Result = eResultType.Success};
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "User " + userEntity.UserId + " password change: Exception " + (e.InnerException != null ? e.InnerException.Message : e.Message));
                return new BaseResult()
                       {
                           Result = eResultType.Error
                       };
            }
        }

        public BaseResult ResetPassword(User user, string newPassword, string resetToken)
        {
            var userEntity = DatabaseContext.Users.FirstOrDefault(x => x.UserId == user.UserId);
            if (userEntity == null)
            {
                return new BaseResult(){Result = eResultType.NotFound};
            }
            var matchingToken = userEntity.PasswordResets.FirstOrDefault(x => x.Token == resetToken && x.GenerationDateTime >= DateTime.Now.AddDays(-1));
            if (matchingToken != null)
            {
                DatabaseContext.PasswordResets.Remove(matchingToken);
                return ChangePassword(userEntity, newPassword);
            }
            return new BaseResult() { Result = eResultType.NotFound };
        }

        public BaseResult Delete(int id)
        {
            var user = DatabaseContext.Users.Find(id);
            if (user == null)
            {
                Logger.Log(LogLevel.Warning, "User " + id + " deletion: User not found");
                return new BaseResult()
                       {
                           Result = eResultType.NotFound,
                           Message = "account.userNotFound"
                       };
            }

            DatabaseContext.Users.Remove(user);
            try
            {
                DatabaseContext.SaveChanges();
                Logger.Log(LogLevel.Information, "User " + user.UserId + " deletion: success");
                return new BaseResult() {Result = eResultType.Success};
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "User " + user.UserId + " deletion: Exception " + (e.InnerException != null ? e.InnerException.Message : e.Message));
                return new BaseResult()
                       {
                           Result = eResultType.Error
                       };
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email).Address;
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}