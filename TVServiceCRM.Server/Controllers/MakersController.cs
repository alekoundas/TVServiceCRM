using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TVServiceCRM.Server.Business;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Model.Dtos.DataTable;
using TVServiceCRM.Server.Model.Dtos;
using TVServiceCRM.Server.Model.Models;
using Microsoft.AspNetCore.Identity;
using TVServiceCRM.Server.Model.Dtos.Lookup;
using TVServiceCRM.Server.Business.Services;
using System.Data;
using System.Security.Claims;
using TVServiceCRM.Server.Model.Dtos.Identity;

namespace TVServiceCRM.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MakersController : GenericController<Maker, MakersController>
    {
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;
        private readonly ILogger<MakersController> _logger;

        public MakersController(IDataService dataService, ILogger<MakersController> logger, IMapper mapper) : base(dataService, logger)
        {
            _logger = logger;
            _dataService = dataService;
            _mapper = mapper;
        }

        // POST: api/Makers/GetDataTable
        [HttpPost("GetDataTable")]
        public async Task<ApiResponse<DataTableDto<MakerDto>>> GetDataTable([FromBody] DataTableDto<MakerDto> dataTable)
        {
            Func<IQueryable<Maker>, IOrderedQueryable<Maker>>? orderByQuery = null;
            List<Func<IOrderedQueryable<Maker>, IOrderedQueryable<Maker>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<Maker>, IOrderedQueryable<Maker>>>();
            List<Expression<Func<Maker, bool>>>? filterQuery = new List<Expression<Func<Maker, bool>>>();
            List<Func<IQueryable<Maker>, IIncludableQueryable<Maker, object>>>? includesQuery = new List<Func<IQueryable<Maker>, IIncludableQueryable<Maker, object>>>();


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
            if (dataTable.Filters?.Title?.Value != null && dataTable.Filters?.Title.Value.Length > 0)
                filterQuery.Add(x => x.Title.Contains(dataTable.Filters.Title.Value));



            // Retrieve Data.
            List<Maker> result = await _dataService.Makers.GetPaggingWithFilterAndSort(
                filterQuery,
                orderByQuery,
                thenOrderByQuery,
                includesQuery,
                dataTable.PageCount.Value,
                dataTable.Page.Value
            );
            List<MakerDto> MakerDto = _mapper.Map<List<MakerDto>>(result);

            //MakerDto.SelectMany(x => x.ContactInformations).ToList().ForEach(x => x.Maker = null);
            //MakerDto.SelectMany(x => x.Tickets).ToList().ForEach(x => x.Maker = null);

            int rows = await _dataService.Makers.CountAsyncFiltered(filterQuery);

            dataTable.Data = MakerDto;
            dataTable.PageCount = rows;

            return new ApiResponse<DataTableDto<MakerDto>>().SetSuccessResponse(dataTable);

        }

        // POST: api/Makers/Lookup
        [HttpPost("Lookup")]
        public async Task<ApiResponse<LookupDto>> Lookup([FromBody] LookupDto lookupDto)
        {
            Func<IQueryable<Maker>, IOrderedQueryable<Maker>>? orderByQuery = null;
            List<Expression<Func<Maker, bool>>>? filterQuery = new List<Expression<Func<Maker, bool>>>();
            List<Func<IOrderedQueryable<Maker>, IOrderedQueryable<Maker>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<Maker>, IOrderedQueryable<Maker>>>();
            List<Func<IQueryable<Maker>, IIncludableQueryable<Maker, object>>>? includesQuery = new List<Func<IQueryable<Maker>, IIncludableQueryable<Maker, object>>>();

            // Order.
            orderByQuery = x => x.OrderByColumnDescending("Title");

            // Filter.
            if (lookupDto?.Filter?.Value != null && lookupDto.Filter.Value.Length > 0)
                filterQuery.Add(x => x.Title.ToLower().StartsWith(lookupDto.Filter.Value.ToLower()));

            if (lookupDto?.Filter?.Id != null && lookupDto.Filter.Id.Length > 0)
            {
                bool success = int.TryParse(lookupDto.Filter.Id, out int id);
                if (success)
                    filterQuery.Add(x => x.Id == id);
            }

            // Retrieve data.
            List<LookupOptionDto> lookupOptions = await _dataService.Makers
                .GetLookup(filterQuery, orderByQuery, 0, 100)
                .Select(x => new LookupOptionDto() { Id = x.Id.ToString(), Value = x.Title })
                .ToListAsync();

            lookupDto.Data = lookupOptions;

            return new ApiResponse<LookupDto>().SetSuccessResponse(lookupDto);
        }

        // PUT: api/Makers/5
        [HttpPut("{id}")]
        public async Task<ApiResponse<Maker>> Update(int? id, [FromBody] MakerDto makerDto)
        {
            // Checks.
            if (id == null || id != makerDto.Id)
                return new ApiResponse<Maker>().SetErrorResponse("error", "Maker name not not set!");

            Maker maker = _mapper.Map<Maker>(makerDto);
            _dataService.Makers.Update(maker);
            int isSuccess = await _dataService.Makers.SaveChangesAsync();

            if (isSuccess == 0)
                return new ApiResponse<Maker>().SetErrorResponse("error", "Error occured while updating.");

            return new ApiResponse<Maker>().SetSuccessResponse(maker);
        }

        // POST: api/Makers
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ApiResponse<Maker>> Create(MakerDto makerDto)
        {
            Maker maker = _mapper.Map<Maker>(makerDto);
            maker.CreatedOn = DateTime.Now;

            _dataService.Makers.Add(maker);
            int isSuccess = await _dataService.SaveChangesAsync();
            if (isSuccess == 0)
                return new ApiResponse<Maker>().SetErrorResponse("error", "Maker Model was not created!");

            return new ApiResponse<Maker>().SetSuccessResponse(maker);
        }

    }
}
