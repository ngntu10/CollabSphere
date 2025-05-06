using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Infrastructures.Repositories;
using CollabSphere.Modules.User.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.User.Service
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid Id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        // Task<Entities.Domain.User> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseModel> UpdateUserAsync(Guid id, UpdateUserDto updateDto, Guid updatedById);
    }

    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public UserService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
                throw new NotFoundException("User not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        // public async Task<Entities.Domain.User> CreateUserAsync(CreateUserDto createUserDto)
        // {
        //     if (createUserDto == null)
        //         throw new ArgumentNullException(nameof(createUserDto), "CreateUserDto không được null.");

        //     var user = _mapper.Map<Entities.Domain.User>(createUserDto)
        //         ?? throw new InvalidOperationException("Mapping failed.");

        //     user.Id = Guid.NewGuid();
        //     user.UserName = createUserDto.UserName;
        //     user.Email = createUserDto.Email;
        //     user.PhoneNumber = createUserDto.PhoneNumber;
        //     user.AvatarId = createUserDto.AvatarId;
        //     user.Gender = createUserDto.Gender;
        //     user.CreatedOn = DateTime.UtcNow;
        //     user.UpdatedOn = DateTime.UtcNow;

        //     _context.Users.Add(user);
        //     await _context.SaveChangesAsync();

        //     return user;
        // }

        public async Task<UserResponseModel> UpdateUserAsync(Guid id, UpdateUserDto updateDto, Guid updatedById)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("Người dùng không tồn tại");

            user.UserName = updateDto.UserName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.AvatarId = updateDto.AvatarId;
            user.Gender = updateDto.Gender;
            user.UpdatedOn = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponseModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarId = user.AvatarId,
                Gender = user.Gender,
                UpdatedOn = user.UpdatedOn,
            };
        }
    }
}
