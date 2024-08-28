import { DataTableFilterMeta, DataTableSortMeta } from "primereact/datatable";

export class DataTableDto<TEntity> {
  data: TEntity[];
  first: number;
  rows: number;
  page: number;
  pageCount: number;
  multiSortMeta: DataTableSortMeta[];
  filters: DataTableFilterMeta;
}
