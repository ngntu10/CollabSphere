using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using CollabSphere.Entities.Domain;
using CollabSphere.Modules.User.Models;
using CollabSphere.Database;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CollabSphere.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace CollabSphere.Modules.User.Service
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<Entities.Domain.User> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseModel> UpdateUserAsync(Guid id, UpdateUserDto updateDto, Guid updatedByUserId);
    }

    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Entities.Domain.User> _userRepository;

        public UserService(DatabaseContext context, IMapper mapper, IBaseRepository<Entities.Domain.User> userRepository)
        {
            _context = context;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                throw new NotFoundException("User not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<Entities.Domain.Post> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                throw new ArgumentNullException(nameof(createUserDto), "CreateUserDto không được null.");
            }

            try
            {
                if (string.IsNullOrEmpty(createUserDto.Name))
                    throw new ArgumentException("Name is required");
                if (string.IsNullOrEmpty(createUserDto.Email))
                    throw new ArgumentException("Email is required");

                var user = _mapper.Map<CollabSphere.Entities.Domain.User>(createUserDto)
                        ?? throw new InvalidOperationException("Mapping failed.");

                user.UserID = Guid.NewGuid();
                user.Name = createUserDto.Name;
                user.Email = createUserDto.Email;
                user.PhoneNumber = "";
                user.AvatarId = "";
                user.CreatedOn = DateTime.UtcNow;
                user.UpdatedOn = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"Database error while creating post: {innerMessage}", dbEx);
            }
            catch (AutoMapperMappingException mapEx)
            {
                throw new Exception($"Mapping error: {mapEx.Message}", mapEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while creating post: {ex.Message}", ex);
            }
        }

        public async Task<UserResponseModel> UpdateUserAsync(Guid userId, UpdateUserModel model)
        {
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                throw new NotFoundException("Người dùng không tồn tại");

            user.Email = model.Email;
            user.Name = model.Name;
            user.Gender = model.Gender;
            user.Phone = model.Phone;
            user.AvatarId = model.AvatarId;
            user.UpdatedOn = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponseModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                AvatarId = user.AvatarId,
                Gender = user.Gender,
                UpdatedOn = user.UpdatedOn,
            };
        }
    }
}