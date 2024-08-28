import React, { useState } from "react";
import { Button } from "primereact/button";
import { Card } from "primereact/card";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { useLocation } from "react-router-dom";
import { CustomerDto } from "../../model/CustomerDto";
import { ContactInformationTypesEnum } from "../../enum/ContactInformationTypesEnum";
import { InputText } from "primereact/inputtext";
import ApiService from "../../services/ApiService";
import { Dialog } from "primereact/dialog";
import { InputTextarea } from "primereact/inputtextarea";
import { ContactInformationDto } from "../../model/ContactInformationDto";
import { Dropdown } from "primereact/dropdown";
import { InputMask } from "primereact/inputmask";
function CustomerForm() {
  //const navigate = useNavigate();
  const { state } = useLocation();
  const formCustomer: CustomerDto = state["formCustomer"];
  const formContactInformation = new ContactInformationDto();

  const [customer, setCustomer] = useState(formCustomer);
  const [contactInformation, setContactInformation] = useState(
    formContactInformation
  );
  const [elementsVisibility, setElementsVisibility] = useState({
    contactInformationVisible: false,
    contactInformationDeleteVisible: false,
    contactInformationPhoneVisible: true,
    contactInformationEmailVisible: false,
  });

  //
  // Handle inpuut changes.
  //
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    // const name: StatusKey  = event.target.name as string;
    const name = event.target.name;

    const value = event.target.value;
    customer[name] = value;
    setCustomer({ ...customer });
  };

  const handleContactInformationChange = (event: any) => {
    const value = event.target.value;
    const name = event.target.name;

    contactInformation[name] = value;
    if (name === "type" && value === ContactInformationTypesEnum.PHONE) {
      elementsVisibility.contactInformationEmailVisible = false;
      elementsVisibility.contactInformationPhoneVisible = true;
      setElementsVisibility({ ...elementsVisibility });
    }

    if (name === "type" && value === ContactInformationTypesEnum.EMAIL) {
      elementsVisibility.contactInformationEmailVisible = true;
      elementsVisibility.contactInformationPhoneVisible = false;
      setElementsVisibility({ ...elementsVisibility });
    }

    setContactInformation({ ...contactInformation });
  };

  //
  // Handle save.
  //
  const handleContactInformationSave = () => {
    //edit mode
    if (contactInformation.id > 0) {
      return;
    }

    if (customer.contactInformations.length >= 1) {
      const minContactInformation = customer.contactInformations.reduce(
        (x, y) => (x.id < y.id ? x : y)
      );

      if (minContactInformation.id < 0)
        contactInformation.id = minContactInformation.id - 1;
      else {
        contactInformation.id = -1;
      }
    } else {
      contactInformation.id = -1;
    }

    customer.contactInformations.push(contactInformation);
    setCustomer({ ...customer });
    setElementsVisibility({
      ...elementsVisibility,
      ["contactInformationVisible"]: false,
    });
  };
  const handleSubmit = (event: any) => {
    event.preventDefault();

    ApiService.create("customers", customer);
  };

  //
  // Handle grid row actions.
  //

  const handleEditContactInformation = (rowData: ContactInformationDto) => {
    setContactInformation({ ...rowData });
    setElementsVisibility({
      ...elementsVisibility,
      ["contactInformationVisible"]: true,
    });
  };

  const handleDeleteContactInformation = (rowData: ContactInformationDto) => {
    // // Find customer index and delete him.
    // const index = customer.contactInformations.indexOf(rowData, 0);
    // if (index > -1) {
    //   customer.contactInformations.splice(index, 1);
    // }
    // setCustomer({ ...customer });
    setContactInformation({ ...rowData });
    setElementsVisibility({
      ...elementsVisibility,
      ["contactInformationDeleteVisible"]: true,
    });
  };

  const openAddContactInformation = () => {
    setContactInformation(new ContactInformationDto());
    elementsVisibility.contactInformationVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const renderHeader = () => {
    return (
      <div className="flex justify-content-between">
        <div></div>
        <Button
          type="button"
          icon="pi pi-plus"
          label="Add"
          outlined
          onClick={openAddContactInformation}
        />
      </div>
    );
  };
  const onHideContactInformation = () => {
    elementsVisibility.contactInformationVisible = false;
    elementsVisibility.contactInformationDeleteVisible = false;

    setElementsVisibility({ ...elementsVisibility });
  };

  const dialogFooter = (
    <React.Fragment>
      <Button
        label="Cancel"
        icon="pi pi-times"
        outlined
        onClick={onHideContactInformation}
      />
      <Button
        label="Save"
        icon="pi pi-check"
        onClick={handleContactInformationSave}
      />
    </React.Fragment>
  );

  const contactInformationGridRowActions = (rowData: ContactInformationDto) => (
    <React.Fragment>
      <Button
        icon="pi pi-pencil"
        rounded
        outlined
        className="mr-2"
        onClick={() => handleEditContactInformation(rowData)}
      />
      <Button
        icon="pi pi-trash"
        rounded
        outlined
        severity="danger"
        onClick={() => handleDeleteContactInformation(rowData)}
      />
    </React.Fragment>
  );
  return (
    <>
      <div className="">
        <Card title="Customer ">
          <form onSubmit={handleSubmit}>
            <div className="flex align-items-center justify-content-center">
              <div className="card flex flex-column md:flex-row gap-3">
                <div className="p-inputgroup flex-1">
                  <span className="p-float-label">
                    <InputText
                      id="firstName"
                      name="firstName"
                      value={customer.firstName}
                      onChange={handleChange}
                    />
                    <label htmlFor="firstName">First Name</label>
                  </span>
                </div>

                <div className="p-inputgroup flex-1">
                  <span className="p-float-label">
                    <InputText
                      id="lastName"
                      name="lastName"
                      value={customer.lastName}
                      onChange={handleChange}
                    />
                    <label htmlFor="lastName">Last Name</label>
                  </span>
                </div>
              </div>
            </div>

            {/* <div className="flex align-items-center justify-content-center">
              <div className="card flex flex-column md:flex-row gap-3">
                <div className="p-inputgroup flex-1">
                  <span className="p-inputgroup-addon">
                    <i className="pi pi-user"></i>
                  </span>
                  <InputText
                    placeholder="First Name"
                    value={customer.FirstName}
                  />
                </div>

                <div className="p-inputgroup flex-1">
                  <span className="p-inputgroup-addon">$</span>
                  <InputNumber placeholder="Price" />
                  <span className="p-inputgroup-addon">.00</span>
                </div>
              </div>
            </div> */}

            <input type="submit" />
          </form>
        </Card>

        <div className="grid mt-2 gap-3 ">
          <Card
            title="Tickets "
            className="col-12 md:col "
          ></Card>
          <Card
            title="Contact Information "
            className="col-12 md:col "
          >
            <DataTable
              value={customer.contactInformations}
              editMode="row"
              dataKey="id"
              className="w-full"
              scrollable
              tableStyle={{ minWidth: "50rem" }}
              header={renderHeader}
            >
              <Column
                field="type"
                header="Type"
                style={{ width: "20%" }}
              ></Column>
              <Column
                field="value"
                header="Value"
                style={{ width: "40%" }}
              ></Column>
              <Column
                field="description"
                header="Description"
                style={{ width: "5%" }}
              ></Column>
              <Column
                headerStyle={{ width: "10%", minWidth: "8rem" }}
                body={contactInformationGridRowActions}
              ></Column>
            </DataTable>
          </Card>
        </div>
      </div>

      <Dialog
        visible={elementsVisibility.contactInformationVisible}
        style={{ width: "35%" }}
        header="Add Contact Information"
        modal
        className="p-fluid"
        footer={dialogFooter}
        onHide={onHideContactInformation}
      >
        <div className="field">
          <label htmlFor="type">Type</label>
          <Dropdown
            id="type"
            name="type"
            value={contactInformation.type}
            onChange={handleContactInformationChange}
            options={Object.keys(ContactInformationTypesEnum)}
            optionLabel="type"
            placeholder="Select Type"
            className="w-full md:w-14rem"
            checkmark={true}
            highlightOnSelect={true}
          />
        </div>

        <div className="field">
          <label htmlFor="value">Value</label>
          {elementsVisibility.contactInformationPhoneVisible && (
            <InputMask
              id="value"
              name="value"
              value={contactInformation.value}
              onChange={handleContactInformationChange}
              mask="999 9999 999"
              placeholder="999 9999 999"
              required
              autoFocus
              // className={classNames({ "p-invalid": submitted && !product.name })}
            />
          )}
          {elementsVisibility.contactInformationEmailVisible && (
            <InputText
              id="value"
              name="value"
              value={contactInformation.value}
              onChange={handleContactInformationChange}
              // placeholder={inputMaskSettings.placeholder}
              required
              autoFocus
              // className={classNames({ "p-invalid": submitted && !product.name })}
            />
          )}
          {/* {submitted && !product.name && (
            <small className="p-error">Name is required.</small>
          )} */}
        </div>
        <div className="field">
          <label htmlFor="description">Description</label>
          <InputTextarea
            id="description"
            name="description"
            value={contactInformation.description}
            onChange={handleContactInformationChange}
            required
            rows={3}
            cols={20}
          />
        </div>
      </Dialog>
      <Dialog
        visible={elementsVisibility.contactInformationDeleteVisible}
        style={{ width: "35%" }}
        header="Confirm"
        modal
        className="p-fluid"
        footer={dialogFooter}
        onHide={onHideContactInformation}
      >
        <div className="confirmation-content">
          <i
            className="pi pi-exclamation-triangle mr-3"
            style={{ fontSize: "2rem" }}
          />
          <span>Are you sure you want to delete ?</span>
        </div>
      </Dialog>
    </>
  );
}

export default CustomerForm;
