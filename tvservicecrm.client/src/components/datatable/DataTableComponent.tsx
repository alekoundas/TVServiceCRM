import { Column, ColumnBodyOptions } from "primereact/column";
import { DataTable, DataTableValue } from "primereact/datatable";
import { DataTableDto } from "../../model/DataTableDto";
import { DataTableColumns } from "../../model/datatable/DataTableColumns";
import React, { useEffect, useState } from "react";
import DataTableService from "../../services/DataTableService";
import { Button } from "primereact/button";
import { ButtonTypeEnum } from "../../enum/ButtonTypeEnum";

interface IField<TEntity> {
  controller: string;
  dataTable: DataTableDto<TEntity>;
  dataTableColumns: DataTableColumns[];
  onButtonClick: (buttonType: ButtonTypeEnum, rowData?: TEntity) => void;
}

export default function DataTableComponent<TEntity extends DataTableValue>({
  controller,
  dataTable,
  dataTableColumns,
  onButtonClick,
}: IField<TEntity>) {
  const [loading, setLoading] = useState(true);
  const [dataTableDto, setDataTableDto] =
    useState<DataTableDto<TEntity>>(dataTable);

  const dataTableService = new DataTableService(
    controller,
    dataTableDto,
    setDataTableDto,
    setLoading,
    null
  );

  const getDataTableColumns = () => {
    const canAddAction =
      dataTableColumns.filter((x) => x.header === "Actions").length === 0;

    if (canAddAction)
      dataTableColumns.push({
        field: "",
        header: "Actions",
        sortable: false,
        filter: false,
        filterPlaceholder: "",
        style: { width: "20%" },
        body: gridRowActions,
      });
    return dataTableColumns;
  };

  const gridRowActions = (rowData: TEntity, _options: ColumnBodyOptions) => (
    <React.Fragment>
      <Button
        icon="pi pi-eye"
        rounded
        outlined
        className="mr-2"
        severity="secondary"
        onClick={() => onButtonClick(ButtonTypeEnum.VIEW, rowData)}
      />
      <Button
        icon="pi pi-pencil"
        rounded
        outlined
        className="mr-2"
        onClick={() => onButtonClick(ButtonTypeEnum.EDIT, rowData)}
      />
      <Button
        icon="pi pi-trash"
        rounded
        outlined
        severity="danger"
        onClick={() => onButtonClick(ButtonTypeEnum.DELETE, rowData)}
      />
    </React.Fragment>
  );

  useEffect(() => {
    dataTableService.loadData(null);
    console.log("loaded");
  }, [location]);

  const renderHeader = () => {
    return (
      <div className="flex justify-content-between">
        <div></div>
        <Button
          type="button"
          icon="pi pi-plus"
          label="Add"
          outlined
          onClick={() => onButtonClick(ButtonTypeEnum.ADD)}
        />
      </div>
    );
  };

  return (
    <>
      <DataTable
        className="w-full"
        value={dataTableDto.data}
        lazy
        stripedRows
        emptyMessage="No data found."
        tableStyle={{ minWidth: "50rem" }}
        selectionMode="single"
        loading={loading}
        // Pagging.
        paginator
        rows={dataTableDto.rows}
        totalRecords={dataTableDto.pageCount}
        onPage={dataTableService.onPage}
        rowsPerPageOptions={[10, 25, 50, 100]}
        // paginatorLeft={paginatorLeft}
        currentPageReportTemplate={
          "1 to " + dataTableDto.rows + " out of " + dataTableDto.pageCount
        }
        paginatorTemplate="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink RowsPerPageDropdown"
        // Filter.
        filterDisplay="row"
        filters={dataTableDto.filters}
        onFilter={dataTableService.onFilter}
        // Sort.
        removableSort
        sortMode="multiple"
        onSort={dataTableService.onSort}
        multiSortMeta={dataTableDto.multiSortMeta}
        header={renderHeader()}
      >
        {getDataTableColumns().map((col, _i) => (
          <Column
            key={col.field}
            field={col.field}
            header={col.header}
            sortable={col.sortable}
            filter={col.filter}
            filterPlaceholder={col.filterPlaceholder}
            style={col.style}
            body={col.body}
            showFilterMenu={false}
          ></Column>
        ))}
      </DataTable>
    </>
  );
}
