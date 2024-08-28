import { CustomerDto } from "./CustomerDto";

export class TicketDto {
  id: number;
  title: string = "";
  description: string = "";
  completedOn: Date;
  customerId: Date;
  customer: CustomerDto;
}
