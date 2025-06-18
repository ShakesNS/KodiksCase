using KodiksCase.Application.Services.Interfaces;
using KodiksCase.Persistence.Managers;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.DTOs.Responses;
using KodiksCase.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager repositoryManager;
        private readonly PasswordHasher<User> passwordHasher;
        private readonly ILogger<UserService> logger;
        public UserService(IRepositoryManager repositoryManager, ILogger<UserService> logger)
        {
            this.repositoryManager = repositoryManager;
            this.passwordHasher = new PasswordHasher<User>();
            this.logger = logger;
        }

        public async Task CreateUserAsync(RegisterRequestDto requestDto)
        {
            var user = new User
            {
                FullName =requestDto.FullName,
                Email = requestDto.Email,
            };

            user.Password = passwordHasher.HashPassword(user, requestDto.Password);
            await repositoryManager.UserRepository.AddAsync(user);
            await repositoryManager.SaveAsync();
            logger.LogInformation("User created successfully: Id={UserId}, FullName={FullName}, Email={Email}", user.Id, user.FullName, user.Email);
        }

        public async Task<User?> ValidateUserAsync(LoginRequestDto requestDto)
        {
            var user = await repositoryManager.UserRepository.GetByEmailAsync(requestDto.Email);
            if (user == null) return null;

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, requestDto.Password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<IEnumerable<UserResponseDto?>> GetUsers()
        {
            var users = await repositoryManager.UserRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.FullName
            });
        }
    }
}
