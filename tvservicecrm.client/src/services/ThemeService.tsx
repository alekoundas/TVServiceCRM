import { RefObject } from "react";
import { PrimeTheme } from "../model/PrimeTheme";

export namespace ThemeService {
  let HTMLRef: RefObject<HTMLLinkElement>;

  export function setRef(ref: RefObject<HTMLLinkElement>) {
    HTMLRef = ref;
  }

  export function getDefaultTheme(): PrimeTheme {
    return new PrimeTheme("bootstrap4-light-blue");
  }

  export function showTheme(primeTheme: PrimeTheme) {
    if (HTMLRef.current) {
      HTMLRef.current.href = primeTheme.theme;
    }
  }

  export function getDarkThemes(): PrimeTheme[] {
    const themes: PrimeTheme[] = [];

    themes.push(new PrimeTheme("lara-dark-purple"));
    themes.push(new PrimeTheme("lara-dark-blue"));
    themes.push(new PrimeTheme("lara-dark-indigo"));
    themes.push(new PrimeTheme("bootstrap4-dark-blue"));
    themes.push(new PrimeTheme("bootstrap4-dark-purple"));

    return themes;
  }

  export function getLightThemes(): PrimeTheme[] {
    const themes: PrimeTheme[] = [];

    themes.push(new PrimeTheme("lara-light-purple"));
    themes.push(new PrimeTheme("lara-light-blue"));
    themes.push(new PrimeTheme("lara-light-indigo"));
    themes.push(new PrimeTheme("bootstrap4-light-blue"));
    themes.push(new PrimeTheme("bootstrap4-light-purple"));

    return themes;
  }
}
