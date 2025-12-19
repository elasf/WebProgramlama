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

        public async Task<IdentityResult> createTrainerAsync(TrainerRegisterViewModel model)
        {

            if (UserExists(model.email)){

                return IdentityResult.Failed(new IdentityError { Description = "Bu email adresi zaten kayıtlı." });
            }

            var user = new UserDetails
            {
                UserName = model.email,
                Email = model.email,
                userAd = model.firstName,
                userSoyad = model.lastName,
                PhoneNumber = model.phoneNumber,
                EmailConfirmed = true //admin ekledği için direkt onay
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (!result.Succeeded){

                return result;
            }

            //rolü ata trainer tablosuna kaydet diyoruz
            await assignTrainerRoleTable(user, model);

            return IdentityResult.Success;
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

        private async Task assignTrainerRoleTable(UserDetails user, TrainerRegisterViewModel model)
        {

            await _userManager.AddToRoleAsync(user, "trainer");

            //dbye ekledik
            var trainer = new Trainer
            {
                userId = user.Id, //bağlantıyı kurduk
                fullName = $"{model.firstName} {model.lastName}",
            };

            await _context.Trainers.AddAsync(trainer);
            await _context.SaveChangesAsync();
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