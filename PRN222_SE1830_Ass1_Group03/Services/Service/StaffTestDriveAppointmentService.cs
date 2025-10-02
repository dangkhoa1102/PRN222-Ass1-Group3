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
    }
}
