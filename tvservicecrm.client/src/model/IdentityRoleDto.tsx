export class IdentityRoleDto {
  id: number = 0;
  name: string = "";
  normalizedName: string = "";
  claims: string[] = [];
}

export interface IdentityRoleDto {
  [key: string]: any;
  id: number;
  name: string;
  normalizedName: string;
  claims: string[];
}
