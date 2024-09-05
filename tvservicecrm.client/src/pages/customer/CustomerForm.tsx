import React, { useEffect, useState } from "react";
import { Button } from "primereact/button";
import { Card } from "primereact/card";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { useNavigate, useParams } from "react-router-dom";
import { CustomerDto } from "../../model/CustomerDto";
import { ContactInformationTypesEnum } from "../../enum/ContactInformationTypesEnum";
import { InputText } from "primereact/inputtext";
import ApiService from "../../services/ApiService";
import { Dialog } from "primereact/dialog";
import { InputTextarea } from "primereact/inputtextarea";
import { ContactInformationDto } from "../../model/ContactInformationDto";
import { Dropdown, DropdownChangeEvent } from "primereact/dropdown";
import { InputMask } from "primereact/inputmask";
import { TicketDto } from "../../model/TicketDto";
import { InputNumber } from "primereact/inputnumber";
import { Editor, EditorTextChangeEvent } from "primereact/editor";
import { TicketTypesEnum } from "../../enum/TicketTypesEnum";
import { TicketStatusEnum } from "../../enum/TicketStatusEnum";

enum FormType {
  Customer,
  Tickets,
  ContactInformation,
}

function CustomerForm() {
  const navigate = useNavigate();
  const params = useParams();
  const formCustomer: CustomerDto = new CustomerDto();
  const formContactInformation = new ContactInformationDto();
  const formTicket = new TicketDto();

  const [customer, setCustomer] = useState(formCustomer);
  const [ticket, setTicket] = useState(formTicket);
  const [contactInformation, setContactInformation] = useState(
    formContactInformation
  );
  const [elementsVisibility, setElementsVisibility] = useState({
    contactInformationVisible: false,
    contactInformationDeleteVisible: false,
    contactInformationPhoneVisible: true,
    contactInformationEmailVisible: false,
    ticketVisible: false,
  });

  //
  //  Load initial data
  //
  useEffect(() => {
    if (params["id"]) {
      const id = params["id"];
      ApiService.get<CustomerDto>("customers", id).then((result) => {
        if (result) setCustomer(result);
      });
    }
    console.log("loaded");
  }, []);

  //
  // Handle input changes.
  //
  const handleChange = (
    event: React.ChangeEvent<any> | DropdownChangeEvent,
    formType: FormType
  ) => {
    const name = event.target.name;
    const value = event.target.value;

    if (formType === FormType.Customer) {
      customer[name] = value;
      setCustomer({ ...customer });
    } else if (formType === FormType.ContactInformation) {
      contactInformation[name] = value;
      setContactInformation({ ...contactInformation });
    } else if (formType === FormType.Tickets) {
      ticket[name] = value;
      setTicket({ ...ticket });
    }
  };

  const handleEditorChange = (event: EditorTextChangeEvent) => {
    ticket.descriptionHTML = event.htmlValue ?? "";
    ticket.description = event.textValue ?? "";
    setTicket({ ...ticket });
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
    // Edit mode.
    if (contactInformation.id > 0) {
      return;
    }

    // Add mode.
    else if (contactInformation.id == 0) {
      if (customer.contactInformations.length >= 1) {
        const minContactInformation = customer.contactInformations.reduce(
          (x, y) => (x.id < y.id ? x : y)
        );

        if (minContactInformation.id < 0)
          contactInformation.id = minContactInformation.id - 1;
      } else {
        contactInformation.id = -1;
      }

      customer.contactInformations.push(contactInformation);
      setCustomer({ ...customer });
    }

    // Add mode edit from grid row action.
    else if (contactInformation.id < 0) {
      const index = customer.contactInformations.findIndex(
        (x) => x.id === contactInformation.id
      );
      customer.contactInformations[index] = contactInformation;
    }

    elementsVisibility.contactInformationVisible = false;
    setElementsVisibility({ ...elementsVisibility });
  };

  const handleTicketSave = () => {
    // Edit mode.
    if (ticket.id > 0) {
      return;
    }

    // Add mode.
    else if (ticket.id == 0) {
      // Get min id and set as minId-1
      if (customer.tickets.length >= 1) {
        const minId = customer.tickets
          .map((x) => x.id)
          .reduce((x, y) => (x < y ? x : y));

        if (minId < 0) ticket.id = minId - 1;
      } else {
        ticket.id = -1;
      }

      customer.tickets.push(ticket);
      setCustomer({ ...customer });
    }
    // Add mode edit from grid row action.
    else if (ticket.id < 0) {
      const index = customer.tickets.findIndex((x) => x.id === ticket.id);
      customer.tickets[index] = ticket;
    }

    elementsVisibility.ticketVisible = false;
    setElementsVisibility({ ...elementsVisibility });
  };

  const handleSubmit = (event: any) => {
    event.preventDefault();

    if (customer.id > 0) {
      ApiService.update("customers", customer);
    } else {
      ApiService.create("customers", customer).then((x) => {
        navigate("/customers/" + x?.id + "/view");
      });
    }
  };

  //
  // Handle grid row actions.
  //

  const handleEditContactInformation = (rowData: ContactInformationDto) => {
    setContactInformation({ ...rowData });

    elementsVisibility.contactInformationVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const handleDeleteContactInformation = (rowData: ContactInformationDto) => {
    // Find customer index and delete him.
    const index = customer.contactInformations.indexOf(rowData, 0);
    if (index > -1) {
      customer.contactInformations.splice(index, 1);
    }
    setCustomer({ ...customer });

    elementsVisibility.contactInformationDeleteVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const handleEditTicket = (rowData: TicketDto) => {
    setTicket({ ...rowData });

    elementsVisibility.ticketVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const handleDeleteTicket = (rowData: TicketDto) => {
    // Find customer index and delete him.
    const index = customer.tickets.indexOf(rowData, 0);
    if (index > -1) {
      customer.tickets.splice(index, 1);
    }
    setCustomer({ ...customer });

    elementsVisibility.contactInformationDeleteVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  //
  //  Handle modals open/close.
  //
  const openAddContactInformation = () => {
    setContactInformation(new ContactInformationDto());
    elementsVisibility.contactInformationVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const openAddTicket = () => {
    setTicket(new TicketDto());
    elementsVisibility.ticketVisible = true;
    setElementsVisibility({ ...elementsVisibility });
  };

  const onHideModal = () => {
    elementsVisibility.contactInformationVisible = false;
    elementsVisibility.ticketVisible = false;
    elementsVisibility.contactInformationDeleteVisible = false;

    setElementsVisibility({ ...elementsVisibility });
  };

  //
  // Handle HTML templates
  //
  const renderHeader = (formType: FormType) => {
    let onClickFn: any = () => {};

    if (formType === FormType.ContactInformation) {
      onClickFn = openAddContactInformation;
    } else if (formType === FormType.Tickets) {
      onClickFn = openAddTicket;
    }
    return (
      <div className="flex justify-content-between">
        <div></div>
        <Button
          type="button"
          icon="pi pi-plus"
          label="Add"
          outlined
          onClick={onClickFn}
        />
      </div>
    );
  };

  const dialogFooter = (formType: FormType) => (
    <React.Fragment>
      <Button
        label="Cancel"
        icon="pi pi-times"
        outlined
        onClick={onHideModal}
      />
      {formType === FormType.ContactInformation && (
        <Button
          label="Save"
          icon="pi pi-check"
          onClick={handleContactInformationSave}
        />
      )}
      {formType === FormType.Tickets && (
        <Button
          label="Save"
          icon="pi pi-check"
          onClick={handleTicketSave}
        />
      )}
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

  const ticketGridRowActions = (rowData: TicketDto) => (
    <React.Fragment>
      <Button
        icon="pi pi-pencil"
        rounded
        outlined
        className="mr-2"
        onClick={() => handleEditTicket(rowData)}
      />
      <Button
        icon="pi pi-trash"
        rounded
        outlined
        severity="danger"
        onClick={() => handleDeleteTicket(rowData)}
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
                      onChange={(e) => handleChange(e, FormType.Customer)}
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
                      onChange={(e) => handleChange(e, FormType.Customer)}
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

        <div className="grid mt-2  ">
          <div className="col-12 lg:col-7 ">
            <Card
              title="Tickets "
              className=""
            >
              <DataTable
                value={customer.tickets}
                stripedRows
                editMode="row"
                dataKey="id"
                className="w-full"
                scrollable
                tableStyle={{ minWidth: "50rem" }}
                header={renderHeader(FormType.Tickets)}
              >
                <Column
                  field="id"
                  header="Id"
                  style={{ width: "20%" }}
                ></Column>
                <Column
                  field="title"
                  header="Ttile"
                  style={{ width: "20%" }}
                ></Column>
                <Column
                  field="description"
                  header="Description"
                  style={{ width: "40%" }}
                ></Column>
                <Column
                  field="completedOn"
                  header="Completed On"
                  style={{ width: "5%" }}
                ></Column>
                <Column
                  headerStyle={{ width: "10%", minWidth: "8rem" }}
                  body={ticketGridRowActions}
                ></Column>
              </DataTable>
            </Card>
          </div>

          <div className="col-12 lg:col-5">
            <Card
              title="Contact Information "
              className=""
            >
              <DataTable
                value={customer.contactInformations}
                editMode="row"
                dataKey="id"
                className="w-full"
                scrollable
                tableStyle={{ minWidth: "50rem" }}
                header={renderHeader(FormType.ContactInformation)}
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
      </div>

      {/*                                   */}
      {/*      Modal of Ticket Form         */}
      {/*                                   */}
      <Dialog
        visible={elementsVisibility.ticketVisible}
        style={{ width: "35%" }}
        header="Add Ticket"
        modal
        className="p-fluid"
        footer={dialogFooter(FormType.Tickets)}
        onHide={onHideModal}
      >
        <div className="field">
          <label htmlFor="id">Id</label>
          <InputNumber
            id="id"
            name="Id"
            value={ticket.id}
            disabled
          />
        </div>
        <div className="field">
          <label htmlFor="type">Type</label>
          <Dropdown
            id="type"
            name="type"
            value={ticket.type}
            onChange={(e) => handleChange(e, FormType.Tickets)}
            options={Object.keys(TicketTypesEnum)}
            optionLabel="type"
            placeholder="Select Type"
            className="w-full md:w-14rem"
            checkmark={true}
            highlightOnSelect={true}
          />
        </div>
        <div className="field">
          <label htmlFor="status">Status</label>
          <Dropdown
            id="status"
            name="status"
            value={ticket.status}
            onChange={(e) => handleChange(e, FormType.Tickets)}
            options={Object.keys(TicketStatusEnum)}
            optionLabel="type"
            placeholder="Select Type"
            className="w-full md:w-14rem"
            checkmark={true}
            highlightOnSelect={true}
          />
        </div>
        <div className="field">
          <label htmlFor="description">Description</label>
          <Editor
            id="description"
            name="description"
            value={ticket.descriptionHTML}
            onTextChange={handleEditorChange}
            style={{ height: "320px" }}
          />
        </div>
        {/* multiple fields */}
        {/* 
        <div className="flex align-items-center justify-content-center">
          <div className="card flex flex-column md:flex-row gap-3">
            <div className="p-inputgroup flex-1"></div>
            <div className="p-inputgroup flex-1"></div>
          </div>
        </div> */}
      </Dialog>

      {/*                                   */}
      {/* Modal of Contact Information Form */}
      {/*                                   */}
      <Dialog
        visible={elementsVisibility.contactInformationVisible}
        style={{ width: "35%" }}
        header="Add Contact Information"
        modal
        className="p-fluid"
        footer={dialogFooter(FormType.ContactInformation)}
        onHide={onHideModal}
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
              onChange={(e) => handleChange(e, FormType.ContactInformation)}
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
            onChange={(e) => handleChange(e, FormType.ContactInformation)}
            required
            rows={3}
            cols={20}
          />
        </div>
      </Dialog>

      {/*                                   */}
      {/*        Modal for Delete           */}
      {/*                                   */}
      <Dialog
        visible={elementsVisibility.contactInformationDeleteVisible}
        style={{ width: "35%" }}
        header="Confirm"
        modal
        className="p-fluid"
        footer={dialogFooter(FormType.Customer)}
        onHide={onHideModal}
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
