import { ApiResponse } from "../model/ApiResponse";
import { DataTableDto } from "../model/DataTableDto";
import { UserLoginRequestDto } from "../model/UserLoginRequestDto";
import { UserRefreshTokenDto } from "../model/UserRefreshTokenDto";
import { LocalStorageService } from "./LocalStorageService";
import { ToastService } from "./ToastService";
import { TokenService } from "./TokenService";

export default class ApiService {
  // static serverUrl = "https://alexps.gr/api/";
  static serverUrl = "http://localhost:8080/api/";

  public static async get<TEntity>(
    controller: string,
    id: number | string
  ): Promise<TEntity | null> {
    const url = this.serverUrl + controller + "/" + id;
    return await this.apiRequest(url, "GET");
  }

  public static async getDataGrid<TEntity>(
    controller: string,
    data: DataTableDto<TEntity>
  ): Promise<DataTableDto<TEntity> | null> {
    const url = this.serverUrl + controller + "/GetDataTable";
    return await this.apiRequest(url, "POST", data);
  }

  public static async create<TEntity>(
    controller: string,
    data: TEntity
  ): Promise<TEntity | null> {
    const url = this.serverUrl + controller;
    return await this.apiRequest(url, "POST", data);
  }

  public static async delete<TEntity>(
    controller: string,
    id: number | string
  ): Promise<TEntity | null> {
    const url = this.serverUrl + controller + "/" + id;
    return await this.apiRequest(url, "DELETE");
  }

  public static async update<TEntity>(
    controller: string,
    data: TEntity,
    id: number | string
  ): Promise<TEntity | null> {
    const url = this.serverUrl + controller + "/" + id;
    return await this.apiRequest(url, "PUT", data);
  }

  public static async login(data: UserLoginRequestDto, authLogin: () => void) {
    const url = this.serverUrl + "users/login";

    await this.apiRequest(url, "POST", data).then((result) => {
      if (result?.isSucceed && result.data) {
        const expireDate = new Date(new Date().getTime() + 604800 * 1000);

        TokenService.setAccessToken(result.data.accessToken);
        TokenService.setRefreshToken(result.data.refreshToken);
        TokenService.setRefreshTokenExpireDate(expireDate.toString());
        authLogin();

        ToastService.showSuccess("Login was successfull!");
      } else {
        ToastService.showError("Login Failed!");
      }
    });
  }

  public static async logout(authLogout: () => void) {
    const url = this.serverUrl + "users/logout";
    await this.apiRequest<ApiResponse<boolean>>(url, "POST").then((result) => {
      if (result?.isSucceed && result.data) {
        TokenService.logout();
        authLogout();
        ToastService.showSuccess("Logout was successfull!");
      } else {
        ToastService.showError("Logout Failed!");
      }
    });
  }

  public static async test<TEntity>() {
    const url = this.serverUrl + "users/profile";
    await this.apiRequest<TEntity>(url, "POST");
  }

  private static async apiRequest<TEntity>(
    url: string,
    method: string,
    data?: TEntity
  ): Promise<TEntity | null> {
    let token = LocalStorageService.getAccessToken();
    try {
      const result = await this.apiFetch<TEntity>(
        url,
        method,
        token,
        data
      ).then((response) => {
        if (response.ok) {
          return response;
        }

        if (response.status === 401) {
          // Refresh Token Expired!
          if (TokenService.isRefreshTokenExpired()) {
            ToastService.showWarn("Token expired. Login Required.");
            TokenService.logout();
            return null;
          }

          // Access Token Expired!
          // Try to renew token and re-execute earlier query.
          if (TokenService.isTokenExpired()) {
            ToastService.showInfo("Token expired. Trying to renew.");
            return this.refreshUserToken().then((isSuccess) => {
              if (isSuccess) {
                ToastService.showInfo(
                  "Token renewed. Trying earlier query again."
                );

                token = LocalStorageService.getAccessToken();
                return this.apiFetch(url, method, token, data);
              }
            });
          }
        }
      });

      if (!result) {
        ToastService.showError("Something didnt go as planned.");
        return null;
      }

      const responseJson = await result.json();
      return responseJson;
    } catch (error) {
      ToastService.showError(
        "Something unexpected happend! API call was not successfull..."
      );
      console.error(error);
      return null;
    }
  }

  private static async refreshUserToken(): Promise<boolean> {
    const url = this.serverUrl + "users/refreshToken";
    const refreshTokenDto = TokenService.getUserRefreshTokenDto();
    try {
      const refreshTokenResponse = await this.apiFetch(
        url,
        "POST",
        null,
        refreshTokenDto
      ).then((response) => {
        if (!response.ok) {
          return false;
        }

        const refreshTokenPromise: Promise<ApiResponse<UserRefreshTokenDto>> =
          response.json();

        return refreshTokenPromise.then((result) => {
          if (!result.data) {
            return false;
          }

          // authLogin();
          TokenService.setAccessToken(result.data.accessToken);
          TokenService.setRefreshToken(result.data.refreshToken);
          TokenService.setRefreshTokenExpireDate(result.toString());
          return true;
        });
      });

      return refreshTokenResponse;
    } catch (error) {
      ToastService.showError(
        "Something unexpected happend! API call was not successfull..."
      );
      console.error(error);
      return false;
    }
  }

  private static async apiFetch<TEntity>(
    url: string,
    method: string,
    token?: string | null,
    data?: TEntity | null
  ): Promise<Response> {
    return await fetch(url, {
      method: method,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(data),
    });
  }
}
