export interface DataTableColumns {
  field: string;
  header: string;
  sortable: boolean;
  filter: boolean;
  filterPlaceholder: string;
  style: React.CSSProperties;
  body: any | null;
}
