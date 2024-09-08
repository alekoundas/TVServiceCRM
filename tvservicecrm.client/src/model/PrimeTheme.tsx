export class PrimeTheme {
  theme: string = "/src/assets/themes/theme-bootstrap4-light-blue.css";
  themeImage: string = "/src/assets/theme-images/bootstrap4-light-blue.svg";

  public constructor(theme: string) {
    this.theme = "/src/assets/themes/theme-" + theme + ".css";

    if (theme.startsWith("bootstrap4"))
      this.themeImage = "/src/assets/theme-images/" + theme + ".svg";

    if (theme.startsWith("lara"))
      this.themeImage = "/src/assets/theme-images/" + theme + ".svg";
  }
}
