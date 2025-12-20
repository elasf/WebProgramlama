using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Models.ViewModels;
using TrainerServiceEntity = odev1.Models.TrainerService;

namespace odev1.Services
{
    public class TrainerManageService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDetails> _userManager;

        public TrainerManageService(ApplicationDbContext context, UserManager<UserDetails> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //listeleme icn
        public async Task<List<Trainer>> GetAllTrainersAsync()
        {
            return await _context.Trainers
                .Include(t => t.User) // İsim, email buradan gelecek
                .Include(t => t.trainerServices).ThenInclude(ts => ts.service) // Hizmet isimleri
                .Include(t => t.trainerExpertises).ThenInclude(te => te.expertise) // Uzmanlık isimleri
                .ToListAsync();
        }

        public List<Service> GetAllServices() => _context.Services.ToList(); 
        public List<Expertise> GetAllExpertises() => _context.Expertises.ToList(); 

        public async Task<IdentityResult> CreateTrainerAsync(TrainerManageViewModel model)
        {
            var user = new UserDetails
            {
                UserName = model.email,
                Email = model.email,
                PhoneNumber=model.PhoneNumber,
                userAd = model.FirstName,
                userSoyad = model.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return result;

            await _userManager.AddToRoleAsync(user, "trainer");

            var trainer = new Trainer
            {
                userId = user.Id,
                fullName = $"{model.FirstName} {model.LastName}"
            };

            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync(); 

           
            await AddRelationshipsAsync(trainer.id, model.SelectedServiceIds, model.SelectedExpertiseIds);

            return IdentityResult.Success;
        }


        public async Task<Trainer> GetTrainerByIdAsync(int id)
        {
            return await _context.Trainers
                .Include(t => t.User)
                .Include(t => t.trainerServices)
                .Include(t => t.trainerExpertises)
                .FirstOrDefaultAsync(x => x.id == id);
        }


        public async Task UpdateTrainerAsync(TrainerManageViewModel model)
        {
            var trainer = await _context.Trainers.FindAsync(model.TrainerId);
            if (trainer == null) return;

            trainer.fullName = $"{model.FirstName} {model.LastName}";

            var user = await _userManager.FindByIdAsync(trainer.userId);
            if (user != null)
            {
                user.userAd = model.FirstName;
                user.userSoyad = model.LastName;
                user.Email = model.email;
                user.UserName = model.email;
                user.PhoneNumber = model.PhoneNumber;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, model.Password);
                }

                await _userManager.UpdateAsync(user);
            }

            var oldServices = _context.TrainerServices.Where(x => x.trainerId == trainer.id);
            _context.TrainerServices.RemoveRange(oldServices);

            var oldExpertises = _context.TrainerExpertises.Where(x => x.trainerId == trainer.id);
            _context.TrainerExpertises.RemoveRange(oldExpertises);

            await _context.SaveChangesAsync();


            await AddRelationshipsAsync(trainer.id, model.SelectedServiceIds, model.SelectedExpertiseIds);
        }

        public async Task DeleteTrainerAsync(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return;

            //once userı siliyorum
            var user = await _userManager.FindByIdAsync(trainer.userId);

            _context.Trainers.Remove(trainer);

            if (user != null) await _userManager.DeleteAsync(user);

            await _context.SaveChangesAsync();
        }
        //yardımcı fonks
        private async Task AddRelationshipsAsync(int trainerId, int[] serviceIds, int[] expertiseIds)
        {
            if (serviceIds != null)
            {
                foreach (var sId in serviceIds)
                {
                    _context.TrainerServices.Add(new TrainerServiceEntity { trainerId = trainerId, serviceId = sId });
                }
            }

            if (expertiseIds != null)
            {
                foreach (var eId in expertiseIds)
                {
                    _context.TrainerExpertises.Add(new TrainerExpertise { trainerId = trainerId, expertiseId = eId });
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}