import {
  DataTableFilterEvent,
  DataTablePageEvent,
  DataTableSortEvent,
} from "primereact/datatable";
import { DataTableDto } from "../model/DataTableDto";
import ApiService from "./ApiService";

export class DataTableService<TEntity> {
  private dataTableDto: DataTableDto<TEntity>;
  private setDataTableDto: any;
  private setLoading: any;
  private controller: string;
  private defaultUrlSearchQuery: string | null = null;

  public constructor(
    controller: string,
    dataTableDto: DataTableDto<TEntity>,
    setDataTableDto: any,
    setLoading: any,
    defaultUrlSearchQuery: string | null
  ) {
    this.controller = controller;
    this.dataTableDto = dataTableDto;
    this.setDataTableDto = setDataTableDto;
    this.setLoading = setLoading;
    this.defaultUrlSearchQuery = defaultUrlSearchQuery;
  }

  public loadData = (urlQuery: string | null) => {
    if (urlQuery)
      try {
        // Decode datatable filters and ordering from base 64
        const dtaTableQuery: DataTableDto<TEntity> = JSON.parse(atob(urlQuery));
        this.dataTableDto = dtaTableQuery;
        this.setDataTableDto({ ...dtaTableQuery });
      } catch (e: any) {
        console.log(e.message);
      }
    this.refreshData();
  };

  public onSort = (event: DataTableSortEvent) => {
    if (event.multiSortMeta)
      this.dataTableDto.multiSortMeta = event.multiSortMeta;

    this.setDataTableDto({ ...this.dataTableDto });
    this.refreshData();
  };

  public onFilter = (event: DataTableFilterEvent) => {
    this.dataTableDto.filters = event.filters;
    this.setDataTableDto({ ...this.dataTableDto });
    this.refreshData();
  };

  public onPage = (event: DataTablePageEvent) => {
    if (event.page) this.dataTableDto.page = event.page;
    if (event.rows) this.dataTableDto.rows = event.rows;
    if (event.pageCount) this.dataTableDto.pageCount = event.pageCount;

    this.setDataTableDto({ ...this.dataTableDto });
    this.refreshData();
  };

  private refreshData = async (): Promise<DataTableDto<TEntity> | null> => {
    this.setLoading(true);
    return await ApiService.getDataGrid<TEntity>(
      this.controller,
      this.dataTableDto
    ).then((response) => {
      if (response) this.setDataTableDto({ ...response });
      if (this.defaultUrlSearchQuery) this.setUrlSearchQuery(response);
      this.setLoading(false);
      return response;
    });
  };

  private setUrlSearchQuery = (response: DataTableDto<TEntity> | null) => {
    if (response) {
      response.data = [];

      // Encode datatable filters and ordering to base 64
      var searchQuery = btoa(JSON.stringify(response));

      if (searchQuery !== this.defaultUrlSearchQuery)
        window.history.replaceState(
          null,
          "New Page Title",
          `/${this.controller}?search=${searchQuery}`
        );
    }
  };
}
export default DataTableService;
