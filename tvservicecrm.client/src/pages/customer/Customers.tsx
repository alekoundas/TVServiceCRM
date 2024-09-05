import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Card } from "primereact/card";
import { Column } from "primereact/column";
import { Button } from "primereact/button";
import {
  DataTable,
  DataTableFilterEvent,
  DataTablePageEvent,
  DataTableSortEvent,
} from "primereact/datatable";
import { ApiService } from "../../services/ApiService";
import { CustomerDto } from "../../model/CustomerDto";
import { DataTableDto } from "../../model/DataTableDto";
import { Calendar } from "primereact/calendar";

function Customers() {
  const navigate = useNavigate();
  const [UrlParameters, _setUrlParameters] = useSearchParams();

  var formCustomer = new CustomerDto();
  const [isInitialised, setIsInitialised] = useState(false);
  const [loading, setLoading] = useState(true);
  const [waitAsync, setWaitAsync] = useState(false);
  const [refreshDataState, setRefreshDataState] = useState(false);
  const [dataTableDtoState, setDataTableDtoState] = useState<
    DataTableDto<CustomerDto>
  >({
    data: [],
    first: 0,
    rows: 10,
    page: 1,
    pageCount: 0,
    multiSortMeta: [
      { field: "lastName", order: 1 },
      { field: "firstName", order: 1 },
      { field: "createdOn", order: -1 },
    ],
    filters: {
      firstName: { value: "", matchMode: "contains" },
      lastName: { value: "", matchMode: "contains" },
      createdOn: { value: "", matchMode: "contains" },
    },
  });

  // const paginatorLeft = (
  //   <Button
  //     type="button"
  //     icon="pi pi-refresh"
  //     text
  //   />
  // );

  useEffect(() => {
    setLoading(true);
    setWaitAsync(true);

    if (!isInitialised) {
      var searchUrlParameter = UrlParameters.get("search");
      if (searchUrlParameter) {
        var searchQuery = JSON.parse(atob(searchUrlParameter));
        setDataTableDtoState(searchQuery);
      }
      setIsInitialised(true);
    }

    const refrshData = async () => {
      // Encode datatable filters and ordering to base 64
      var searchQuery = btoa(JSON.stringify(dataTableDtoState));
      window.history.replaceState(
        null,
        "New Page Title",
        "/customers?search=" + searchQuery
      );

      var response = await ApiService.getDataGrid<CustomerDto>(
        "customers",
        dataTableDtoState
      );

      if (response) {
        setDataTableDtoState(response);
      }
      setLoading(false);
      setWaitAsync(false);
    };

    if (!waitAsync) {
      refrshData();
    }
  }, [refreshDataState]);

  const onPage = (event: DataTablePageEvent) => {
    if (event.page) dataTableDtoState.page = event.page;
    if (event.pageCount) dataTableDtoState.pageCount = event.pageCount;
    if (event.rows) dataTableDtoState.rows = event.rows;

    setDataTableDtoState(dataTableDtoState);
    setRefreshDataState(!refreshDataState);
  };

  const onSort = (event: DataTableSortEvent) => {
    if (event.multiSortMeta)
      dataTableDtoState.multiSortMeta = event.multiSortMeta;

    setDataTableDtoState(dataTableDtoState);
    setRefreshDataState(!refreshDataState);
  };

  const onFilter = (event: DataTableFilterEvent) => {
    dataTableDtoState.filters = event.filters;
    setDataTableDtoState(dataTableDtoState);
    setRefreshDataState(!refreshDataState);
  };

  const activityRowFilterTemplate = (options: any) => {
    return (
      <div className="flex gap-1">
        <Calendar
          value={options.value}
          onChange={(e) => options.filterApplyCallback(e.value)}
          selectionMode="range"
          // readOnlyInput
          className="w-full"
          hideOnRangeSelection
        />
      </div>
    );
  };

  const gridRowActions = (rowData: CustomerDto) => (
    <React.Fragment>
      <Button
        icon="pi pi-eye"
        rounded
        outlined
        onClick={() =>
          navigate("/customers/" + rowData.id + "/view", {
            state: { formCustomer: rowData },
          })
        }
      />
      <Button
        icon="pi pi-pencil"
        rounded
        outlined
        className="mr-2"
        severity="info"
        onClick={() =>
          navigate("/customers/" + rowData.id + "/edit", {
            state: { formCustomer: rowData },
          })
        }
      />
    </React.Fragment>
  );

  const renderHeader = () => {
    return (
      <div className="flex justify-content-between">
        <div></div>
        <Button
          type="button"
          icon="pi pi-plus"
          label="Add"
          outlined
          onClick={() =>
            navigate("/customers/add", { state: { formCustomer } })
          }
        />
      </div>
    );
  };

  return (
    <>
      <div className="w-full card ">
        <Card title="Customers">
          <DataTable
            value={dataTableDtoState.data}
            // dataKey="id"
            lazy
            stripedRows
            emptyMessage="No customers found."
            tableStyle={{ minWidth: "50rem" }}
            selectionMode="single"
            loading={loading}
            header={renderHeader}
            // Pagging.
            paginator
            rows={dataTableDtoState.rows}
            totalRecords={dataTableDtoState.pageCount}
            onPage={onPage}
            rowsPerPageOptions={[10, 25, 50, 100]}
            // paginatorLeft={paginatorLeft}
            currentPageReportTemplate={
              "1 to " +
              dataTableDtoState.rows +
              " out of " +
              dataTableDtoState.pageCount
            }
            paginatorTemplate="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink RowsPerPageDropdown"
            // Filter.
            filterDisplay="row"
            filters={dataTableDtoState.filters}
            onFilter={onFilter}
            // Sort.
            removableSort
            sortMode="multiple"
            onSort={onSort}
            multiSortMeta={dataTableDtoState.multiSortMeta}
          >
            <Column
              field="firstName"
              header="First Name"
              sortable
              filter
              filterPlaceholder="Search by First Name"
            ></Column>
            <Column
              field="lastName"
              header="Last Name"
              sortable
              filter
              filterPlaceholder="Search by Last Name"
            ></Column>
            <Column
              field="createdOn"
              header="Creation Date"
              filterElement={activityRowFilterTemplate}
              sortable
              filter
              filterPlaceholder="Search by Creation Day"
            ></Column>
            <Column
              header="Actions"
              headerStyle={{ width: "10%", minWidth: "8rem" }}
              body={gridRowActions}
            ></Column>
          </DataTable>
        </Card>
      </div>
    </>
  );
}

export default Customers;
