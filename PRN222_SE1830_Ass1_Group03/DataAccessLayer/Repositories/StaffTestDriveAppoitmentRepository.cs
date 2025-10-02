using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface ITestDriveApoitmentRepository
    {
        /// <summary>
        /// truyen vao id cua dealer dang login vao web
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<TestDriveAppointment>> GetAllTestDriveAppoitments(Guid id); 
        Task<Boolean> CompleteTestDriveAppoitments(Guid apmId);
        Task<Boolean> BrowseTestDriveAppoitments(Boolean browse, Guid apoitmentsId);
        Task<List<TestDriveAppointment>> GetAppointmentsByCusId(Guid cusId);
        Task<TestDriveAppointment?> GetAppointmentById(Guid apoitmentsId);
    }

    public class StaffTestDriveAppoitmentRepository : ITestDriveApoitmentRepository
    {
        private enum Status
        {
            pending,
            process,
            confirmed,
            cancelled,
            completed
        }

        private readonly Vehicle_Dealer_ManagementContext _context;

        public StaffTestDriveAppoitmentRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> BrowseTestDriveAppoitments(bool browse, Guid apoitmentsId)
        {
            try
            {
                var apm = await _context.TestDriveAppointments.FirstOrDefaultAsync(x => x.Id == apoitmentsId);
                if (apm == null)
                {
                    return false;
                }
                if (browse)
                {
                    apm.Status = Status.confirmed.ToString();
                }
                else
                {
                    apm.Status = Status.cancelled.ToString();
                }
                if(await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> CompleteTestDriveAppoitments(Guid apmId)
        {
            try
            {
                var apm = await _context.TestDriveAppointments.FirstOrDefaultAsync(x => x.Id == apmId);
                if (apm == null)
                {
                    return false;
                }
                apm.Status = Status.completed.ToString();
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<TestDriveAppointment?> GetAppointmentById(Guid id)
        {
            try
            {
                return await _context.TestDriveAppointments
                    .Include(a => a.Customer)
                    .Include(a => a.Vehicle)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<TestDriveAppointment>> GetAppointmentsByCusId(Guid id)
        {
            try
            {
                return await _context.TestDriveAppointments.Where(x => x.CustomerId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<TestDriveAppointment>> GetAllTestDriveAppoitments(Guid dealerId)
        {
            try
            {
                return await _context.TestDriveAppointments
                    .Include(x => x.Customer)
                    .Include(x => x.Vehicle)
                    .Where(x => x.DealerId == dealerId)
                    .ToListAsync();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
