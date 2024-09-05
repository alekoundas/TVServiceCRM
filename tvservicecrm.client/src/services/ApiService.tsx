import { DataTableDto } from "../model/DataTableDto";

export class ApiService {
  static serverUrl = "https://alexps.gr/api/";
  // static serverUrl = "http://localhost:8080/api/";

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
    data: TEntity
  ): Promise<TEntity | null> {
    try {
      const response = await fetch(this.serverUrl + controller, {
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
}
export default ApiService;
