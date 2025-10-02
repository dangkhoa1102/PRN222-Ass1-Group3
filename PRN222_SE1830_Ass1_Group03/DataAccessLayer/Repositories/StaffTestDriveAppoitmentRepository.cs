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
        
        // New methods for customer functionality
        Task<bool> CreateTestDriveAppointment(TestDriveAppointment appointment);
        Task<List<Vehicle>> GetAvailableVehiclesForTestDrive();
        Task<bool> IsTimeSlotAvailable(Guid vehicleId, DateTime appointmentDate);
        Task<bool> CancelCustomerAppointment(Guid appointmentId, Guid customerId);
        Task<Dealer?> GetFirstAvailableDealer();
        Task<List<TestDriveAppointment>> GetAllTestDriveAppointments(); // Method mới để lấy tất cả
    }

    public class StaffTestDriveAppoitmentRepository : ITestDriveApoitmentRepository
    {
        private enum Status
        {
            pending,     // Chờ xử lý
            confirmed,   // Đã xác nhận
            completed,   // Hoàn thành
            cancelled    // Đã hủy
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
                apm.UpdatedAt = DateTime.Now;
                
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
                apm.UpdatedAt = DateTime.Now;
                
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
                    .Include(a => a.Dealer)
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
                return await _context.TestDriveAppointments
                    .Include(x => x.Vehicle)
                    .Include(x => x.Dealer)
                    .Where(x => x.CustomerId == id)
                    .OrderByDescending(x => x.CreatedAt ?? x.AppointmentDate)
                    .ToListAsync();
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
                // Nếu dealerId là Guid.Empty, lấy tất cả
                if (dealerId == Guid.Empty)
                {
                    return await _context.TestDriveAppointments
                        .Include(x => x.Customer)
                        .Include(x => x.Vehicle)
                        .Include(x => x.Dealer)
                        .OrderByDescending(x => x.CreatedAt ?? x.AppointmentDate)
                        .ToListAsync();
                }
                
                // Lấy theo dealer cụ thể
                return await _context.TestDriveAppointments
                    .Include(x => x.Customer)
                    .Include(x => x.Vehicle)
                    .Include(x => x.Dealer)
                    .Where(x => x.DealerId == dealerId)
                    .OrderByDescending(x => x.CreatedAt ?? x.AppointmentDate)
                    .ToListAsync();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // SỬA CREATE METHOD
        public async Task<bool> CreateTestDriveAppointment(TestDriveAppointment appointment)
        {
            try
            {
                if (appointment == null)
                    return false;

                // Ensure all required fields are set
                if (appointment.Id == Guid.Empty)
                    appointment.Id = Guid.NewGuid();
                    
                if (appointment.CreatedAt == DateTime.MinValue)
                    appointment.CreatedAt = DateTime.Now;
                    
                if (appointment.UpdatedAt == DateTime.MinValue)
                    appointment.UpdatedAt = DateTime.Now;

                // ĐẶT STATUS THÀNH "pending" (đúng theo database constraint)
                if (string.IsNullOrEmpty(appointment.Status))
                    appointment.Status = Status.pending.ToString();

                _context.TestDriveAppointments.Add(appointment);
                return await _context.SaveChangesAsync() > 0;
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
                return await _context.Vehicles
                    .Where(v => v.IsActive == true && v.StockQuantity > 0)
                    .OrderBy(v => v.Name)
                    .ToListAsync();
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
                // Check if there's already an appointment for this vehicle at the same date and time
                // Allow 2-hour buffer between appointments
                var startTime = appointmentDate.AddHours(-2);
                var endTime = appointmentDate.AddHours(2);
                
                // KIỂM TRA CẢ "pending" VÀ "confirmed"
                var existingAppointment = await _context.TestDriveAppointments
                    .FirstOrDefaultAsync(a => a.VehicleId == vehicleId 
                                             && a.AppointmentDate >= startTime
                                             && a.AppointmentDate <= endTime
                                             && (a.Status == Status.pending.ToString() 
                                                || a.Status == Status.confirmed.ToString()));
                
                return existingAppointment == null;
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
                var appointment = await _context.TestDriveAppointments
                    .FirstOrDefaultAsync(a => a.Id == appointmentId && a.CustomerId == customerId);
                
                if (appointment == null)
                    return false;

                // CHO PHÉP HỦY CẢ "pending" VÀ "confirmed"
                if (appointment.Status == Status.pending.ToString() || appointment.Status == Status.confirmed.ToString())
                {
                    appointment.Status = Status.cancelled.ToString();
                    appointment.UpdatedAt = DateTime.Now;
                    return await _context.SaveChangesAsync() > 0;
                }

                return false;
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
                return await _context.Dealers
                    .Where(d => d.IsActive == true)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // Method mới
        public async Task<List<TestDriveAppointment>> GetAllTestDriveAppointments()
        {
            try
            {
                return await _context.TestDriveAppointments
                    .Include(x => x.Customer)
                    .Include(x => x.Vehicle)
                    .Include(x => x.Dealer)
                    .OrderByDescending(x => x.CreatedAt ?? x.AppointmentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
