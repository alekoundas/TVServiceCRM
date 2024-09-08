import { Menubar } from "primereact/menubar";
import { Badge } from "primereact/badge";
import { useNavigate } from "react-router-dom";
import { Menu } from "primereact/menu";
import { Button } from "primereact/button";
import { MenuItem } from "primereact/menuitem";
import { useRef, useState } from "react";
import { Sidebar } from "primereact/sidebar";
import { ThemeService } from "../../services/ThemeService";
import { Image } from "primereact/image";

function NavTop() {
  const navigate = useNavigate();
  const menuRight = useRef<Menu>(null);

  const itemRenderer = (item: any) => (
    <a className="flex align-items-center p-menuitem-link">
      <span className={item.icon} />
      <span className="mx-2">{item.label}</span>
      {item.badge && (
        <Badge
          className="ml-auto"
          value={item.badge}
        />
      )}
      {item.shortcut && (
        <span className="ml-auto border-1 surface-border border-round surface-100 text-xs p-1">
          {item.shortcut}
        </span>
      )}
    </a>
  );
  const items = [
    {
      label: "Home",
      icon: "pi pi-home",
      command: () => {
        navigate("/");
      },
    },
    {
      label: "Features",
      icon: "pi pi-star",
    },
    {
      label: "Projects",
      icon: "pi pi-search",
      items: [
        {
          label: "Core",
          icon: "pi pi-bolt",
          shortcut: "⌘+S",
          template: itemRenderer,
        },
        {
          label: "Blocks",
          icon: "pi pi-server",
          shortcut: "⌘+B",
          template: itemRenderer,
        },
        {
          label: "UI Kit",
          icon: "pi pi-pencil",
          shortcut: "⌘+U",
          template: itemRenderer,
        },
        {
          separator: true,
        },
        {
          label: "Templates",
          icon: "pi pi-palette",
          items: [
            {
              label: "Apollo",
              icon: "pi pi-palette",
              badge: 2,
              template: itemRenderer,
            },
            {
              label: "Ultima",
              icon: "pi pi-palette",
              badge: 3,
              template: itemRenderer,
            },
          ],
        },
      ],
    },
    {
      label: "Contact",
      icon: "pi pi-envelope",
      badge: 3,
      template: itemRenderer,
    },
  ];

  const start = (
    <img
      alt="logo"
      src="https://primefaces.org/cdn/primereact/images/logo.png"
      height="40"
      className="mr-2"
    ></img>
  );

  // const dialogFooter = () => (
  //   <React.Fragment>
  //     <Button
  //       label="Cancel"
  //       icon="pi pi-times"
  //       outlined
  //     />
  //   </React.Fragment>
  // );

  const itemsSettings: MenuItem[] = [
    {
      label: "Options",
      items: [
        {
          label: "Login",
          icon: "pi pi-user",
          command: () => {
            navigate("/user/login");
          },
        },
        {
          label: "Theme",
          icon: "pi pi-palette",
          command: () => {
            setVisible(true);
          },
        },
        {
          separator: true,
        },
        // {
        //   template: dialogFooter,
        // },
      ],
    },
  ];
  const end = (
    <div>
      <Menu
        model={itemsSettings}
        popup
        ref={menuRight}
        id="popup_menu_right"
        popupAlignment="right"
      />
      <Button
        rounded
        outlined
        icon="pi pi-cog"
        aria-label="Filter"
        className="mr-2"
        onClick={(event) => menuRight.current?.toggle(event)}
        aria-controls="popup_menu_right"
        aria-haspopup
      />
    </div>
  );
  const [visible, setVisible] = useState<boolean>(false);
  return (
    <>
      <div className="card">
        <Menubar
          model={items}
          start={start}
          end={end}
        />
      </div>
      <Sidebar
        visible={visible}
        position="right"
        onHide={() => setVisible(false)}
      >
        <h2>Dark Themes:</h2>
        <div className="flex flex-wrap ">
          {ThemeService.getDarkThemes().map((col, _i) => (
            <div className="flex bg-primary m-1 border-round">
              <Button
                onClick={() => ThemeService.showTheme(col)}
                className="cursor-pointer p-link"
              >
                <Image
                  src={col.themeImage}
                  width="50"
                  alt="saga-blue"
                />
              </Button>
            </div>
          ))}
        </div>

        <h2>Light Themes:</h2>
        <div className="flex flex-wrap ">
          {ThemeService.getLightThemes().map((col, _i) => (
            <div className="flex bg-primary m-1 border-round">
              <Button
                onClick={() => ThemeService.showTheme(col)}
                className="cursor-pointer p-link"
              >
                <Image
                  src={col.themeImage}
                  width="50"
                  alt="saga-blue"
                />
              </Button>
            </div>
          ))}
        </div>
      </Sidebar>
    </>
  );
}

export default NavTop;
