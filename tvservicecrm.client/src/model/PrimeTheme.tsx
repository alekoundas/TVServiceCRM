export class PrimeTheme {
  theme: string = "/public/themes/theme-bootstrap4-light-blue.css";
  themeImage: string = "/public/theme-images/bootstrap4-light-blue.svg";

  public constructor(theme: string) {
    this.theme = "public/themes/theme-" + theme + ".css";

    if (theme.startsWith("bootstrap4"))
      this.themeImage = "public/theme-images/" + theme + ".svg";

    if (theme.startsWith("lara"))
      this.themeImage = "public/theme-images/" + theme + ".svg";
  }
}
