import DataTableComponent from "../../components/datatable/DataTableComponent";
import DeleteDialogComponent from "../../components/dialog/DeleteDialogComponent";
import EditDialogComponent from "../../components/dialog/EditDialogComponent";
import ViewDialogComponent from "../../components/dialog/ViewDialogComponent";
import { ButtonTypeEnum } from "../../enum/ButtonTypeEnum";
import { FormMode } from "../../enum/FormMode";
import { DataTableColumns } from "../../model/datatable/DataTableColumns";
import { DataTableDto } from "../../model/DataTableDto";
import { IdentityRoleDto } from "../../model/IdentityRoleDto";
import RoleForm from "./RoleForm";

export default function Roles() {
  let setDeleteDialogVisibility: (newValue: boolean) => void = () => {};
  let setEditDialogVisibility: (newValue: boolean) => void = () => {};
  let setViewDialogVisibility: (newValue: boolean) => void = () => {};

  const formMode: FormMode = window.location.href.endsWith("/add")
    ? FormMode.ADD
    : window.location.href.endsWith("/edit")
    ? FormMode.EDIT
    : FormMode.VIEW;

  const datatableDto: DataTableDto<IdentityRoleDto> = {
    data: [],
    first: 0,
    rows: 10,
    page: 1,
    pageCount: 0,
    multiSortMeta: [],
    filters: {
      value: { value: "", matchMode: "contains" },
      description: { value: "", matchMode: "contains" },
      customerId: { value: "", matchMode: "contains" },
    },
  };

  const dataTableColumns: DataTableColumns[] = [
    {
      field: "name",
      header: "Name",
      sortable: formMode !== FormMode.ADD,
      filter: formMode !== FormMode.ADD,
      filterPlaceholder: "Search",
      style: { width: "30%" },
      body: null,
    },
    {
      field: "normalizedName",
      header: "Normalized Name",
      sortable: formMode !== FormMode.ADD,
      filter: formMode !== FormMode.ADD,
      filterPlaceholder: "Search",
      style: { width: "30%" },
      body: null,
    },
  ];

  const onDataTableClick = (
    buttonType: ButtonTypeEnum,
    rowData?: IdentityRoleDto
  ) => {
    switch (buttonType) {
      case ButtonTypeEnum.EDIT:
        setEditDialogVisibility(true);
        break;
      case ButtonTypeEnum.DELETE:
        setDeleteDialogVisibility(true);
        break;
      case ButtonTypeEnum.VIEW:
        setViewDialogVisibility(true);
        break;

      default:
        break;
    }

    console.log(rowData);
    console.log(buttonType);
  };

  const onDialogClick = (
    buttonType: ButtonTypeEnum,
    rowData?: IdentityRoleDto
  ) => {
    console.log(rowData);
    console.log(buttonType);
    setDeleteDialogVisibility(false);
  };

  return (
    <>
      <div className="card">
        <DataTableComponent
          onButtonClick={onDataTableClick}
          controller="roles"
          dataTable={datatableDto}
          dataTableColumns={dataTableColumns}
        />
      </div>

      <DeleteDialogComponent
        onParentVisibilityUpdate={(fn) => (setDeleteDialogVisibility = fn)}
        onButtonClick={onDialogClick}
      />
      <ViewDialogComponent
        onParentVisibilityUpdate={(fn) => (setViewDialogVisibility = fn)}
      >
        <RoleForm />
      </ViewDialogComponent>
      <EditDialogComponent
        onButtonClick={onDialogClick}
        onParentVisibilityUpdate={(fn) => (setEditDialogVisibility = fn)}
      >
        <RoleForm />
      </EditDialogComponent>
    </>
  );
}
