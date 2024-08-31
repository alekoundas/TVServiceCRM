import { DataTableDto } from "../model/DataTableDto";

export class ApiService {
  static serverUrl = "http://localhost:8080/api/";
  // static myInstance = null;

  // static getInstance() {
  //   return new ApiService();
  // }

  // static async getGridData<TEntity>(
  //   controller: string
  // ): Promise<TEntity | null> {
  //   try {
  //     let response = await fetch(this.serverUrl + controller);
  //     let responseJson = await response.json();

  //     return responseJson;
  //   } catch (error) {
  //     return null;
  //     console.error(error);
  //   }
  // }

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
}
export default ApiService;
