import React, { useState } from "react";
import { FormMode } from "../../enum/FormMode";
import { Card } from "primereact/card";
import { InputText } from "primereact/inputtext";
import ApiService from "../../services/ApiService";
import { ToastService } from "../../services/ToastService";
import { MakerModelDto } from "../../model/MakerModelDto";

interface IField {
  data: MakerModelDto;
  formMode: FormMode;
  // onEnableSaveButton?: () => void;
  // onDisableSaveButton?: () => void;
  onAfterSave: () => void;
  triggerSaveForm?: () => void;
}

export default function MakerModelForm({
  data,
  formMode,
  // onEnableSaveButton,
  // onDisableSaveButton,
  onAfterSave,
  triggerSaveForm,
}: IField) {
  const [makerModelDto, setMakerModelDto] = useState(data);
  const isFirstRender = React.useRef(true);

  //
  // Handle save triggered from parent.
  //
  React.useEffect(() => {
    // Skip the first run
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }
    if (triggerSaveForm) {
      handleSave();
    }
  }, [triggerSaveForm]);

  const handleSave = () => {
    if (formMode == FormMode.EDIT) {
      ApiService.update("makermodels", makerModelDto, makerModelDto.id)
        .then((response) => {
          if (response) {
            ToastService.showSuccess("");
            return;
          }
          ToastService.showError("");
        })
        .then(onAfterSave);
    } else {
      ApiService.create("makermodels", makerModelDto)
        .then((response) => {
          if (response) {
            ToastService.showSuccess("");
            return;
          }
          ToastService.showError("");
        })
        .then(onAfterSave);
    }
  };

  const handleChange = (event: React.ChangeEvent<any>) => {
    const name = event.target.name;
    const value = event.target.value;

    makerModelDto[name] = value;
    setMakerModelDto({ ...makerModelDto });
  };

  // Load initial data.
  React.useEffect(() => {
    if (formMode === FormMode.EDIT || formMode === FormMode.VIEW) {
      ApiService.get<MakerModelDto>("makermodels", makerModelDto.id).then(
        (result) => {
          if (result) {
            setMakerModelDto({ ...result });
          }
        }
      );
    }
  }, []);

  return (
    <>
      <Card title="Maker Model Form ">
        <form>
          <div className="flex align-items-center justify-content-center">
            <div className="field">
              <label htmlFor="Title">Title</label>
              <InputText
                id="Title"
                name="Title"
                value={makerModelDto.name}
                onChange={handleChange}
                disabled={formMode !== FormMode.ADD}
              />
            </div>
          </div>
        </form>
      </Card>
    </>
  );
}
