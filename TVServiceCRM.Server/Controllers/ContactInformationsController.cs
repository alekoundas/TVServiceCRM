using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Business.IServices;

namespace TVServiceCRM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInformationsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TicketsController> _logger;

        public ContactInformationsController(ILogger<TicketsController> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // GET: api/ContactInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactInformation>>> GetContactInformations()
        {
            return await _dataService.ContactInformations.GetPaggingWithFilterAndSort(null,null,null);
        }

        // GET: api/ContactInformations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactInformation>> GetContactInformation(int id)
        {
            var contactInformation = await _dataService.ContactInformations.FindByIdAsync(id);

            if (contactInformation == null)
                return NotFound();

            return contactInformation;
        }

        // PUT: api/ContactInformations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactInformation(int id, ContactInformation contactInformation)
        {
            if (id != contactInformation.Id)
                return BadRequest();

            _dataService.ContactInformations.Update(contactInformation);

            try
            {
                await _dataService.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactInformationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/ContactInformations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactInformation>> PostContactInformation(ContactInformation contactInformation)
        {
            _dataService.ContactInformations.Add(contactInformation);
            await _dataService.SaveChangesAsync();

            return CreatedAtAction("GetContactInformation", new { id = contactInformation.Id }, contactInformation);
        }

        // DELETE: api/ContactInformations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactInformation(int id)
        {
            var contactInformation = await _dataService.ContactInformations.FindByIdAsync(id);
            if (contactInformation == null)
                return NotFound();

            _dataService.ContactInformations.Remove(contactInformation);
            await _dataService.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactInformationExists(int id)
        {
            return _dataService.ContactInformations.Any(e => e.Id == id);
        }
    }
}
