import { Route, Routes } from "react-router-dom";
import { ScrollPanel } from "primereact/scrollpanel";
import { ToastService } from "./services/ToastService.tsx";
import { ThemeService } from "./services/ThemeService.tsx";
import { useEffect, useRef } from "react";
import { Toast } from "primereact/toast";

import NavTop from "./components/nav_top/NavTop.tsx";
import NavLeft from "./components/nav_left/NavLeft.tsx";
import Home from "./pages/home/Home.tsx";
import Customers from "./pages/customer/Customers.tsx";
import CustomerForm from "./pages/customer/CustomerForm.tsx";
import Tickets from "./pages/ticket/Tickets.tsx";
import Login from "./pages/user/Login.tsx";
import ApiService from "./services/ApiService.tsx";
import Roles from "./pages/role/Roles.tsx";

export default function App() {
  // Set Toast messages here
  const toast = useRef<Toast>(null);
  const theme = useRef<HTMLLinkElement>(null);

  useEffect(() => {
    ToastService.setToastRef(toast);
    ThemeService.setRef(theme);
    ThemeService.setDefaultTheme();
    ThemeService.setDefaultThemeScale();

    ApiService.test();
  }, []);

  return (
    <>
      {/* Theme switching here. */}
      <link
        ref={theme}
        rel="stylesheet"
        type="text/css"
      />
      <Toast ref={toast} />

      <div className=" w-full ">
        <div className="flex flex-row pb-3">
          <div className="flex flex-column w-full">
            <NavTop />
          </div>
        </div>

        <div className="flex flex-row  gap-3">
          <div className="flex-column">
            <NavLeft />
          </div>

          <div className="flex-column w-full ">
            <ScrollPanel
              style={{ height: "93vh" }}
              className="custombar1"
            >
              <Routes>
                <Route
                  path="/"
                  element={<Home />}
                />
                <Route
                  path="/customers"
                  element={<Customers />}
                />
                <Route
                  path="/customers/add"
                  element={<CustomerForm />}
                />
                <Route
                  path="/customers/:id/edit"
                  element={<CustomerForm />}
                />
                <Route
                  path="/customers/:id/view"
                  element={<CustomerForm />}
                />
                <Route
                  path="/tickets"
                  element={<Tickets />}
                />
                <Route
                  path="/user/login"
                  element={<Login />}
                />

                <Route
                  path="/user/roles"
                  element={<Roles />}
                />
              </Routes>
            </ScrollPanel>
          </div>
        </div>
      </div>
    </>
  );
}
