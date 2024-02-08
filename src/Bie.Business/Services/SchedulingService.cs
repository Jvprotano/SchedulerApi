using Bie.Business.Interfaces.Repositories;
using Bie.Business.Interfaces.Repositories.Base;
using Bie.Business.Interfaces.Services;
using Bie.Business.Models;
using Bie.Business.Services.Base;

namespace Bie.Business.Services;
public class SchedulingService : Service<Scheduling>, ISchedulingService
{
    private readonly ISchedulingRepository _repositoryScheduling;
    private readonly ICompanyOpeningHoursRepository _repositoryCompanyOpeningHours;
    private readonly ICompanyService _serviceCompany;

    public SchedulingService(IRepository<Scheduling> repositoryBase, ISchedulingRepository repositoryScheduling,
    ICompanyOpeningHoursRepository repositoryCompanyOpeningHours, ICompanyService serviceCompany) : base(repositoryBase)
    {
        _repositoryScheduling = repositoryScheduling;
        _repositoryCompanyOpeningHours = repositoryCompanyOpeningHours;
        _serviceCompany = serviceCompany;
    }
    public override void Validate(Scheduling entity)
    {
        if (String.IsNullOrWhiteSpace(entity.CompanyId))
            throw new Exception("Company is required");
        // if (String.IsNullOrWhiteSpace(entity.CustomerId))
        //     throw new Exception("Customer is required");
        if (entity.ScheduledDate == default)
            throw new Exception("Scheduled date is required");
        if (String.IsNullOrWhiteSpace(entity.ServicesOfferedId))
            throw new Exception("Service is required");

        base.Validate(entity);
    }

    public override Task SaveAsync(Scheduling entity)
    {
        return base.SaveAsync(entity);
    }

    public async Task<IEnumerable<Scheduling>> GetAllOpenByCompanyIdAsync(string companyId, DateTime initialDate, DateTime finalDate)
    {
        return await _repositoryScheduling.GetAllOpenByCompanyIdAsync(companyId, initialDate, finalDate);
    }

    public async Task<IEnumerable<TimeSpan>> GetAvailableTimesAsync(string companyId, string serviceSelected, DateOnly date)
    {
        List<CompanyOpeningHours> openingHours = _repositoryCompanyOpeningHours.GetByDayOfWeek(companyId, date.DayOfWeek);

        List<TimeSpan> availableTimes = new();

        if (openingHours.Count > 0)
        {
            Company company = await _serviceCompany.GetByIdAsync(companyId);

            CompanyServiceOffered? serviceOffered = company.ServicesOffered.Where(c => c.Id == serviceSelected).FirstOrDefault();

            // var shortestServiceDuration = company.ServicesOffered.Where(c => c.Duration > new TimeSpan()).Select(c => c.Duration).Min();
            // if (shortestServiceDuration == default)
            //     throw new Exception("Não há serviços cadastrados");

            var shortestServiceDuration = new TimeSpan(0, 10, 0);

            if (date == DateOnly.FromDateTime(DateTime.Now))
                openingHours.RemoveAll(c => c.OpeningHour < DateTime.Now.TimeOfDay && c.ClosingHour < DateTime.Now.TimeOfDay);

            foreach (var item in openingHours)
            {
                if (item.OpeningHour == item.ClosingHour)
                    continue;
                if (item.OpeningHour > item.ClosingHour)
                    throw new Exception("Opening hour cannot be greater than closing hour");

                if (date == DateOnly.FromDateTime(DateTime.Now) && item.OpeningHour < DateTime.Now.TimeOfDay)
                    item.OpeningHour = DateTime.Now.TimeOfDay;

                var opening = item.OpeningHour;
                var closing = item.ClosingHour;

                while (opening <= closing - serviceOffered?.Duration)
                {
                    availableTimes.Add(opening);
                    opening = opening.Add(shortestServiceDuration);
                }
            }

            List<Scheduling> busyTimes = (await _repositoryScheduling.GetAllByDateAsync(companyId, date)).ToList();

            if (date == DateOnly.FromDateTime(DateTime.Now))
                availableTimes.RemoveAll(c => c < DateTime.Now.TimeOfDay);

            foreach (var item in busyTimes)
            {
                var scheduledHour = item.ScheduledDate.TimeOfDay;
                var scheduledDuration = item.ServiceOffered.Duration;

                TimeSpan finalBusyTime = scheduledHour.Add(scheduledDuration);

                availableTimes.RemoveAll(c => c >= scheduledHour && c <= finalBusyTime);
            }
        }

        return availableTimes;
    }
}