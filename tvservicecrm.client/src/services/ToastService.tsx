import { Toast } from "primereact/toast";
import { RefObject } from "react";

export namespace ToastService {
  let toastRef: RefObject<Toast>;

  export function setToastRef(toastRefValue: RefObject<Toast>) {
    toastRef = toastRefValue;
  }

  export function asdas() {
    toastRef.current?.show({
      severity: "success",
      summary: "Success",
      detail: "Update was successfull.",
    });
  }
}
