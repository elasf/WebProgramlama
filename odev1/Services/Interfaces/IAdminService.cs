using Microsoft.AspNetCore.Identity;
using odev1.Models;
using odev1.Models.ViewModels;

namespace odev1.Services
{
    public interface IAdminService
    {
        Task<IdentityResult> createTrainerAsync(TrainerRegisterViewModel model);
        List<Member> getAllMembers();

        Task<List<Service>> GetAllServicesAsync();
        Task<Service> GetServiceByIdAsync(int id);
        Task CreateServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(int id);
    }
}

