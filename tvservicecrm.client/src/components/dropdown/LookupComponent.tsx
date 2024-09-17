import { Dropdown, DropdownChangeEvent } from "primereact/dropdown";
import React, { useState } from "react";
import ApiService from "../../services/ApiService";
import { VirtualScrollerLazyEvent } from "primereact/virtualscroller";
import { LookupDto } from "../../model/lookup/LookupDto";
import { Button } from "primereact/button";

interface IField {
  controller: string;
  idValue: string;
  isEditable: boolean;
  isEnabled: boolean;
  allowCustom: boolean;
  isCustomChange?: (isCustom: boolean) => void;
  onChange?: (id: string) => void;
}

export default function LookupComponent({
  controller,
  idValue,
  isEditable,
  isEnabled,
  allowCustom,
  isCustomChange,
  onChange,
}: IField) {
  const [isVisible, setIsVisible] = useState(false);
  const [loading, setLoading] = useState(false);
  const [lookupDto, setLookupDto] = useState<LookupDto>(new LookupDto());
  const [selectedOption, setSelectedOption] = useState<string | null>(null);

  const refreshData = async (dto: LookupDto) => {
    setLoading(true);

    return await ApiService.getDataLookup(controller, dto).then((result) => {
      lookupDto.data = result?.data;
      setLoading(false);
      return result;
    });
  };

  // Load intitial data.
  React.useEffect(() => {
    if (idValue) {
      const dto = new LookupDto();
      dto.filter.id = idValue;
      refreshData(dto).then((x) => {
        if (x?.data && x?.data[0]?.value) {
          setSelectedOption(x.data[0].value);
        }
      });
    }
  }, []);

  const onLazyLoad = async (_event: VirtualScrollerLazyEvent) => {
    refreshData(lookupDto).then((resultLookupDto) => {
      if (resultLookupDto) setLookupDto({ ...resultLookupDto });
      else setLookupDto(new LookupDto());
    });
  };

  const onSaveCustom = (event: any) => {
    if (event) event.caller;
    selectedOption;
  };

  const handleChange = (event: DropdownChangeEvent): void => {
    // Stupid PrimeRact Dropdown doesnt retrieve object but just a string.
    // This "if" statement means that user selected an option.
    if (event.originalEvent) {
      if (isCustomChange) isCustomChange(false);

      setIsVisible(false);
      setSelectedOption(event.value);
    }

    // This else statement means that user set a custom value.
    else {
      if (isCustomChange) isCustomChange(true);
      if (allowCustom) setIsVisible(true);

      lookupDto.filter.value = event.value;
      setLookupDto({ ...lookupDto });
      refreshData(lookupDto).then((resultLookupDto) => {
        if (resultLookupDto) setLookupDto({ ...resultLookupDto });
        else setLookupDto(new LookupDto());
      });
      setSelectedOption(event.value);
    }

    if (onChange) onChange(event.value);
  };

  return (
    <>
      <div className="grid ">
        <div className="col-9 pr-0">
          <div className="flex justify-center">
            <Dropdown
              optionLabel="value" // What to display in the dropdown
              optionValue="id" // The value binding (id)
              value={selectedOption}
              onChange={handleChange}
              editable={isEditable}
              options={lookupDto.data}
              placeholder="Select a Role"
              className="w-full md:w-14rem"
              disabled={!isEnabled}
              virtualScrollerOptions={{
                lazy: true,
                onLazyLoad: onLazyLoad,
                itemSize: 38,
                showLoader: true,
                loading: loading,

                delay: 250,
              }}
            />
          </div>
        </div>
        <div className="col-3 pl-0">
          <Button
            type="button"
            visible={isVisible}
            icon="pi pi-save"
            onClick={onSaveCustom}
          />
        </div>
      </div>
    </>
  );
}
