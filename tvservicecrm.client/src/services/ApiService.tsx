import { ApiResponse } from "../model/ApiResponse";
import { DataTableDto } from "../model/DataTableDto";
import { UserLoginRequestDto } from "../model/UserLoginRequestDto";
import { UserLoginResponseDto } from "../model/UserLoginResponseDto";
import { AuthService } from "./AuthService";
import { LocalStorageService } from "./LocalStorageService";
import { ToastService } from "./ToastService";

export class ApiService {
  static serverUrl = "https://alexps.gr/api/";
  // static serverUrl = "http://localhost:8080/api/";
  // private static { isUserAuthenticated, login, logout } = useAuth();

  static async get<TEntity>(
    controller: string,
    id: number | string
  ): Promise<TEntity | null> {
    try {
      const url = this.serverUrl + controller + "/" + id;
      const response = await fetch(url, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      });

      const responseJson = await response.json();

      return responseJson;
    } catch (error) {
      return null;
      console.error(error);
    }
  }

  static async getDataGrid<TEntity>(
    controller: string,
    dataTableDto: DataTableDto<TEntity>
  ): Promise<DataTableDto<TEntity> | null> {
    try {
      dataTableDto.data = [];
      const response = await fetch(
        this.serverUrl + controller + "/GetDataTable",
        {
          method: "POST",
          body: JSON.stringify(dataTableDto),
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      const responseJson = await response.json();

      return responseJson;
    } catch (error) {
      return null;
      console.error(error);
    }
  }

  static async create<TEntity>(
    controller: string,
    data: TEntity
  ): Promise<TEntity | null> {
    try {
      const response = await fetch(this.serverUrl + controller, {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      });

      const responseJson = await response.json();

      return responseJson;
    } catch (error) {
      return null;
      console.error(error);
    }
  }

  static async update<TEntity>(
    controller: string,
    data: TEntity,
    id: number
  ): Promise<TEntity | null> {
    try {
      const url = this.serverUrl + controller + "/" + id;

      const response = await fetch(url, {
        method: "PUT",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
        },
      });

      const responseJson = await response.json();

      return responseJson;
    } catch (error) {
      return null;
      console.error(error);
    }
  }

  static async login(
    data: UserLoginRequestDto,
    onLoginSuccess: () => void
  ): Promise<ApiResponse<UserLoginResponseDto> | null> {
    try {
      const url = this.serverUrl + "users/login";

      const response = await fetch(url, {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
          "Content-Type": "application/json",
          // accept: "text/plain",
        },
      });

      const result: ApiResponse<UserLoginResponseDto> = await response.json();
      if (result.isSucceed && result.data) {
        const expireDate = new Date(new Date().getTime() + 604800 * 1000);
        ToastService.showSuccess("Login was successfull!");

        AuthService.setAccessToken(result.data.accessToken);
        AuthService.setRefreshToken(result.data.refreshToken);
        AuthService.setRefreshTokenExpireDate(expireDate.toString());
        onLoginSuccess();
      } else {
        ToastService.showError("Login Failed!");
      }

      return result;
    } catch (error) {
      ToastService.showError(
        "Something unexpected happend! API call was not successfull"
      );
      console.error(error);
      return null;
    }
  }

  static async logout(
    onLogoutSuccess: () => void
  ): Promise<ApiResponse<UserLoginResponseDto> | null> {
    try {
      const token = LocalStorageService.getAccessToken();
      const url = this.serverUrl + "users/logout";

      const response = await fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
          // accept: "text/plain",
        },
      });

      const result: ApiResponse<UserLoginResponseDto> = await response.json();
      if (result.isSucceed && result.data) {
        ToastService.showSuccess("Logout was successfull!");
        AuthService.logout();
        onLogoutSuccess();
      } else {
        ToastService.showError("Login Failed!");
      }

      return result;
    } catch (error) {
      ToastService.showError(
        "Something unexpected happend! API call was not successfull"
      );
      console.error(error);
      return null;
    }
  }

  static async test<TEntity>(): Promise<TEntity | null> {
    const token = LocalStorageService.getAccessToken();
    try {
      const response = await fetch(this.serverUrl + "users/profile", {
        credentials: "include",
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      const responseJson = await response.json();

      return responseJson;
    } catch (error) {
      return null;
      console.error(error);
    }
  }
}
export default ApiService;
