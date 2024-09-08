import { Toast } from "primereact/toast";
import { RefObject } from "react";

export namespace ToastService {
  let toastRef: RefObject<Toast>;

  export function setToastRef(toastRefValue: RefObject<Toast>) {
    toastRef = toastRefValue;
  }

  export function showSuccess() {
    toastRef.current?.show({
      severity: "success",
      summary: "Success",
      detail: "Update was successfull.",
    });
  }

  export function showInfo() {
    toastRef.current?.show({
      severity: "info",
      summary: "Info",
      detail: "Message Content",
      life: 3000,
    });
  }

  export function showWarn() {
    toastRef.current?.show({
      severity: "warn",
      summary: "Warning",
      detail: "Message Content",
      life: 3000,
    });
  }

  export function showError(message: string) {
    toastRef.current?.show({
      severity: "error",
      summary: "Error",
      detail: message,
      life: 3000,
    });
  }

  export function showSecondary() {
    toastRef.current?.show({
      severity: "secondary",
      summary: "Secondary",
      detail: "Message Content",
      life: 3000,
    });
  }

  export function showContrast() {
    toastRef.current?.show({
      severity: "contrast",
      summary: "Contrast",
      detail: "Message Content",
      life: 3000,
    });
  }
}
