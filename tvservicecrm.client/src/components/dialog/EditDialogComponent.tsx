import React, { ReactNode, useState } from "react";
import { Button } from "primereact/button";
import { ButtonTypeEnum } from "../../enum/ButtonTypeEnum";
import { Dialog } from "primereact/dialog";

interface IField {
  children: ReactNode;
  onButtonClick: (buttonType: ButtonTypeEnum) => void;
  onParentVisibilityUpdate: (callback: (value: boolean) => void) => void;
}

export default function EditDialogComponent({
  children,
  onButtonClick,
  onParentVisibilityUpdate,
}: IField) {
  const [isVisible, setIsVisible] = useState(false);

  React.useEffect(() => {
    onParentVisibilityUpdate((newValue: boolean) => setIsVisible(newValue));
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
        label="Save"
        icon="pi pi-check"
        onClick={() => onButtonClick(ButtonTypeEnum.SAVE)}
      />
    </React.Fragment>
  );

  return (
    <>
      <Dialog
        visible={isVisible}
        style={{ width: "35%" }}
        header="Edit"
        modal
        className="p-fluid"
        footer={dialogFooter()}
        onHide={() => setIsVisible(false)}
      >
        {children}
      </Dialog>
    </>
  );
}
