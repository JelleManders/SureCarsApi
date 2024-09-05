using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SureCarsApi.DatabaseContext;
using SureCarsApi.Models;

namespace SureCarsApi.Controllers
{
    [ApiController]
    public class SureCarController : ControllerBase
    {
        private readonly SureCarDbContext _context;
        private string[] _validStatuses = [
            "Leased", "Available", "UnderRepair", "Sold", "Ordered"
            ];

        public SureCarController(SureCarDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Init")]
        public IActionResult InitCarDB()
        {
            for (int i = 0; i < 100; i++)
            {
                var car = new SureCarDbo();
                car.Color = "Purple";
                car.PlateNumber = i < 10 ? $"TEST-0{i}" : $"TEST-{i}";
                car.Year = 1900 + i;
                car.LeasedBy = null;
                car.Status = "Available";
                car.Remarks = $"Test Car #{i}";

                _context.SureCars.Add(car);
            }

            _context.SaveChanges();

            return Ok("Database has been initialised");

        }

        [HttpGet]
        [Route("Cars")]
        public IActionResult GetSureCars()
        {
            return Ok(_context.SureCars.ToList());
        }

        [HttpGet]
        [Route("Cars/Filter")]
        public IActionResult GetSureCars([FromBody] FilterCarsRequest request)
        {
            if (request.Status != null && !isValidStatus(request.Status)) return BadRequest("Invalid Status");

            DbSet<SureCarDbo> dbset = _context.SureCars;
            IQueryable<SureCarDbo> query = dbset.Where(_ => true);

            if (request.UserId != null)
            {
                Guid userId = new Guid(request.UserId);
                query = query.Where(c => c.ID == userId);
            }
            
            if(request.Status != null)
            {
                query = query.Where(c => c.Status == request.Status);
            }

            SureCarDbo[] result = query.ToArray();
            return Ok(result);
        }

        [HttpPost]
        [Route("AddCar")]
        public IActionResult CreateSureCar([FromBody] SureCarDto car)
        {
            if (!isValidStatus(car.Status)) return BadRequest("Invalid Status");

            SureCarDbo newCar = new SureCarDbo();
            newCar.ID = Guid.NewGuid();
            newCar.Color = car.Color;
            newCar.PlateNumber = car.PlateNumber;
            newCar.Year = car.Year;
            newCar.LeasedBy = car.LeasedBy != null ? new Guid(car.LeasedBy) : null;
            newCar.Status = car.Status;
            newCar.Remarks = car.Remarks;
            
            _context.SureCars.Add(newCar);
            _context.SaveChanges();

            return Ok(newCar);
        }

        [HttpPatch]
        [Route("SetStatus")]
        public IActionResult UpdateStatus([FromBody] PatchCarRequest patchDto) 
        {
            if (patchDto.CarId == null) return BadRequest("Request should have valid body");

            Guid patchId = new Guid(patchDto.CarId);
            SureCarDbo[] result = _context.SureCars.Where(c => c.ID == patchId).ToArray();

            if (result.Length == 0)                 return NotFound("Car ID was not found");
            if (result[0].Status == "Sold")         return BadRequest("Car has already been sold"); 
            if (!isValidStatus(patchDto.Status))    return BadRequest("Invalid Status");

            SureCarDbo car = result[0];
            car.Status = patchDto.Status;

            _context.SureCars.Update(car);
            _context.SaveChanges();

            return Ok(car.ToDto());
        }

        [HttpPost]
        [Route("LeaseCar")]
        public IActionResult LeaseCar([FromBody] LeaseCarRequest request) 
        {
            Guid requestCarId = new Guid(request.CarId);
            Guid requestUserId = new Guid(request.UserId);

            SureCarDbo[] cars = _context.SureCars.Where(c => c.ID == requestCarId).ToArray();
            if (cars.Length == 0) return NotFound("Car ID was not found");

            SureUserDbo[] users = _context.SureUsers.Where(u => u.ID == requestUserId).ToArray();
            if (users.Length == 0) return NotFound("User ID was not found");

            SureCarDbo car = cars[0];
            if (car.Status != "Available") return BadRequest("Indicated Car is not Vaialable");

            SureUserDbo user = users[0];
            car.Status = "Leased";
            car.LeasedBy = user.ID;

            _context.SureCars.Update(car);
            _context.SaveChanges();

            return Ok(car.ToDto());
        }

        private bool isValidStatus(string status)
        {
            return _validStatuses.Contains(status);
        }
    }
}
