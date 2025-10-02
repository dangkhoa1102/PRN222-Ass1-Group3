using BusinessObjects.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public interface ITestDriveAppointmentService
    {
        Task<List<TestDriveAppointment>> GetAllTestDriveAppointments(Guid id);
        Task<bool> CompleteTestDriveAppointments(Guid apmId);
        Task<bool> BrowseTestDriveAppointments(bool browse, Guid apoitmentsId);
        Task<List<TestDriveAppointment>> GetAppointmentsByCusId(Guid cusId);
        Task<TestDriveAppointment?> GetAppointmentById(Guid apoitmentsId);
        
        // New methods for customer functionality
        Task<bool> CreateTestDriveAppointment(TestDriveAppointment appointment);
        Task<List<Vehicle>> GetAvailableVehiclesForTestDrive();
        Task<bool> IsTimeSlotAvailable(Guid vehicleId, DateTime appointmentDate);
        Task<bool> CancelCustomerAppointment(Guid appointmentId, Guid customerId);
        Task<Dealer?> GetFirstAvailableDealer();
    }

    public class StaffTestDriveAppointmentService : ITestDriveAppointmentService
    {
        private readonly ITestDriveApoitmentRepository _repo;
        public StaffTestDriveAppointmentService(ITestDriveApoitmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> BrowseTestDriveAppointments(bool browse, Guid apoitmentsId)
        {
            try
            {
                return await _repo.BrowseTestDriveAppoitments(browse, apoitmentsId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> CompleteTestDriveAppointments(Guid apmId)
        {
            try
            {
                return await _repo.CompleteTestDriveAppoitments(apmId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<TestDriveAppointment>> GetAllTestDriveAppointments(Guid id)
        {
            try
            {
                var apm = await _repo.GetAllTestDriveAppoitments(id);
                return apm ?? new List<TestDriveAppointment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<TestDriveAppointment?> GetAppointmentById(Guid apoitmentsId)
        {
            try
            {
                return await _repo.GetAppointmentById(apoitmentsId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<TestDriveAppointment>> GetAppointmentsByCusId(Guid cusId)
        {
            try
            {
                var appointments = await _repo.GetAppointmentsByCusId(cusId);
                return appointments ?? new List<TestDriveAppointment>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // New implementations for customer functionality
        public async Task<bool> CreateTestDriveAppointment(TestDriveAppointment appointment)
        {
            try
            {
                if (appointment == null)
                    return false;

                // Set default values
                appointment.Id = Guid.NewGuid();
                appointment.Status = "pending";
                appointment.CreatedAt = DateTime.Now;
                appointment.UpdatedAt = DateTime.Now;

                return await _repo.CreateTestDriveAppointment(appointment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<Vehicle>> GetAvailableVehiclesForTestDrive()
        {
            try
            {
                return await _repo.GetAvailableVehiclesForTestDrive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> IsTimeSlotAvailable(Guid vehicleId, DateTime appointmentDate)
        {
            try
            {
                return await _repo.IsTimeSlotAvailable(vehicleId, appointmentDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> CancelCustomerAppointment(Guid appointmentId, Guid customerId)
        {
            try
            {
                return await _repo.CancelCustomerAppointment(appointmentId, customerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<Dealer?> GetFirstAvailableDealer()
        {
            try
            {
                return await _repo.GetFirstAvailableDealer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
