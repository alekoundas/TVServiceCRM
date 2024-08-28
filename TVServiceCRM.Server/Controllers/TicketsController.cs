﻿using Microsoft.AspNetCore.Mvc;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Business.IServices;
using System.Linq.Expressions;
using TVServiceCRM.Server.Business;
using TVServiceCRM.Server.Business.Services;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Model.Dtos.DataTable;

namespace TVServiceCRM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ILogger<TicketsController> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // GET: api/Tickets/GetDataTable
        [HttpPost("GetDataTable")]
        public async Task<DataTableDto<Ticket>> GetDataTable([FromBody] DataTableDto<Ticket> dataTable)
        {
            Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>? orderByQuery = null;
            List<Func<IOrderedQueryable<Ticket>, IOrderedQueryable<Ticket>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<Ticket>, IOrderedQueryable<Ticket>>>();
            List<Expression<Func<Ticket, bool>>>? filterQuery = new List<Expression<Func<Ticket, bool>>>();

            // Handle Sorting of DataTable.
            if (dataTable.MultiSortMeta?.Count() > 0)
            {
                // Create the first OrderBy().
                DataTableSortDto? dataTableSort = dataTable.MultiSortMeta.First();
                if (dataTableSort.Order > 0)
                    orderByQuery = x => x.OrderByColumn(dataTableSort.Field);
                else if (dataTableSort.Order < 0)
                    orderByQuery = x => x.OrderByColumnDescending(dataTableSort.Field);

                // Create the rest OrderBy methods as ThenBy() if any.
                foreach (var sortInfo in dataTable.MultiSortMeta.Skip(1))
                {
                    if (dataTableSort.Order > 0)
                        thenOrderByQuery.Add(x => x.ThenByColumn(sortInfo.Field));
                    else if (dataTableSort.Order < 0)
                        thenOrderByQuery.Add(x => x.ThenByColumnDescending(sortInfo.Field));
                }
            }


            // Handle Filtering of DataTable.
            //if (dataTable.Filters?.FirstName?.Value != null && dataTable.Filters?.FirstName.Value.Length > 0)
            //    filterQuery.Add(x => x.FirstName.Contains(dataTable.Filters.FirstName.Value));

            //if (dataTable.Filters?.LastName?.Value != null && dataTable.Filters?.LastName.Value.Length > 0)
            //    filterQuery.Add(x => x.LastName.Contains(dataTable.Filters.LastName.Value));


            // Retrieve Data.
            List<Ticket> data = await _dataService.Tickets.GetPaggingWithFilterAndSort(
                filterQuery,
                orderByQuery,
                thenOrderByQuery,
                null,
                dataTable.PageCount.Value,
                dataTable.Page.Value
            );
            int rows = await _dataService.Tickets.CountAsyncFiltered(filterQuery);

            dataTable.Data = data;
            dataTable.PageCount = rows;
            return dataTable;

        }


        // GET: api/Tickets
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        public string gettickets()
        {
            //List<Ticket> result = await _dataService.Tickets.GetPaggingWithFilterAndSort(null, null, null);
            return Directory.GetCurrentDirectory() + "/VolumeDB/TVServiceCRM.db";
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _dataService.Tickets.FindByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id != ticket.Id)
                return BadRequest();

            _dataService.Tickets.Update(ticket);

            try
            {
                await _dataService.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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

        // POST: api/Tickets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            _dataService.Tickets.Add(ticket);
            await _dataService.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _dataService.Tickets.FindByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _dataService.Tickets.Remove(ticket);
            await _dataService.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _dataService.Tickets.Any(e => e.Id == id);
        }
    }
}
