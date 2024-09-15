import { useRef, useState } from "react";
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
import { DataTableFilterDisplayEnum } from "../../enum/DataTableFilterDisplayEnum";
import AddDialogComponent from "../../components/dialog/AddDialogComponent";

export default function Roles() {
  const [roleName, setRoleName] = useState("");
  const [roleId, setRoleId] = useState("");

  let setDeleteDialogVisibility: (newValue: boolean) => void = () => {};
  let setEditDialogVisibility: (newValue: boolean) => void = () => {};
  let setViewDialogVisibility: (newValue: boolean) => void = () => {};
  let setAddDialogVisibility: (newValue: boolean) => void = () => {};
  let setSaveDisable: (newValue: boolean) => void = () => {};
  let triggerFormSave: () => void = () => {};
  const onRefreshDataTable = useRef<(() => void) | undefined>(undefined);

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
    setRoleName(rowData?.name ?? "");
    setRoleId(rowData?.id ?? "");
    switch (buttonType) {
      case ButtonTypeEnum.VIEW:
        setViewDialogVisibility(true);
        break;
      case ButtonTypeEnum.ADD:
        setAddDialogVisibility(true);
        break;
      case ButtonTypeEnum.EDIT:
        setEditDialogVisibility(true);
        break;
      case ButtonTypeEnum.DELETE:
        setDeleteDialogVisibility(true);
        break;
      case ButtonTypeEnum.SAVE:
        triggerFormSave();
        setAddDialogVisibility(false);
        setEditDialogVisibility(false);
        if (onRefreshDataTable.current) onRefreshDataTable.current();
        break;

      default:
        break;
    }

    console.log(rowData);
    console.log(buttonType);
  };

  const afterSave = () => {
    if (onRefreshDataTable.current) onRefreshDataTable.current();
    setDeleteDialogVisibility(false);
  };

  // Link onSave event button to form Save function.
  const [triger, settriger] = useState(0);
  let triggerSave = () => {
    settriger(triger + 1);
    triggerFormSave();
    setAddDialogVisibility(false);
    setEditDialogVisibility(false);
  };

  return (
    <>
      <div className="card">
        <DataTableComponent
          onButtonClick={onDataTableClick}
          controller="roles"
          enableGridRowActions={true}
          filterDisplay={DataTableFilterDisplayEnum.ROW}
          enableAddAction={true}
          dataTable={datatableDto}
          dataTableColumns={dataTableColumns}
          triggerRefreshData={onRefreshDataTable}
        />
      </div>

      {/* Delete Modal */}
      <DeleteDialogComponent
        onAfterRowDeletion={afterSave}
        triggerDialogVisibility={(fn) => (setDeleteDialogVisibility = fn)}
        id={roleId}
        name={roleName}
      />

      {/* View Modal */}
      <ViewDialogComponent
        triggerDialogVisibility={(fn) => (setViewDialogVisibility = fn)}
      >
        <RoleForm
          id={roleId}
          formMode={FormMode.VIEW}
          roleName={roleName}
          onAfterSave={afterSave}
        />
      </ViewDialogComponent>

      {/* Edit Modal */}
      <EditDialogComponent
        onSaveButtonClick={triggerSave}
        triggerDialogVisibility={(fn) => (setEditDialogVisibility = fn)}
        triggerSaveDisable={(fn) => (setSaveDisable = fn)}
        triggerSaveEnable={(fn) => (setSaveDisable = fn)}
      >
        <RoleForm
          id={roleId}
          formMode={FormMode.EDIT}
          roleName={roleName}
          onAfterSave={afterSave}
          onDisableSaveButton={() => setSaveDisable(false)}
          onEnableSaveButton={() => setSaveDisable(true)}
          triggerSaveForm={triggerFormSave}
        />
      </EditDialogComponent>

      {/* Add Modal */}
      <AddDialogComponent
        onSaveButtonClick={triggerSave}
        triggerDialogVisibility={(fn) => (setAddDialogVisibility = fn)}
        triggerSaveDisable={(fn) => (setSaveDisable = fn)}
        triggerSaveEnable={(fn) => (setSaveDisable = fn)}
      >
        <RoleForm
          id={roleId}
          formMode={FormMode.ADD}
          roleName={roleName}
          onAfterSave={afterSave}
          onDisableSaveButton={() => setSaveDisable(false)}
          onEnableSaveButton={() => setSaveDisable(true)}
          triggerSaveForm={triggerFormSave}
        />
      </AddDialogComponent>
    </>
  );
}
