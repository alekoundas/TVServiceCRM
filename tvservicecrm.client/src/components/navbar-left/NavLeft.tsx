import { useRef } from "react";
import { Menu } from "primereact/menu";
import { MenuItem } from "primereact/menuitem";
import { Toast } from "primereact/toast";
import { useNavigate } from "react-router-dom";

function NavLeft() {
  const toast = useRef<Toast>(null);
  const navigate = useNavigate();

  const items: MenuItem[] = [
    {
      label: "",
      items: [
        {
          label: "Customers",
          icon: "pi pi-user",
          command: () => {
            navigate("/customers");
          },
        },
        {
          label: "Tickets",
          icon: "pi pi-ticket",
          command: () => {
            navigate("/tickets");
          },
        },
        {
          label: "Users",
          icon: "pi pi-users",
          command: () => {
            navigate("/users");
          },
        },
        {
          label: "Roles",
          icon: "pi pi-key",
          command: () => {
            navigate("/roles");
          },
        },
      ],
    },
    // {
    //   label: "Profile",
    //   items: [
    //     {
    //       label: "Settings",
    //       icon: "pi pi-cog",
    //     },
    //     {
    //       label: "Logout",
    //       icon: "pi pi-sign-out",
    //     },
    //   ],
    // },
  ];

  return (
    <>
      <div
        className="card w-full"
        style={{ height: "93vh" }}
      >
        <Toast ref={toast} />
        <Menu
          model={items}
          className="h-full w-full md:w-15rem"
        />
      </div>
    </>
  );
}

export default NavLeft;
