import { LocalStorageService } from "./LocalStorageService";

// interface JwtData {
//   exp?: number;
//   id?: string;
//   userName?: string;
//   roleClaim?: string[];
// }

export class AuthService {
  public static isTokenExpired = (): boolean => {
    const expireDateString = LocalStorageService.getRefreshTokenExpireDate();
    if (!expireDateString) {
      return true;
    }

    const expireDate = new Date(expireDateString);

    return new Date() > expireDate;
  };

  public static logout = () => {
    LocalStorageService.setRefreshToken();
    LocalStorageService.setAccessToken();
    LocalStorageService.setRefreshTokenExpireDate();
  };

  // public static isTokenExpired = (): boolean => {
  //   const token = LocalStorageService.getRefreshToken();
  //   if (!token) {
  //     return false;
  //   }

  //   try {
  //     const decodedToken = jwtDecode<JwtData>(token);

  //     // Check if the token has an 'exp' claim
  //     if (decodedToken.exp) {
  //       // Convert 'exp' (seconds) to milliseconds and compare with the current time
  //       const expirationDate = new Date(decodedToken.exp * 1000);
  //       const currentDate = new Date();

  //       // If current time is greater than expiration time, the token is expired
  //       return currentDate > expirationDate;
  //     }

  //     return false;
  //   } catch (error) {
  //     console.error("Error decoding token:", error);
  //     return true; // If there's an error decoding the token, assume it's invalid/expired
  //   }
  // };

  public static setAccessToken = (value: string) =>
    LocalStorageService.setAccessToken(value);

  public static setRefreshToken = (value: string) =>
    LocalStorageService.setRefreshToken(value);

  public static setRefreshTokenExpireDate = (value: string) =>
    LocalStorageService.setRefreshTokenExpireDate(value);
}
