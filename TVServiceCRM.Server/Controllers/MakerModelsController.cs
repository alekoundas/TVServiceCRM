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
    public class MakerModelsController : GenericController<MakerModel, MakerModelsController>
    {
        private readonly IDataService _dataService;
        private readonly IMapper _mapper;
        private readonly ILogger<MakerModelsController> _logger;

        public MakerModelsController(IDataService dataService, ILogger<MakerModelsController> logger, IMapper mapper) : base(dataService, logger)
        {
            _logger = logger;
            _dataService = dataService;
            _mapper = mapper;
        }

        // POST: api/MakerModels/GetDataTable
        [HttpPost("GetDataTable")]
        public async Task<ApiResponse<DataTableDto<MakerModelDto>>> GetDataTable([FromBody] DataTableDto<MakerModelDto> dataTable)
        {
            Func<IQueryable<MakerModel>, IOrderedQueryable<MakerModel>>? orderByQuery = null;
            List<Func<IOrderedQueryable<MakerModel>, IOrderedQueryable<MakerModel>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<MakerModel>, IOrderedQueryable<MakerModel>>>();
            List<Expression<Func<MakerModel, bool>>>? filterQuery = new List<Expression<Func<MakerModel, bool>>>();
            List<Func<IQueryable<MakerModel>, IIncludableQueryable<MakerModel, object>>>? includesQuery = new List<Func<IQueryable<MakerModel>, IIncludableQueryable<MakerModel, object>>>();


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
            List<MakerModel> result = await _dataService.MakerModels.GetPaggingWithFilterAndSort(
                filterQuery,
                orderByQuery,
                thenOrderByQuery,
                includesQuery,
                dataTable.PageCount.Value,
                dataTable.Page.Value
            );
            List<MakerModelDto> MakerModelDto = _mapper.Map<List<MakerModelDto>>(result);

            //MakerModelDto.SelectMany(x => x.ContactInformations).ToList().ForEach(x => x.MakerModel = null);
            //MakerModelDto.SelectMany(x => x.Tickets).ToList().ForEach(x => x.MakerModel = null);

            int rows = await _dataService.MakerModels.CountAsyncFiltered(filterQuery);

            dataTable.Data = MakerModelDto;
            dataTable.PageCount = rows;

            return new ApiResponse<DataTableDto<MakerModelDto>>().SetSuccessResponse(dataTable);

        }

        // POST: api/MakerModels/Lookup
        [HttpPost("Lookup")]
        public async Task<ApiResponse<LookupDto>> Lookup([FromBody] LookupDto lookupDto)
        {
            Func<IQueryable<MakerModel>, IOrderedQueryable<MakerModel>>? orderByQuery = null;
            List<Expression<Func<MakerModel, bool>>>? filterQuery = new List<Expression<Func<MakerModel, bool>>>();
            List<Func<IOrderedQueryable<MakerModel>, IOrderedQueryable<MakerModel>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<MakerModel>, IOrderedQueryable<MakerModel>>>();
            List<Func<IQueryable<MakerModel>, IIncludableQueryable<MakerModel, object>>>? includesQuery = new List<Func<IQueryable<MakerModel>, IIncludableQueryable<MakerModel, object>>>();

            // Order.
            orderByQuery = x => x.OrderByColumnDescending("Title");

            // Filter.
            if (lookupDto?.Filter?.Value != null && lookupDto.Filter.Value.Length > 0)
                filterQuery.Add(x => x.Title.Contains(lookupDto.Filter.Value));

            if (lookupDto?.Filter?.Id != null && lookupDto.Filter.Id.Length > 0)
            {
                bool success = int.TryParse(lookupDto.Filter.Id, out int id);
                if (success)
                    filterQuery.Add(x => x.Id == id);
            }

            // Retrieve data.
            List<LookupOptionDto> lookupOptions = await _dataService.MakerModels
                .GetLookup(filterQuery, orderByQuery, 0, 100)
                .Select(x => new LookupOptionDto() { Id = x.Id.ToString(), Value = x.Title })
                .ToListAsync();

            lookupDto.Data = lookupOptions;

            return new ApiResponse<LookupDto>().SetSuccessResponse(lookupDto);
        }

        // PUT: api/MakerModels/5
        [HttpPut("{id}")]
        public async Task<ApiResponse<MakerModel>> Update(int? id, [FromBody] MakerModelDto MakerModelDto)
        {
            // Checks.
            if (id == null || id != MakerModelDto.Id)
                return new ApiResponse<MakerModel>().SetErrorResponse("error", "MakerModel name not not set!");

            MakerModel MakerModel = _mapper.Map<MakerModel>(MakerModelDto);
            _dataService.MakerModels.Update(MakerModel);

            return new ApiResponse<MakerModel>().SetSuccessResponse(MakerModel);
        }

        // POST: api/MakerModels
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ApiResponse<MakerModel>> Create(MakerModelDto makerModelDto)
        {
            MakerModel makerModel = _mapper.Map<MakerModel>(makerModelDto);
            makerModel.CreatedOn = DateTime.Now;

            _dataService.MakerModels.Add(makerModel);
            int isSuccess = await _dataService.SaveChangesAsync();
            if(isSuccess==0)
                return new ApiResponse<MakerModel>().SetErrorResponse("error","Maker Model was not created!");

            return new ApiResponse<MakerModel>().SetSuccessResponse(makerModel);
        }

    }
}
