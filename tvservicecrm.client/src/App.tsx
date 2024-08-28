import NavTop from "./components/nav_top/NavTop.tsx";
import NavLeft from "./components/nav_left/NavLeft.tsx";
import Home from "./pages/home/Home.tsx";
import CustomerForm from "./pages/customer/CustomerForm.tsx";
import { ScrollPanel } from "primereact/scrollpanel";
import { Route, Routes } from "react-router-dom";
import Customers from "./pages/customer/Customers.tsx";
import Tickets from "./pages/ticket/Tickets.tsx";

function App() {
  return (
    <>
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
              </Routes>
            </ScrollPanel>
          </div>
        </div>
      </div>
    </>
  );
}

export default App;
