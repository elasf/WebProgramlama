using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Models.ViewModels;

namespace odev1.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDetails> _userManager;

        public AdminService(ApplicationDbContext context, UserManager<UserDetails> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        

        public List<Member> getAllMembers()
        {
            
            return _context.Members
                           .Include(m => m.User)
                           .ToList();
        }

        private bool UserExists(string email)
        {
            //email checki yapıyoruz
            return _context.Users.Any(u => u.Email == email);
        }

        


        // ---servis yönetim kısmı ---

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _context.Services.ToListAsync();
        }

        //id ile getir fonks idk
        public async Task<Service> GetServiceByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }


        public async Task CreateServiceAsync(Service service)
        {
            _context.Services.Add(service);
            var expertise = new Expertise
            {
                expertise = service.name //hizmet eklerken uzmanlığa da ekliyoruz pratik olsun diye
            };

            
            _context.Expertises.Add(expertise);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateServiceAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}