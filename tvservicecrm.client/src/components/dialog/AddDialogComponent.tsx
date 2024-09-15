import React, { ReactElement, useState } from "react";
import { Button } from "primereact/button";
import { Dialog } from "primereact/dialog";

interface IField {
  children: ReactElement;
  onSaveButtonClick: () => void;
  triggerDialogVisibility: (callback: (value: boolean) => void) => void;
  triggerSaveDisable: (callback: (value: boolean) => void) => void;
  triggerSaveEnable: (callback: (value: boolean) => void) => void;
}

export default function AddDialogComponent({
  children,
  onSaveButtonClick,
  triggerDialogVisibility,
  triggerSaveDisable,
  triggerSaveEnable,
}: IField) {
  const [isVisible, setIsVisible] = useState(false);
  const [isEnabled, setIsEnabled] = useState(false);

  React.useEffect(() => {
    triggerDialogVisibility((value: boolean) => setIsVisible(value));
  }, [triggerDialogVisibility]);

  React.useEffect(() => {
    triggerSaveDisable((value: boolean) => setIsEnabled(value));
  }, [triggerSaveDisable, triggerSaveEnable]);

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
        onClick={() => onSaveButtonClick()}
        disabled={!isEnabled}
      />
    </React.Fragment>
  );

  return (
    <>
      <Dialog
        visible={isVisible}
        style={{ width: "45%" }}
        header="Add"
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
