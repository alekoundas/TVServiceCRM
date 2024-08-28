import { ContactInformationDto } from "./ContactInformationDto";
import { TicketDto } from "./TicketDto";

export class CustomerDto {
  id: number = 0;
  firstName: string = "";
  lastName: string = "";
  createdOn: Date = new Date();

  contactInformations: ContactInformationDto[] = [];
  tickets: TicketDto[] = [];
}
