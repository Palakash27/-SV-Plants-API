using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plants_API.Models;

namespace Plants_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly PlantContext _context;

        public PlantsController(PlantContext context)
        {
            _context = context;
        }

        // GET: api/Plants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> Getplants()
        {
            return await _context.plants.ToListAsync();
        }

        [HttpPut("{id}/start-watering")]
        public async Task<IActionResult> StartWatering(int id)
        {
            int COOL_DOWN_SECONDS = 30;
            int WATERING_SECONDS = 10;

            var plant = await _context.plants.FirstAsync(plant => plant.Id == id);

            if (plant == null)
            {
                return NotFound();
            }

            DateTime lastWateredEndDate = DateTime.Parse(plant.LastWateringEnd);
            DateTime nowDate = DateTime.Now;
            if (lastWateredEndDate.AddSeconds(COOL_DOWN_SECONDS) >= nowDate)
            {
                return BadRequest();
            }

            plant.LastWateringEnd = string.Concat(nowDate.AddSeconds(WATERING_SECONDS).ToUniversalTime().ToString("s"), "Z");

            _context.Entry(plant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("{id}/stop-watering")]
        public async Task<IActionResult> StopWatering(int id)
        {
            var plant = await _context.plants.FirstAsync(plant => plant.Id == id);

            if (plant == null)
            {
                return NotFound();
            }

            DateTime lastWateredEndDate = DateTime.Parse(plant.LastWateringEnd);
            DateTime nowDate = DateTime.Now;
            if (lastWateredEndDate <= nowDate)
            {
                return BadRequest();
            }

            plant.LastWateringEnd = string.Concat(nowDate.ToUniversalTime().ToString("s"), "Z");

            _context.Entry(plant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Plants
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Plant>>> PostPlant(Plant[] plants)
        {
            foreach (var plant in plants)
            {
                _context.plants.Add(plant);
            }

            await _context.SaveChangesAsync();

            return await _context.plants.ToListAsync();
        }

        private bool PlantExists(int id)
        {
            return _context.plants.Any(e => e.Id == id);
        }
    }
}