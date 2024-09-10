export class LocalStorageService {
  public static getThemeName = (): string | null => {
    return localStorage.getItem("themeName");
  };

  public static getThemeScale = (): string | null => {
    return localStorage.getItem("themeScale");
  };

  public static setThemeScale = (value: string) => {
    localStorage.setItem("themeScale", value);
  };

  public static setThemeName = (value: string) => {
    localStorage.setItem("themeName", value);
  };
}
