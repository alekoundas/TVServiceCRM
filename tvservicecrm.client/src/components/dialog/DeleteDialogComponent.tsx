import React, { useState } from "react";
import { Button } from "primereact/button";
import { ButtonTypeEnum } from "../../enum/ButtonTypeEnum";
import { Dialog } from "primereact/dialog";

interface IField {
  onButtonClick: (buttonType: ButtonTypeEnum) => void;
  onParentVisibilityUpdate: (callback: (value: boolean) => void) => void;
}

export default function DeleteDialogComponent({
  onButtonClick,
  onParentVisibilityUpdate,
}: IField) {
  const [isVisible, setIsVisible] = useState(false);

  React.useEffect(() => {
    onParentVisibilityUpdate((visibility: boolean) => setIsVisible(visibility));
  }, [onParentVisibilityUpdate]);

  const dialogFooter = () => (
    <React.Fragment>
      <Button
        label="Cancel"
        icon="pi pi-times"
        outlined
        onClick={() => setIsVisible(false)}
      />
      <Button
        label="Delete"
        icon="pi pi-trash"
        onClick={() => onButtonClick(ButtonTypeEnum.DELETE)}
      />
    </React.Fragment>
  );

  return (
    <>
      <Dialog
        visible={isVisible}
        style={{ width: "35%" }}
        header="Confirm"
        modal
        className="p-fluid"
        footer={dialogFooter()}
        onHide={() => setIsVisible(false)}
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
