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
    /// <summary>
    /// Provides user-related business logic including user creation, validation, and retrieval.
    /// </summary>
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

        /// <summary>
        /// Creates a new user with hashed password and persists it in the repository.
        /// </summary>
        /// <param name="requestDto">User registration data transfer object.</param>
        public async Task CreateUserAsync(RegisterRequestDto requestDto)
        {
            // Create a new User entity from registration data
            var user = new User
            {
                FullName =requestDto.FullName,
                Email = requestDto.Email,
            };

            // Hash the user's password securely
            user.Password = passwordHasher.HashPassword(user, requestDto.Password);

            // Add user to repository and save changes
            await repositoryManager.UserRepository.AddAsync(user);
            await repositoryManager.SaveAsync();

            // Log successful user creation with details
            logger.LogInformation("User created successfully: Id={UserId}, FullName={FullName}, Email={Email}", user.Id, user.FullName, user.Email);
        }

        /// <summary>
        /// Validates user credentials by verifying email and password.
        /// </summary>
        /// <param name="requestDto">User login data transfer object.</param>
        /// <returns>The user entity if valid; otherwise, null.</returns>
        public async Task<User?> ValidateUserAsync(LoginRequestDto requestDto)
        {
            // Retrieve user by email from repository
            var user = await repositoryManager.UserRepository.GetByEmailAsync(requestDto.Email);
            if (user == null) return null;

            // Verify provided password against stored hashed password
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, requestDto.Password);

            // Return user if password is valid, otherwise null
            return result == PasswordVerificationResult.Success ? user : null;
        }

        /// <summary>
        /// Retrieves all users and maps them to response DTOs.
        /// </summary>
        /// <returns>A collection of user response data transfer objects.</returns>
        public async Task<IEnumerable<UserResponseDto?>> GetUsers()
        {
            // Retrieve all users from repository
            var users = await repositoryManager.UserRepository.GetAllAsync();

            // Map user entities to response DTOs
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.FullName
            });
        }
    }
}
