import { CustomerDto } from "./CustomerDto";

export class TicketDto {
  id: number = 0;
  title: string = "";
  description: string = "";
  completedOn: Date = new Date();
  customerId: Date = new Date();
  customer: CustomerDto = new CustomerDto();
}
