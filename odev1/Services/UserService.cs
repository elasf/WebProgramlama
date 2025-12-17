using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using odev1.Models;

namespace odev1.Services
{

    public class UserService
    {
        private readonly UserManager<UserDetails> _userManager;

        public UserService(UserManager<UserDetails> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDetails> GetByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new KeyNotFoundException("user not found");
            }

            return user;

        }

        public async Task<UserDetails> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("email not founf");

            return user;
        }

        public async Task<UserDetails> updateProfileAsync(
                string userId,
                string firstName,
                string lastName,
                string  phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.userAd = firstName;
            user.userSoyad = lastName;
            user.PhoneNumber = phoneNumber;

            var result=await _userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                 );


            }

            return user ;

        }






    }


}